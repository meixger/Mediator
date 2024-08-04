using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Mediator.Services;

public record Notification(string Input) : INotification;

public class NotificationHandler1 : INotificationHandler<Notification>
{
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"NotificationHandler #1 '{notification.Input}'");
        return Task.CompletedTask;
    }
}

public class NotificationHandler2 : INotificationHandler<Notification>
{
    public async Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"NotificationHandler #2 '{notification.Input}'");
    }
}