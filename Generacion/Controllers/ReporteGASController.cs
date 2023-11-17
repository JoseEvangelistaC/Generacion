using Generacion.Application.ReporteDiarioGAS;
using Generacion.Application.ReporteDiarioGAS.Query;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ReporteDiarioGAS;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MReporteGAS = Generacion.Models.ReporteDiarioGAS.ReporteGAS;

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
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            DateTime dateTime   = DateTime.Now;
            // Obtener datos de la sesión
            string cabeceras = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabeceras);
            ViewData["Datoscabecera"] = datoscabecera;

            var idReporteGas = await _obtenerDatosReporteGAS.ObtenerIdReporteGAS(user.IdSitio);

            Respuesta<Dictionary<string, List<DetalleReporteGas>>> detalleReporte = await _obtenerDatosReporteGAS.ObtenerDetallesReporte($"{user.IdSitio}-ELD.CTL-OM-003_{dateTime.ToString("dd_MM_yyyy")}");

            ViewData["detalleReporte"] = detalleReporte.Detalle;
            ViewData["idReporteGas"] = idReporteGas.Detalle ?? new MReporteGAS();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GuardarDetalle([FromBody] List<DetalleReporteGas> datos )
        {
            Respuesta<string> respuesta = await _registroDatosGAS.GuardarDetalle(datos);

            DateTime dateTime = DateTime.Now;

            MReporteGAS reporteGAS = new MReporteGAS()
            {
                IdReporteGas = datos[0].IdReporteGas,
                Activo = datos.Count >= 7 ? 1 :0,
                Fecha = dateTime.ToString("dd/MM/yyyy")
            };

                _registroDatosGAS.guardarIdReporte(reporteGAS);

            return Json(new { respuesta = respuesta });
        }
    }
}
