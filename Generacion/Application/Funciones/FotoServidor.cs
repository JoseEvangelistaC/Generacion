namespace Generacion.Application.Funciones
{
    public class FotoServidor
    {
        public async Task<string> GuardarImagenes(byte[] imagenes,int indexImagen, string casilllaId)
        {
            DateTime fecha = DateTime.Now;
            var rutaGuardar = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"Fotos-Reporte-Mantenimiento_ {fecha.ToString("ddMMyyyy")}");
            var nombreArchivo = $"{fecha.ToString("dd-MM-yyyy")}_{indexImagen}_{casilllaId}.jpg "  ;
            if (!Directory.Exists(rutaGuardar))
            {
                Directory.CreateDirectory(rutaGuardar);
            }
            using (FileStream fs = new FileStream(Path.Combine(rutaGuardar, nombreArchivo), FileMode.Create))
            {
                    fs.Write(imagenes, 0, imagenes.Length);
            }
            return "Ok";
        }
    }
}
