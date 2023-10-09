
using Microsoft.AspNetCore.Mvc;
using System;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Application.LecturaCampo;
using Generacion.Models.LecturasCampo;
using Generacion.Models;
using Newtonsoft.Json;
using Generacion.Models.DatosConsola;

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
            //Respuesta<List<RegistrosDatosGenerator>> datosGenerador = await _lecturaCampo.ObtenerDetalleGenerador(fechaActual.ToString("dd/MM/yyyy"));
            //Respuesta<List<RegistrosDatosEngine>> datosEngine = await _lecturaCampo.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yyyy"));
            //Dictionary<string, LecturasMedianoche> datosLecturas = await _lecturaCampo.ObtenerLecturaMediaNoche("ELD-CTL-OM002_" + fechaActual.ToString("yyyy-MM-dd"));

            string usuarioDetail = HttpContext.Session.GetString("datoscabecera");
            Dictionary<string, CabecerasTablaCampo> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTablaCampo>>(usuarioDetail);


            ViewData["TipoRegistros"] = tipoRegistrosCampo.Detalle;
            //ViewData["datosLecturas"] = datosLecturas;
            ViewData["GasCombustible"] = datosRegistro.Detalle["SIST_GAS_COMBUSTIBLE"];
            ViewData["AguaRefrigMotor"] = datosRegistro.Detalle["SIST_AGUA_REFRIG_MOTOR"];
            ViewData["AguaRefrigLt"] = datosRegistro.Detalle["SIST_AGUA_REFRIG_LT"];
            ViewData["AguaRefrigHt"] = datosRegistro.Detalle["SIST_AGUA_REFRIG_HT"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["ENTRADA_SALIDA_AGUA_RADIADORES"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["COMPRESOR_INSTRUMENTACION"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["COMPRESOR_AIRE_ARRANQUE"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["GENERADOR"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["COMPLEMENTARIOS"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["INSPECCIONES"];
            ViewData["GasCombustible"] = datosRegistro.Detalle["SIST_COMUNES"];
            ViewData["DatoscabeceraCampo"] = datoscabecera;



            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosConsola([FromBody] DatosFormatoCampo datos)
        {
            Respuesta<string> respuesta = await _datosRegistroCampo.GuardarDatosPrincipal(datos);
            return Json(new { respuesta = respuesta });
        }
    }
}
