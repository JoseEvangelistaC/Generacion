using Generacion.Models;
using Generacion.Views.ControlGAS;
using MediatR;

namespace Generacion.Application.DashBoard.ControlGAS.Command
{
    public class GuardarContratoGas : IRequest<Respuesta<string>>
    {
        public ContratoGas contratoGas { get; init; }
    }
    public class RegistroControlGasHandler : IRequestHandler<GuardarContratoGas, Respuesta<string>>
    {
        private readonly IRegistroControlGas _registroControlGas;
        public RegistroControlGasHandler(IRegistroControlGas registroControlGas)
        {
            _registroControlGas = registroControlGas;
        }
        public async Task<Respuesta<string>> Handle(GuardarContratoGas request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            respuesta = await _registroControlGas.RegistrarContratosControl(request.contratoGas);

            return respuesta; 
        }
    }
}
