using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Models.DashBoard;
using Generacion.Models;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Generacion.Application.FiltroCentrifugo;
using Generacion.Application.Funciones;
using Generacion.Models.FiltroCentrifugo;

namespace Generacion.Controllers
{
    public class FiltroCentrifugoController : Controller
    {
        private readonly IRegistroFiltroCentrifugo _registroFiltroCentrifugo;
        private readonly DatosFiltroCentrifugo _datosFiltroCentrifugo;
        private readonly Function _function;
        
        public FiltroCentrifugoController(
            DatosFiltroCentrifugo datosFiltroCentrifugo, 
            IRegistroFiltroCentrifugo registroFiltroCentrifugo,
            Function function)
        {
            _function = function;
            _registroFiltroCentrifugo = registroFiltroCentrifugo;
            _datosFiltroCentrifugo = datosFiltroCentrifugo;

        }
        public async Task<IActionResult> Index()
        {
            DetalleOperario user = await _function.ObtenerDatosOperario();

            //Respuesta<ReporteFiltro> detalleFiltro = await _datosFiltroCentrifugo.ObtenerReporteFiltro();

            Respuesta<List<DetalleFiltro>> detalleFiltro = await _datosFiltroCentrifugo.ObtenerDatosFiltroPorSitio();
            //Respuesta<List<EspecificacionFiltro>> detalleFiltro = await _datosFiltroCentrifugo.ObtenerDetallesPorSitio();
            detalleFiltro.Detalle = detalleFiltro.Detalle.OrderBy(x =>
            {
                DateTime fecha;
                return DateTime.TryParse(x.Fecha, out fecha) ? fecha : DateTime.MinValue;
            }).ToList();


            return View(detalleFiltro.Detalle);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosFiltro([FromBody] List<DetalleFiltro> detalles)
        {

            Respuesta<string> respuesta = await _registroFiltroCentrifugo.GuardarDatosFiltro(detalles);

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleFiltro([FromBody] List<EspecificacionFiltro> detalles)
        {
            Respuesta<string> respuesta = await _registroFiltroCentrifugo.GuardarDetalleFiltro(detalles);

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarReporteFiltro([FromBody] ReporteFiltro datos)
        {
            Respuesta<string> respuesta = await _registroFiltroCentrifugo.GuardarReporteFiltro(datos);

            return Json(new { respuesta = respuesta });
        }

        
    }
}
