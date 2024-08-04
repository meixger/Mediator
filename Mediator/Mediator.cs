using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MediatR;

public static partial class MediatorExtensions
{
    public static void AddMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(assembly => assembly.GetTypes()).ToList();

        types.Where(type => type.GetInterfaces().Any(IsHandler)).ToList().ForEach(type => type.GetInterfaces().Where(IsHandler).ToList().ForEach(@interface => services.AddScoped(@interface, type)));

        services.AddScoped<IMediator, Mediator>();

        return;

        static bool IsHandler(Type type) => IsType(type, typeof(IRequestHandler<>)) || IsType(type, typeof(IRequestHandler<,>)) || IsType(type, typeof(INotificationHandler<>));

        static bool IsType(Type type, MemberInfo memberInfo) => type.IsGenericType && type.GetGenericTypeDefinition() == memberInfo;
    }
}

public interface IMediator
{
    /// <summary>
    /// Publish a INotificaton to multiple INotificationHandler
    /// </summary>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;

    /// <summary>
    /// Send a IRequest to a single IRequestHandler
    /// </summary>
    Task Send<TRequest>(in TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

    /// <summary>
    /// Send a IRequest to a single IRequestHandler&lt;TResponse&gt; and returns a TResponse
    /// </summary>
    Task<TResponse> Send<TResponse>(in IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public interface INotification;

public interface IRequest;

public interface IRequest<out TResponse>;

public interface INotificationHandler<TNotification>
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}

public interface IRequestHandlerBase<TResponse>
{
    Task<TResponse> Handle(object request, CancellationToken cancellationToken);
}

public interface IRequestHandler<in TRequest>
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandler<in TRequest, TResponse> : IRequestHandlerBase<TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    Task<TResponse> IRequestHandlerBase<TResponse>.Handle(object request, CancellationToken cancellationToken)
        => Handle((TRequest)request, cancellationToken);
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken) where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>().ToArray();
        return Task.WhenAll(handlers.Select(h => h.Handle(notification, cancellationToken)));
    }

    public Task Send<TRequest>(in TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
        return handler.Handle(request, cancellationToken);
    }

    public Task<TResponse> Send<TResponse>(in IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var requestHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = (IRequestHandlerBase<TResponse>)serviceProvider.GetRequiredService(requestHandlerType);
        return handler.Handle(request, cancellationToken);
    }
}
