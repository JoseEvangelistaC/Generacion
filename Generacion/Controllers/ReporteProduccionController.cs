using Generacion.Application.ION.Query;
using Generacion.Models.ION;
using Generacion.Models;
using Microsoft.AspNetCore.Mvc;
using Generacion.Models.Usuario;
using Generacion.Application.Mantenimiento.Query;
using Generacion.Models.Mantenimiento;
using Generacion.Application.DataBase.cache;
using Newtonsoft.Json;
using Generacion.Application.ReporteProduccion;
using Generacion.Models.ReporteProduccion;
using Generacion.Application.DatosConsola.Query;
using Generacion.Models.DatosConsola;
using Generacion.Application.ReporteProduccion.Query;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Models.LecturasCampo;

namespace Generacion.Controllers
{
    public class ReporteProduccionController : Controller
    {
        private readonly ConsultarION _consultarION;
        private readonly CacheDatos _cacheDatos;
        private readonly DatosMantenimiento _datosMantenimiento;
        private readonly DatosConsola _datosConsola;
        private readonly IReporteProduccion _datosEnergiaProducida;
        private readonly ConsultarProduccion _consultarProduccion;
        private readonly LecturaCampo _lecturaCampo;
        private readonly ILogger<ReporteProduccionController> _logger;

        public ReporteProduccionController(ConsultarION consultarION, 
            DatosMantenimiento datosMantenimiento, CacheDatos cacheDatos, 
            IReporteProduccion datoEnergiaProducida, DatosConsola datosConsola, 
            ConsultarProduccion consultarProduccion, LecturaCampo lecturaCampo,
            ILogger<ReporteProduccionController> logger)
        {
            _lecturaCampo = lecturaCampo;
            _consultarProduccion = consultarProduccion;
            _datosConsola = datosConsola;
            _cacheDatos = cacheDatos;
            _datosMantenimiento = datosMantenimiento;
            _consultarION = consultarION;
            _datosEnergiaProducida = datoEnergiaProducida; 
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try { 
            Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));

            ViewData["horarioOperarios"] = horarioOperarios;
            DateTime fechaActual = DateTime.Now;
            DateTime fechaMedianoche = DateTime.Now;
            if (int.Parse(fechaActual.ToString("HH")) >= 0 && int.Parse(fechaActual.ToString("HH")) < 2)
            {
                fechaActual = fechaActual.AddDays(-1);
            }
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            string datoscabeceraJson = HttpContext.Session.GetString("datoscabecera");

            Respuesta<List<decimal>> datosRegistro = await _datosConsola.ObtenerDatosDetConsola(fechaActual.AddDays(-1).ToString("dd/MM/yyyy"), fechaActual.ToString("dd/MM/yyyy"),24);
            var datosSincro = await _consultarProduccion.ObtenerNumeroArranque($"LUREN-ELD-RPT-PROD_{fechaActual.AddDays(-1).ToString("yyyy-MM-dd")}");
            //Respuesta<List<DatosFormatoMGD>> respuesta = await _consultarION.ObtenerDatosION("09/01/23", "09/02/23"); //(fechaActual.ToString("dd/MM/yyyy"));
            string fechaFrontend = "09/09/2023";
            TimeSpan horaEspecifica = new TimeSpan(5, 0, 0);
            DateTime fechaHoraEspecifica = DateTime.Now;
            if (DateTime.TryParseExact(fechaFrontend, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaParseada))
            {
                fechaHoraEspecifica = fechaParseada.Date + horaEspecifica;
            }
            Respuesta<List<DatosFormatoMGD>> respuesta = await _consultarION.ObtenerDatosIONSQL("441", "12889,12890,12891,12892,12893,12894,537,540", fechaHoraEspecifica);
            //Respuesta<ReporteDiarioMantenimiento> horasGen = await _datosMantenimiento.ObtenerDatosMGD(fechaActual.ToString("dd/MM/yy"));
            Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionHoy = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24,fechaActual.ToString("dd/MM/yy"));
            Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> detalleCampoOperacionAyer = await _lecturaCampo.ObtenerDetalleCampoPorFecha("HrsOperacion,Operacion_ci,NivelCarter", 24, fechaActual.AddDays(-1).ToString("dd/MM/yy"));
            Respuesta<Dictionary<string, List<RegistrosDatosEngine>>> datosEngine = await _datosConsola.ObtenerDatosEngine(fechaActual.ToString("dd/MM/yyyy"), fechaMedianoche.ToString("dd/MM/yyyy"), user.IdSitio);

