using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Mediator.Examples;

public class EmailRequestHandler : IRequestHandler<EmailRequestHandler.Request>
{
    public record Request(string Input): IRequest;

    public async Task Handle(Request request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending email to '{request.Input}'");
        await Task.Delay(10, cancellationToken);
    }
}