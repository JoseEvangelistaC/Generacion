using Generacion.Application.Usuario;
using Generacion.Application.Usuario.Query;
using Generacion.Models;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Generacion.Controllers
{
    public class HistorialController : Controller
    {
        private readonly IUsuario _usuario;
        private readonly ConsultarUsuario _consultarUsuario;
        public HistorialController( IUsuario usuario , ConsultarUsuario consultarUsuario)
        {
            _usuario = usuario;
            _consultarUsuario = consultarUsuario;
        }
        public async Task<IActionResult> Index()
        {
            Respuesta<List<HistorialUsuario>> historialOperario =  await _consultarUsuario.ObtenerDatosHistorial("op-47789733");
            //historialOperario.Detalle = historialOperario.Detalle.OrderBy(h => TimeSpan.Parse(h.Hora)).ToList();

            return View(historialOperario);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarHistorial([FromBody] List<HistorialUsuario> historialUsuarios)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            Respuesta<List<HistorialUsuario>> respuesta = await _usuario.GuardarHistorial(historialUsuarios,detalleOperario.IdOperario);

            return Json(new { respuesta = respuesta });
        }

     /*   [HttpPost]
        public async Task<JsonResult> EliminarHistorial([FromBody] string id)
        {
           // Respuesta<List<HistorialUsuario>> respuesta = await _usuario.EliminarDatosHistorial(id);

           // return Json(new { respuesta = respuesta });
        }*/
    }
}
