
using Microsoft.AspNetCore.Mvc;
using System;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Application.LecturaCampo;
using Generacion.Models.LecturasCampo;
using Generacion.Models;
using Newtonsoft.Json;
using Generacion.Models.DatosConsola;
using Generacion.Application.DatosConsola.Query;

namespace Generacion.Controllers
{
    public class LecturasCampoController : Controller
    {
        private readonly LecturaCampo _lecturaCampo;
        private readonly ILecturaCampo _datosRegistroCampo;
        public LecturasCampoController(LecturaCampo lecturaCampo, ILecturaCampo datosRegistroCampo)
        {
            _datosRegistroCampo = datosRegistroCampo;
            _lecturaCampo = lecturaCampo;
        }

        public async Task<IActionResult> Index()
        {
            DateTime fechaActual = DateTime.Now;
            DateTime fechaMedianoche = DateTime.Now;

            if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
            {
                fechaActual = fechaActual.AddDays(-1);
            }

            Respuesta<List<TiposRegistroCampo>> tipoRegistrosCampo = await _lecturaCampo.ObtenerTiposDeRegistro();
            Respuesta<Dictionary<string, List<DatosFormatoCampo>>> datosRegistro = await _lecturaCampo.ObtenerTiposDeRegistro(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"));
           

            string usuarioDetail = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTablaCampo> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTablaCampo>>(usuarioDetail);


            ViewData["TipoRegistros"] = tipoRegistrosCampo.Detalle;
            ViewData["Titulos"] = datosRegistro.Detalle;

            ViewData["GasCombustible"] = datosRegistro.Detalle["SIST_GAS_COMBUSTIBLE"];
            ViewData["AguaRefrigMotor"] = datosRegistro.Detalle["SIST_AGUA_REFRIG_MOTOR"];
            ViewData["AguaRefrigLt"] = datosRegistro.Detalle["SIST_AGUA_REFRIG_LT"];
            ViewData["AguaRefrigHt"] = datosRegistro.Detalle["SIST_AGUA_REFRIG_HT"];
            ViewData["EntradaSalidaRad"] = datosRegistro.Detalle["ENTRADA_SALIDA_AGUA_RADIADORES"];
            ViewData["CompresorInst"] = datosRegistro.Detalle["COMPRESOR_INSTRUMENTACION"];
            ViewData["Compresor"] = datosRegistro.Detalle["COMPRESOR_AIRE_ARRANQUE"];
            ViewData["Generador"] = datosRegistro.Detalle["GENERADOR"];
            ViewData["Complementarios"] = datosRegistro.Detalle["COMPLEMENTARIOS"];
            ViewData["Inspecciones"] = datosRegistro.Detalle["INSPECCIONES"];
            ViewData["SistComunes"] = datosRegistro.Detalle["SIST_COMUNES"];
            ViewData["DatoscabeceraCampo"] = datoscabecera;



            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosCampo([FromBody] DatosFormatoCampo datos)
        {
            Respuesta<string> respuesta = await _datosRegistroCampo.GuardarDatosPrincipal(datos);
            return Json(new { respuesta = respuesta });
        }
    }
}

