using Generacion.Application.Funciones;
using Generacion.Application.Mantenimiento;
using Generacion.Models.Mantenimiento;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Generacion.Controllers
{
    public class RegMantenimientoController : Controller
    {
        private readonly IMantenimiento _mantenimiento;
        private readonly FotoServidor _fotoServidor;
        public RegMantenimientoController(IMantenimiento mantenimiento , FotoServidor fotoServidor)
        {
            _mantenimiento = mantenimiento;
            _fotoServidor = fotoServidor;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] DatosAEnviar datosAEnviar)
        {
            _mantenimiento.GuardarDatosMotoGenerador(datosAEnviar.motoGeneradores);
            _mantenimiento.GuardarDatosServ(datosAEnviar.mantenimientoServicios);
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
        public List<MotoGenerador> motoGeneradores { get; set; }
        public List<MantenimientoServicios> mantenimientoServicios { get; set; }
    }
}
