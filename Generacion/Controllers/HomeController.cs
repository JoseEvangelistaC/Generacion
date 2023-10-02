using Generacion.Application.Usuario;
using Generacion.Models;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace Generacion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuario _usuario;

        public HomeController(ILogger<HomeController> logger, IUsuario usuario)
        {
            _logger = logger;
            _usuario = usuario;
        }

        public async Task<IActionResult> Index()
        {
            Respuesta<DetalleOperario> datos = await _usuario.ObtenerDatosOperario("jevangelista");

            if (datos.IdRespuesta ==0)
            {
                HttpContext.Session.SetString("usuarioDetail", JsonConvert.SerializeObject(datos.Detalle));

                // Obtener datos de la sesión
                string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
                DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            }
            ViewData["DetalleOperario"] = datos.Detalle;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}