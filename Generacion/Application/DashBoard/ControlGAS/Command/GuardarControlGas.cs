using Generacion.Models;
using Generacion.Views.ControlGAS;
using MediatR;

namespace Generacion.Application.DashBoard.ControlGAS.Command
{
    public class GuardarControlGas : IRequest<Respuesta<string>>
    {
        public ConsumoGas consumoGas { get; init; }
    }

    public class GuardarControlGasHandler : IRequestHandler<GuardarControlGas, Respuesta<string>>
    {
        private readonly IRegistroControlGas _registroControlGas;

        public GuardarControlGasHandler(IRegistroControlGas registroControlGas)
        {
            _registroControlGas = registroControlGas;
        }
        public async Task<Respuesta<string>> Handle(GuardarControlGas request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            respuesta = await _registroControlGas.RegistrarDatosControl(request.consumoGas);

            return respuesta;
        }
    }
}
