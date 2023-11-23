using Generacion.Models;
using Generacion.Models.Aceite;
using MediatR;

namespace Generacion.Application.DashBoard.CambioAceite.Command
{
    public class ListaControlCambioAceite : IRequest<Respuesta<string>>
    {
        public List<ControlCambioAceite> datosControl { get; set; }
    }

    public class ControlCambioAceiteHandler : IRequestHandler<ListaControlCambioAceite, Respuesta<string>>
    {
        public readonly IRegistroControlAceite _registroControlAceite;
        public ControlCambioAceiteHandler( IRegistroControlAceite registroControlAceite)
        {
            _registroControlAceite = registroControlAceite;
        }
        public async Task<Respuesta<string>> Handle(ListaControlCambioAceite request, CancellationToken cancellationToken)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            foreach (var item in request.datosControl)
            {
                if (respuesta.IdRespuesta==99)
                    continue;

                respuesta = await _registroControlAceite.GuardarDatosControlAceite(item);
            }


            return respuesta;
        }
    }
}
