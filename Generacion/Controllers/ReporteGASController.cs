using Generacion.Application.ReporteDiarioGAS;
using Generacion.Application.ReporteDiarioGAS.Query;
using Generacion.Application.ReporteGAS;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ReporteDiarioGAS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Generacion.Controllers
{
    public class ReporteGASController : Controller
    {
        public readonly IRegistroDatosGAS _registroDatosGAS;
        private readonly ObtenerDatosReporteGAS _obtenerDatosReporteGAS;
        public ReporteGASController(IRegistroDatosGAS registroDatosGAS, ObtenerDatosReporteGAS obtenerDatosReporteGAS)
        {
            _registroDatosGAS = registroDatosGAS;
            _obtenerDatosReporteGAS = obtenerDatosReporteGAS;
        }
        public async Task<IActionResult> Index()
        {
            // Obtener datos de la sesión
            string usuarioDetail = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(usuarioDetail);
            ViewData["Datoscabecera"] = datoscabecera;



            Respuesta<Dictionary<string, List<DetalleReporteGas>>> detalleReporte = await _obtenerDatosReporteGAS.ObtenerDetallesReporte("ELD.CTL-OM-003_01_10_2023");

            var idReporteGas = await _obtenerDatosReporteGAS.ObtenerIdReporteGAS();

            ViewData["detalleReporte"] = detalleReporte.Detalle;
            ViewData["idReporteGas"] = idReporteGas.Detalle;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GuardarDetalle([FromBody] List<DetalleReporteGas> datos )
        {
            Respuesta<string> respuesta = await _registroDatosGAS.GuardarDetalle(datos);

            return Json(new { respuesta = respuesta });
        }
    }
}
