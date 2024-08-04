using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Mediator.Examples;

public class ModelRequest(string email) : IRequest<ModelRequest.Model>
{
    private string Email { get; } = email;

    public record Model(string Email);

    public class ModelHandler() : IRequestHandler<ModelRequest, Model>
    {
        public async Task<Model> Handle(ModelRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            return new Model(request.Email);
        }
    }
}