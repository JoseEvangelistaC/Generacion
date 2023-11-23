using MediatR;

namespace Generacion.Models.Aceite
{
    public class ControlCambioAceite
    {
        public string IdControlCambioAceite { get; set; }
        public decimal HorasCambio { get; set; }
        public int NumeroGenerador { get; set; }
        public string Tipo { get; set; }
        public string FechaCambio { get; set; }
    }

}
