using Generacion.Application.Common;
using Generacion.Application.DashBoard.ControlGAS.Query;
using Generacion.Application.FiltroCentrifugo.Command;
using Generacion.Models;
using Generacion.Views.ControlGAS;
using MediatR;
using NuGet.Packaging;

namespace Generacion.Application.DashBoard
{
    public class ObtenerDatosDashBoard : IRequest<Respuesta<Dictionary<string, object>>>
    { 
    }

    public class ObtenerDatosDashBoardHanlder : IRequestHandler<ObtenerDatosDashBoard, Respuesta<Dictionary<string, object>>>
    {
        private readonly ISender _sender;
        private readonly ObtenerDetalleConsumoGas _obtenerDetalleConsumoGas;

        public ObtenerDatosDashBoardHanlder(ISender sender, ObtenerDetalleConsumoGas obtenerDetalleConsumoGas)
        {
            _obtenerDetalleConsumoGas = obtenerDetalleConsumoGas;
            _sender = sender;
        }
        public async Task<Respuesta<Dictionary<string, object>>> Handle(ObtenerDatosDashBoard request, CancellationToken cancellationToken)
        {
            Respuesta<Dictionary<string, object>> respuesta = new Respuesta<Dictionary<string, object>>();
            respuesta.Detalle = new Dictionary<string, object>();

            var datosFiltro = await ObtenerDatosFiltro();
            respuesta.Detalle.AddRange(datosFiltro);

            var datosConmsumoGas = await ObtenerDetalleConsumoGas();
            respuesta.Detalle.AddRange(datosConmsumoGas);

            return respuesta;
        }
        public async Task<Dictionary<string, object>> ObtenerDetalleConsumoGas()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            var datoContrato = await _obtenerDetalleConsumoGas.ObtenerContratoGas();
            respuesta.Add("datoContrato", datoContrato.Detalle);

            var datosConsumo = await _obtenerDetalleConsumoGas.ObtenerDatosConsumoGas();
            respuesta.Add("datosConsumo", datosConsumo.Detalle);

            var datodetalleConsumo = datosConsumo.Detalle != null
                                    ? await _obtenerDetalleConsumoGas.ObtenerDetalleDiarioConsumoGas(datosConsumo.Detalle.IdConsumoGas)
                                    : new Respuesta<DetalleConsumoGas>();
            respuesta.Add("datodetalleConsumo", datodetalleConsumo.Detalle);

            return respuesta;

        }

        public async Task<Dictionary<string, object>> ObtenerDatosFiltro()
        {
            Dictionary<string, object> respuesta = new Dictionary<string, object>();

            MantenimientoComponentes requestCentrifugo = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroCentrifugo,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuestaCentrifugo = await _sender.Send(requestCentrifugo);

            MantenimientoComponentes requestAutomatico = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroAutomatico,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuestaAutomatico = await _sender.Send(requestAutomatico);

            respuesta["datosFiltroCentrifugo"] = respuestaCentrifugo.Detalle["datosDashboard"];
            respuesta["datosFiltroAutomatico"] = respuestaAutomatico.Detalle["datosDashboard"];

            return respuesta;
        }
    }
}