            Respuesta<List<EnergiaProducida>> datosProduccion = await _consultarProduccion.ObtenerRegistroProduccion(fechaActual.ToString("dd_MM_yyyy"));
            Respuesta<List<LevelLubeOilCartel>> datosProduccionCarter = await _consultarProduccion.ObtenerRegistroLevelCartel(fechaActual.ToString("dd_MM_yyyy"));
            Respuesta<List<CityGateFlow>> datosProduccionCity = await _consultarProduccion.ObtenerRegistroCityGate(fechaActual.ToString("dd_MM_yyyy"));
            Respuesta<List<TkCleanLube>> datosProduccionTkClean = await _consultarProduccion.ObtenerRegistroTkClean(fechaActual.ToString("dd_MM_yyyy"));
            Respuesta<Dictionary<string,ManttoVessel>> datosMantoVessel = await _consultarProduccion.ObtenerMantoTKVessel(fechaActual.ToString("dd_MM_yyyy"));
            Dictionary<string, CabecerasTabla> datoscabecera = JsonConvert.DeserializeObject<Dictionary<string, CabecerasTabla>>(datoscabeceraJson);
            

             ViewData["DatoscabeceraCampo"] = datoscabecera;
            ViewData["Produccion"] = datosProduccion.Detalle;
            ViewData["ProduccionCarter"] = datosProduccionCarter.Detalle;
            ViewData["ProduccionCity"] = datosProduccionCity.Detalle;
            ViewData["ProduccionTkClean"] = datosProduccionTkClean.Detalle;
            ViewData["datosMantoVessel"] = datosMantoVessel.Detalle;
            
            ViewData["DetalleOperacionCampoHoy"] = detalleCampoOperacionHoy.Detalle;
            ViewData["DetalleOperacionCampoAyer"] = detalleCampoOperacionAyer.Detalle;


            ViewData["DatosGraficoION"] = respuesta.Detalle
                .OrderBy(x => x.Hora == "00:00" ? TimeSpan.FromHours(24) : TimeSpan.Parse(x.Hora))
                .ToList();

            ViewData["DatosSincro"] = datosSincro.Detalle;
            ViewData["datosEngine"] = datosEngine.Detalle; 
            ViewData["datosBFA901"] = datosRegistro.Detalle;

            }
            catch (Exception ex)
            {
                _logger.LogError("ReporteProduccionController  Error : {ErrorDetails}", ex.ToString());
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosEnergiaProducida([FromBody] EnergiaProducida datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosEnergiaProducida(datos);
            return Json(new { respuesta = respuesta } );
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleProduccion([FromBody] Produccion datos)
        {
            Respuesta<string> respuesta= null;
            if (datos != null)
            {
                int respuestaProduccion = await _datosEnergiaProducida.InsertOrUpdateProduccionStatus(datos.reporteProduccionStatus);
                var respuestaDetalle = await _datosEnergiaProducida.InsertOrUpdateDetalleProduccion(datos.detalleProduccion);
                var respuestaSincro = await _datosEnergiaProducida.InsertOrUpdateArranqueSincro(datos.arranqueSincronizacion);
                respuesta = await _datosEnergiaProducida.InsertOrUpdateRegistroEventos(datos.registroEventos);
            }

            return Json(new{ respuesta = respuesta });
        }
        [HttpPost]
        public async Task<JsonResult> GuardarDatosLevelOilCarter([FromBody] LevelLubeOilCartel datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosLevelOilCarter(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosRefCarter([FromBody] RefrescamientoCartel datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosRefCarter(datos);
            return Json( new { respuesta = respuesta });
        }
        // CREACION DE CITY GATE 

        [HttpPost]
        public async Task<JsonResult> GuardarDatosCityGate([FromBody] CityGateFlow datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosCityGate(datos);
            return Json(new { respuesta = respuesta });
        }


        [HttpPost]
        public async Task<JsonResult> GuardarDatosManttoVessel([FromBody] ManttoVessel datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosManttoVessel(datos);
            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosTkCleanLube([FromBody] TkCleanLube datos)
        {
            Respuesta<string> respuesta = await _datosEnergiaProducida.GuardarDatosTkCleanLube(datos);
            return Json(new { respuesta = respuesta });
        }
    }

    public class Produccion
    {
        public List<ArranqueSincronizacion> arranqueSincronizacion { get; set; }
        public List<ReporteProduccionStatus> reporteProduccionStatus { get; set; }
        public List<RegistroEventos> registroEventos { get; set; }
        public List<DetalleProduccion> detalleProduccion { get; set; }
    }
}
