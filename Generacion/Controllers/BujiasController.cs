using Generacion.Application.Almacen.Query;
using Generacion.Models.Almacen.Bujias;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Models.Almacen;
using Generacion.Application.Almacen;
using Generacion.Models.Usuario;
using Newtonsoft.Json;
using Generacion.Application.Bujias;
using Generacion.Models.DatosConsola;
using Generacion.Application.Bujias.Query;

namespace Generacion.Controllers
{
    public class BujiasController : Controller
    {
        private readonly ConsultasAlmacen _consultasAlmacen;
        private readonly IAlmacen _almacen; 
            
        private readonly IBujias _bujias;
        private readonly ConsultaBujias _consultaBujias;
        public BujiasController(ConsultasAlmacen consultasAlmacen, IAlmacen almacen, IBujias bujias, ConsultaBujias consultaBujias)
        {
            _consultaBujias = consultaBujias;
            _bujias = bujias;
            _almacen = almacen;
            _consultasAlmacen = consultasAlmacen;
        }
        public async Task<IActionResult> Index()
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            Respuesta<List<ComponenteBujias>> detalleBujias = await _consultasAlmacen.ObtenerDatosAlmacenBujias("Bujias251023", user.IdSitio);
            Respuesta<List<PrestamoComponente>> detalleBujiasPrestadas = await _consultasAlmacen.ObtenerDatosPrestados("Bujias251023", user.IdSitio);
            
            ViewData["DetalleBujiasPrestadas"] = detalleBujiasPrestadas.Detalle; //List<PrestamoComponente>

            return View(detalleBujias);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosPrestamo([FromBody] Prestamo datosPrestamo)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            Respuesta<string> respuesta = await _almacen.GuardarDatosPrestados(datosPrestamo);
            
            return Json(new { respuesta = respuesta });
        }

        public async Task<IActionResult> ControlCambio()
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            string cabecera = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabecera);

            var datosControlCambio = await _consultaBujias.ObtenerControlCambioPorLado(user.IdSitio);
            var datosControl = await _consultaBujias.ObtenerControlCambio(user.IdSitio);

            Dictionary<int,DetalleRegistroBujias> diccinarioEG = datosControl.Detalle.ToDictionary(x => x.Numerogenerador);

            ViewData["Datoscabecera"] = datoscabecera;
            ViewData["diccinarioEG"] = diccinarioEG; //Dictionary<int,DetalleRegistroBujias>


            return View(datosControlCambio.Detalle);
        }
        public async Task<IActionResult> RegistroControlCambio()
        {
            string cabecera = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabecera);

            ViewData["Datoscabecera"] = datoscabecera;

            return View();
        }


        [HttpGet]
        public async Task<JsonResult> ObtenerControlCambio([FromHeader]string lado, [FromHeader] int fila, [FromHeader] int EG)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            Respuesta<List<RegistroBujias>> respuesta = await _consultaBujias.ObtenerdetalleControlCambio(lado, fila, EG,user.IdSitio);


            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleDatosBujias([FromBody] List<RegistroBujias> datos)
        {
            Respuesta<string> respuestaRegistro = await _bujias.GuardarOActualizarRegisto(datos);
            return Json(new { respuesta = respuestaRegistro });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosBujias([FromBody] ControlCambioData datos)
        {
            Respuesta<string> respuestaControlCambio = await _bujias.GuardarOActualizarControlCambio(datos);

            return Json(new { respuesta = respuestaControlCambio });
        }
    }
}
