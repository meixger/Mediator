using Mediator.Examples;
using Mediator.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();

public class Worker(IHostApplicationLifetime hostApplicationLifetime, IMediator mediator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(100, stoppingToken);

        // Notification with 2 Handler
        await mediator.Publish(new Notification("user@example.com"), stoppingToken);

        // Request without Response
        await mediator.Send(new EmailRequestHandler.Request("user@example.com"));

        // Request with Response
        var viewModel = await mediator.Send(new ModelRequest("user@example.com"), stoppingToken);
        Console.WriteLine(viewModel);

        hostApplicationLifetime.StopApplication();
    }
}