using Generacion.Application.ION.Query;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Query;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Generacion.Controllers
{
    public class MGDController : Controller
    {
        private readonly ConsultarDatosMGD _consultarDatosMGD;

        public MGDController(ConsultarDatosMGD consultarDatosMGD)
        {
            _consultarDatosMGD = consultarDatosMGD;
        }
        public async Task<IActionResult> Index()
        {
            DateTime dateTime = DateTime.Now;
            var datosMGD = await _consultarDatosMGD.ObtenerDatosMGD(dateTime.ToString("yyyy/MM/dd"));

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            
            ViewData["DetalleOperario"] = detalleOperario;

            return View(datosMGD.Detalle);
        }
    }
}
