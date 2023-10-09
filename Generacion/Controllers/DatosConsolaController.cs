using Generacion.Application.DatosConsola.Query;
using Generacion.Models.DatosConsola;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Application.DatosConsola;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Generacion.Controllers
{
    public class DatosConsolaController : Controller
    {
        private readonly DatosConsola _datosConsola;
        private readonly IDatosRegistroConsola _datrosconsolaRegistro;
        public DatosConsolaController(DatosConsola datosConsola, IDatosRegistroConsola datosRegistroConsola)
        {
            _datrosconsolaRegistro = datosRegistroConsola;
            _datosConsola = datosConsola;
        }
        public async Task<IActionResult> Index()
        {
            DateTime fechaActual = DateTime.Now;
            DateTime fechaMedianoche = DateTime.Now;
      
            if (int.Parse(fechaActual.ToString("HH")) >=0 && int.Parse(fechaActual.ToString("HH")) < 2)
            {
                fechaActual = fechaActual.AddDays(-1);
            }

            Respuesta<List<TiposDeRegistroConsola>> tipoRegistros = await _datosConsola.ObtenerTiposDeRegistro();
            Respuesta<Dictionary<string, List<DatosFormatoConsola>>> datosRegistro = await _datosConsola.ObtenerRegistroDeConsola(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"));
            Respuesta<List<RegistrosDatosGenerator>> datosGenerador = await _datosConsola.ObtenerDetalleGenerador(fechaActual.ToString("dd/MM/yyyy"));
            Respuesta<List<RegistrosDatosEngine>> datosEngine = await _datosConsola.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yyyy"));
            Dictionary<string, LecturasMedianoche> datosLecturas = await _datosConsola.ObtenerLecturaMediaNoche("ELD-CTL-OM002_"+ fechaActual.ToString("yyyy-MM-dd"));

            string usuarioDetail = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(usuarioDetail);



            ViewData["datosLecturas"] = datosLecturas; 
            ViewData["EnergiaGenerada1"] = datosRegistro.Detalle["EG01"];
            ViewData["EnergiaGeneradaBAA"] = datosRegistro.Detalle["BAA901"];
            ViewData["TipoRegistros"] = tipoRegistros.Detalle;
            ViewData["Datoscabecera"] = datoscabecera;
                ViewData["DatosGenerador"] = datosGenerador.Detalle;
                ViewData["DatosEng"] = datosEngine.Detalle;

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosConsola([FromBody] DatosFormatoConsola datos)
        {
            Respuesta<string> respuesta = await _datrosconsolaRegistro.GuardarDatosEG(datos);
            return Json(new { respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosGenerador([FromBody] RegistroDatosGenerator datos)
        {
            Respuesta<string> respuesta = await _datrosconsolaRegistro.GuardarDatosGenerador(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosEngine([FromBody] RegistroDetalleEngine datos)
        {
            Respuesta<string> respuesta = await _datrosconsolaRegistro.GuardarDatosEngine(datos);
            return Json(respuesta = respuesta);
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosLectMedianoche([FromBody] LecturasMedianoche datos)
        {
            Respuesta<string> respuesta = await _datrosconsolaRegistro.GuardarLectMedianoche(datos);
            return Json(respuesta = respuesta);
        }
        
    }
}
