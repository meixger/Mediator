using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Mediator.Examples;
using Mediator.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Tests;

[TestClass]
public class SimpleTests
{
    private readonly IMediator _mediator;

    public SimpleTests()
    {
        var service = new ServiceCollection();
        service.AddMediatR(GetType());
        var serviceProvider = service.BuildServiceProvider();
        _mediator = serviceProvider.GetRequiredService<IMediator>();
    }

    [TestMethod]
    public async Task Notification()
    {
        await _mediator.Publish(new Notification("user@example.com"));
    }

    [TestMethod]
    public async Task ThrowingNotification()
    {
        var e = await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => _mediator.Publish(new ThrowingNotificationHandler.ThrowingNotification()));
        Assert.AreEqual("investigate stack trace", e.Message);
        Console.WriteLine($"got {e.GetType().Name} '{e.Message}'");
    }

    [TestMethod]
    public async Task RequestWithoutResponse()
    {
        await _mediator.Send(new EmailRequestHandler.Request("user@example.com"));
    }

    [TestMethod]
    public async Task RequestWithResponse()
    {
        var model = await _mediator.Send(new ModelRequest("user@example.com"));
        Assert.AreEqual("user@example.com", model.Email);
        Console.WriteLine($"got model with '{model.Email}'");
    }
}