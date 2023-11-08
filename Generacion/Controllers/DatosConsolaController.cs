using Generacion.Application.DatosConsola.Query;
using Generacion.Models.DatosConsola;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Application.DatosConsola;
using System.Collections.Generic;
using Newtonsoft.Json;
using Generacion.Models.Usuario;
using Generacion.Application.DataBase.cache;

namespace Generacion.Controllers
{
    public class DatosConsolaController : Controller
    {
        private readonly DatosConsola _datosConsola;
        private readonly IDatosRegistroConsola _datosconsolaRegistro;
        private readonly CacheDatos _cacheDatos;
        
        public DatosConsolaController(CacheDatos cacheDatos, DatosConsola datosConsola, IDatosRegistroConsola datosRegistroConsola)
        {
            _cacheDatos = cacheDatos;
            _datosconsolaRegistro = datosRegistroConsola;
            _datosConsola = datosConsola;
        }
        public async Task<IActionResult> Index()
        {
            DateTime fechaActual = DateTime.Now;
            DateTime fechaMedianoche = DateTime.Now;

            if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
            {
                fechaActual = fechaActual.AddDays(-1);
            }
            else
            {
                fechaMedianoche = fechaMedianoche.AddDays(+1);
            }

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            Respuesta<List<TiposDeRegistroConsola>> tipoRegistros = await _datosConsola.ObtenerTiposDeRegistro();
            Respuesta<Dictionary<string, List<DatosFormatoConsola>>> datosRegistro = await _datosConsola.ObtenerRegistroDeConsola(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);
            Respuesta<Dictionary<string, List<RegistrosDatosGenerator>>> datosGenerador = await _datosConsola.ObtenerDetalleGenerador(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);
            Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> datosEngine = await _datosConsola.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yy"), fechaMedianoche.ToString("dd/MM/yy"), user.IdSitio);
            Dictionary<string, LecturasMedianoche> datosLecturas = await _datosConsola.ObtenerLecturaMediaNoche($"{user.IdSitio}-ELD-CTL-OM002_{fechaActual.ToString("yyyy-MM-dd")}");
            Respuesta<List<OutGoingFeeder>> datosDelived = await _datosConsola.ObtenerDetalleBAO("DELIVERED", fechaActual.ToString("yyyy-MM-dd"));
            Respuesta<List<OutGoingFeeder>> datosReceived = await _datosConsola.ObtenerDetalleBAO("RECIVED", fechaActual.ToString("yyyy-MM-dd"));
            Respuesta<FormatoConsola> datoFormato = await _datosConsola.ObtenerFormatoConsola($"{user.IdSitio}-ELD-CTL-OM002_{fechaActual.ToString("yyyy-MM-dd")}");

            string cabecera = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(cabecera);

            Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));

            ViewData["horarioOperarios"] = horarioOperarios;

            ViewData["datoFormato"] = datoFormato.Detalle;
            ViewData["datosDelived"] = datosDelived.Detalle;
            ViewData["datosReceived"] = datosReceived.Detalle;
            ViewData["datosLecturas"] = datosLecturas;
            ViewData["EnergiaGenerada"] = datosRegistro.Detalle;
            ViewData["TipoRegistros"] = tipoRegistros.Detalle;
            ViewData["Datoscabecera"] = datoscabecera;
            ViewData["DatosGenerador"] = datosGenerador.Detalle;
            ViewData["DatosEng"] = datosEngine.Detalle;

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosConsola([FromBody] List<DatosFormatoConsola> datos)
        {
            Respuesta<string> respuesta = await _datosconsolaRegistro.GuardarDatosEG(datos);
            return Json(new { respuesta = respuesta } );
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosGenerador([FromBody] List<RegistroDatosGenerator> datos)
        {
            Respuesta<string> respuesta = await _datosconsolaRegistro.GuardarDatosGenerador(datos);
            return Json(new { respuesta = respuesta } );
           
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosEngine([FromBody] List<RegistroDetalleEngine> datos)
        {
            Respuesta<string> respuesta = await _datosconsolaRegistro.GuardarDatosEngine(datos);
            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosLectMedianoche([FromBody] LecturasMedianoche datos)
        {
            Respuesta<string> respuesta = await _datosconsolaRegistro.GuardarLectMedianoche(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleBAO([FromBody] List<OutGoingFeeder> datos)
        {
            Respuesta<string> respuesta = await _datosconsolaRegistro.GuardarDetallesBAO(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarFormato([FromBody] FormatoConsola datos)
        {
            Respuesta<string> respuesta = await _datosconsolaRegistro.GuardarDatoFormato(datos);
            return Json(new { respuesta = respuesta });
        }

    }
}
