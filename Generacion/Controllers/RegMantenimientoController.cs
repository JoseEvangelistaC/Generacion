using Generacion.Application.DataBase.cache;
using Generacion.Application.Funciones;
using Generacion.Application.Mantenimiento;
using Generacion.Models.Mantenimiento;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Generacion.Controllers
{
    public class RegMantenimientoController : Controller
    {
        private readonly IMantenimiento _mantenimiento;
        private readonly FotoServidor _fotoServidor;
        private readonly CacheDatos _cacheDatos;

        public RegMantenimientoController(IMantenimiento mantenimiento , FotoServidor fotoServidor, CacheDatos cacheDatos)
        {
            _mantenimiento = mantenimiento;
            _fotoServidor = fotoServidor;
            _cacheDatos = cacheDatos;
        }
        public async Task<IActionResult> Index()
        {
            Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));

            ViewData["horarioOperarios"] = horarioOperarios;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] DatosAEnviar datosAEnviar)
        {
            _mantenimiento.GuardarDatosMotoGenerador(datosAEnviar.MotoGeneradores);
            _mantenimiento.GuardarDatosServ(datosAEnviar.MantenimientoServicios);
            _mantenimiento.GuardarDatosAceiteCarter(datosAEnviar.CilindroAceiteCarter);
            _mantenimiento.GuardarExpansionVessel(datosAEnviar.ListaExpansionVersel);
            _mantenimiento.GuardarReporteDiario(datosAEnviar.reporteDiarioMantenimiento);

            Dictionary<int, List<string>> horarioOperarios = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(_cacheDatos.ObtenerContenidoCache("HorarioOperario"));

            ViewData["horarioOperarios"] = horarioOperarios;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GuardarImagen([FromBody] List<ImagenModel> imagenesBase64)
        {
            try
            {
                int indexImagen = 0;
                foreach (ImagenModel imagenBase64 in imagenesBase64)
                {
                    indexImagen++;
                    string base64Data = Regex.Match(imagenBase64.base64, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;

                    byte[] imageBytes = Convert.FromBase64String(base64Data);

                    _fotoServidor.GuardarImagenes(imageBytes, indexImagen, imagenBase64.casillaid);
                }

                return Json(new { mensaje = "Imágenes recibidas correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al recibir las imágenes: " + ex.Message });
            }
        }
    }

    public class DatosAEnviar
    {
        public List<MotoGenerador> MotoGeneradores { get; set; }
        public List<ExpansionVersel> ListaExpansionVersel { get; set; }
        public List<MantenimientoServicios> MantenimientoServicios { get; set; }
        public List<CilindroAceiteCarter> CilindroAceiteCarter { get; set; }
        public ReporteDiarioMantenimiento reporteDiarioMantenimiento { get; set; }
        

    }
}
