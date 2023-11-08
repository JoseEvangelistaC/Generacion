
using Microsoft.AspNetCore.Mvc;
using System;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Application.LecturaCampo;
using Generacion.Models.LecturasCampo;
using Generacion.Models;
using Newtonsoft.Json;
using Generacion.Models.DatosConsola;
using Generacion.Models.Usuario;
using Generacion.Application.DataBase.cache;

namespace Generacion.Controllers
{
    public class LecturasCampoController : Controller
    {
        private readonly LecturaCampo _lecturaCampo;
        private readonly ILecturaCampo _datosRegistroCampo;
        private readonly CacheDatos _cacheDatos;
        
        public LecturasCampoController(LecturaCampo lecturaCampo, ILecturaCampo datosRegistroCampo, CacheDatos cacheDatos)
        {
            _cacheDatos = cacheDatos;
            _datosRegistroCampo = datosRegistroCampo;
            _lecturaCampo = lecturaCampo;
        }

        public async Task<IActionResult> Index()
        {
            DateTime fechaActual = DateTime.Now;
            DateTime fechaMedianoche = DateTime.Now;

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            string datoscabeceraJson = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(datoscabeceraJson);

            if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
            {
                fechaActual = fechaActual.AddDays(-1);
            }

            Respuesta<List<TiposRegistroCampo>> tipoRegistrosCampo = await _lecturaCampo.ObtenerTiposDeRegistro();
            var datosCampo = await _lecturaCampo.ObtenerDetalleCampo(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);

            Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));

            ViewData["horarioOperarios"] = horarioOperarios;

            ViewData["TipoRegistros"] = tipoRegistrosCampo.Detalle;
            ViewData["DatoscabeceraCampo"] = datoscabecera;
            ViewData["DatosCampo"] = datosCampo.Detalle;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosConsola([FromBody] List<DatosFormatoCampo> datos)
        {
            Respuesta<string> respuesta = await _datosRegistroCampo.GuardarDatosPrincipal(datos);
            return Json( new { respuesta = respuesta } );
        }
    }
}
