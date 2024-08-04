using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace MediatR;

// optional jbogard/MediatR compatible extension for registration
public static partial class MediatorExtensions
{
    public static void AddMediatR(this IServiceCollection services, params Type[] handlerAssemblyMarkupTypes)
    {
        AddMediatR(services, handlerAssemblyMarkupTypes.Select(type => type.GetTypeInfo().Assembly).ToArray());
    }

    public static void AddMediatR(this IServiceCollection services, Action<MediatRServiceConfiguration> configuration)
    {
        var serviceConfig = new MediatRServiceConfiguration();

        configuration.Invoke(serviceConfig);

        if (!serviceConfig.AssembliesToRegister.Any())
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");

        AddMediatR(services, serviceConfig.AssembliesToRegister.ToArray());
    }
}

public class MediatRServiceConfiguration
{
    internal List<Assembly> AssembliesToRegister { get; } = new();
    public void RegisterServicesFromAssemblyContaining<T>() => RegisterServicesFromAssemblyContaining(typeof(T));
    public void RegisterServicesFromAssemblyContaining(Type type) => RegisterServicesFromAssembly(type.Assembly);
    public void RegisterServicesFromAssembly(Assembly assembly) => AssembliesToRegister.Add(assembly);
    public void RegisterServicesFromAssemblies(params Assembly[] assemblies) => AssembliesToRegister.AddRange(assemblies);
}
