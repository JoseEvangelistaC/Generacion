using System.ComponentModel;

namespace Generacion.Models.DashBoard
{
    public class DetalleFiltro : DashboardDetalleFiltro
    {
        public decimal HorasTrabajadasFiltro { get; set; }
        public decimal HorasOperacionMantto { get; set; }
        public decimal HorasOpUltimoMantto { get; set; }
        public string Observaciones { get; set; }
    }

    public class DashboardDetalleFiltro
    {
        public string IdDetalleFiltro { get; set; }
        [DisplayName("Ingrese las horas de operacion.")]
        public decimal ProximaHoraCambio { get; set; }

        [DisplayName("Ingrese un número de generador del 0 al 2.")]
        public decimal NumeroGenerador { get; set; }
        [DisplayName("Ingrese una fecha.")]
        public string Fecha { get; set; }
        [DisplayName("Ingrese las horas de operacion.")]
        public decimal HorasOperacion { get; set; }
        [DisplayName("Ingrese el espesor.")]
        public decimal Espesor { get; set; }
        public string IdReporteFiltro { get; set; }
        [DisplayName("Seleccione un ejecutor.")]
        public string OperadorEjecutor { get; set; }
        public string ValidarPropiedadesNulasOVacias()
        {
            var mensajesError = string.Empty;
            var properties = GetType().GetProperties();

            foreach (var property in properties)
            {
                var valor = property.GetValue(this);

                if (valor == null || string.IsNullOrWhiteSpace(valor.ToString()))
                {
                    var displayName = property.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                               .Cast<DisplayNameAttribute>()
                                               .FirstOrDefault()?.DisplayName;

                    mensajesError = displayName;
                }
                else if (property.Name == "NumeroGenerador")
                {
                    // Validar el rango de NumeroGenerador
                    decimal numeroGenerador = (decimal)valor;

                    if (numeroGenerador < 1 || numeroGenerador > 2)
                    {
                        mensajesError = "Número de generador debe estar entre 1 y 2.";
                    }
                }
            }

            return mensajesError;
        }
    }

   
}
