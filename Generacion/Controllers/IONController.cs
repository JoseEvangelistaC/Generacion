using Generacion.Application.ION;
using Generacion.Application.ION.Query;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Query;
using Generacion.Models.ION;
using Generacion.Models;
using Generacion.Models.MGD;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace Generacion.Controllers
{
    public class IONController : Controller
    {
        private readonly ConsultarION _consultarION; 
        private readonly IDatosMGD _datosMGD;
        private readonly IDatosION _datosION;
        
        public IONController(ConsultarION consultarION,IDatosMGD datosMGD, IDatosION datosION)
        {
            _consultarION = consultarION;
            _datosMGD = datosMGD;
            _datosION = datosION;
        }
        public async Task<IActionResult> Index()
        {
            string idSitio = "LUREN";
            DateTime fechaActual = DateTime.Now;
            // Respuesta<List<DatosFormatoMGD>> respuesta = await _consultarION.ObtenerDatosION("31/05/23", "1/06/23");

            string fechaFrontend = "09/09/2023";
            TimeSpan horaEspecifica = new TimeSpan(5, 0, 0);
            DateTime fechaHoraEspecifica = DateTime.Now;
            if (DateTime.TryParseExact(fechaFrontend, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaParseada))
            {
                fechaHoraEspecifica = fechaParseada.Date + horaEspecifica;
            }

            Respuesta<List<DatosFormatoMGD>> respuesta = await _consultarION.ObtenerDatosIONSQL("441", "12889,12890,12891,12892,12893,12894,537,540", fechaHoraEspecifica);

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            if (respuesta != null && respuesta.Detalle.Count > 0)
            {
                DatosMGD datosMGD = new DatosMGD()
                {
                    Fecha = fechaActual.ToString("yyyy-MM-dd"),
                    IdOperario = detalleOperario.IdOperario,
                    IdReporteMGD = $"MGD-{idSitio}-{fechaActual.ToString("dd_MM_yyyy")}",
                    Idsitio = idSitio,
                    Revenue = respuesta.Detalle
                };
                var respuestaMGD = await _datosMGD.GuardarDatosConsultaMGD(datosMGD);

                var guardarDatosION = await _datosION.GuardarDatosION(datosMGD);
                respuesta.Detalle = respuesta.Detalle.OrderBy(x => DateTime.Parse(x.Date_Time)).ToList();
            }
           

            return View(respuesta.Detalle);
        }

    }
}
