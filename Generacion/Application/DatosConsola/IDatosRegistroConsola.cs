
using Generacion.Models;
using Generacion.Models.DatosConsola;

namespace Generacion.Application.DatosConsola
{
    public interface IDatosRegistroConsola
    {
        Task<Respuesta<string>> GuardarDatosEG(DatosFormatoConsola datosConsola);
        Task<Respuesta<string>> GuardarDatosGenerador(RegistroDatosGenerator datosConsola);
        Task<Respuesta<string>> GuardarDatosEngine(RegistroDetalleEngine datosConsola);
        Task<Respuesta<string>> GuardarLectMedianoche(LecturasMedianoche lecturasMedianoche);

    }
}
