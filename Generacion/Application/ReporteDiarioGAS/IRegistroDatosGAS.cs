using Generacion.Models;
using Generacion.Models.ReporteDiarioGAS;

namespace Generacion.Application.ReporteDiarioGAS
{
    public interface IRegistroDatosGAS
    {
        Task<Respuesta<string>> GuardarDetalle(List<DetalleReporteGas> datos);

    }
}
