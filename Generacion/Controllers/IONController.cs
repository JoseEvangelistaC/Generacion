using Generacion.Application.ION.Query;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Query;
using Generacion.Models.MGD;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Generacion.Controllers
{
    public class IONController : Controller
    {
        private readonly ConsultarION _consultarION; 
        private readonly IDatosMGD _datosMGD;

        public IONController(ConsultarION consultarION,IDatosMGD datosMGD)
        {
            _consultarION = consultarION;
            _datosMGD = datosMGD;
        }
        public async Task<IActionResult> Index()
        {
            string idSitio = "LUREN";
            DateTime fechaActual = DateTime.Now;
            var respuesta = await _consultarION.ObtenerDatosION("1/06/2023"); //(fechaActual.ToString("dd/MM/yyyy"));

            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario detalleOperario = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

            DatosMGD datosMGD = new DatosMGD()
            {
                Fecha = fechaActual.ToString("yyyy-MM-dd"),
                IdOperario = detalleOperario.IdOperario,
                IdReporteMGD = $"MGD-{idSitio}-{fechaActual.ToString("dd_MM_yyyy")}",
                Idsitio= idSitio,
                Revenue = respuesta.Detalle
            };
            var respuestaMGD = await _datosMGD.GuardarDatosConsultaMGD(datosMGD);
            return View(respuesta.Detalle);
        }

    }
}
