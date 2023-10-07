using Generacion.Application.ION.Query;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Query;
using Microsoft.AspNetCore.Mvc;

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
            var datosMGD = await _consultarDatosMGD.ObtenerDatosMGD("2023/10/06");


            return View(datosMGD.Detalle);
        }
    }
}
