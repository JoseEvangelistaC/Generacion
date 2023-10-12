using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.LecturasCampo;


namespace Generacion.Application.LecturaCampo
{
    public interface ILecturaCampo
    {
        Task<Respuesta<string>> GuardarDatosPrincipal(DatosFormatoCampo datosFormatoCampo);

        Task<Respuesta<string>> GuardarDatoFormato(FormatoConsola datos);
    }
}
