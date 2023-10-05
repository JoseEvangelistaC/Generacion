namespace Generacion.Models.DatosConsola
{
    public class OutGoingFeeder
    { 
        public string IdOutGoing { get; set; }
        public string IdTipoEngine { get; set; }
        public decimal kWh { get; set; }
        public decimal kVARh { get; set; }
        public string Hora { get; set; }
        public string Fecha { get; set; }
        public string IdFormatoConsola { get; set; }
        public int Fila { get; set; }
    }
}
