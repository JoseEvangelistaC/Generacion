namespace Generacion.Views.ControlGAS
{
    public class ContratoGas
    {
        public string IdContratoGas { get; set; }
        public decimal ConsumoContrato { get; set; }
        public string Fecha { get; set; }
        public int Activo { get; set; }
        public string IdOperario { get; set; }
    }
    public class ConsumoGas
    {
        public string IdConsumoGas { get; set; }
        public string Fecha { get; set; }
        public int DiasMes { get; set; }
        public decimal ConsumoDelMes { get; set; }
        public string IdContratoGas { get; set; }
        public string IdOperario { get; set; }
    }
    public class DetalleConsumoGas
    {
        public string IdDetalleConsumoGas { get; set; }
        public string Fecha { get; set; }
        public decimal ConsumoTotalActual { get; set; }
        public decimal ConsumoDiario { get; set; }
        public string IdOperario { get; set; }
        public string IdConsumoGas { get; set; }
    }

}
