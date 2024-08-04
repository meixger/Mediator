using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Mediator.Services;

public class ThrowingNotificationHandler : INotificationHandler<ThrowingNotificationHandler.ThrowingNotification>
{
    public record ThrowingNotification : INotification { }

    public Task Handle(ThrowingNotification notification, CancellationToken cancellationToken)
    {
        throw new OperationCanceledException("investigate stack trace");
    }
}