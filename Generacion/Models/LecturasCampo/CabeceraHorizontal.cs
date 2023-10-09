using Generacion.Models.DatosConsola;
using Generacion.Models.Usuario;
using System.Security.Principal;

namespace Generacion.Models.LecturasCampo
{

    public class CabeceraHorizontal
    {
        public string Hora { get; set; }
        public string Und { get; set; }
        public string Rango { get; set; }
        public int P_Activa { get; set; }
        public int H_Operacion { get; set; }
        public int Temp_Ambiente { get; set; }
        public int Humed_Relativa { get; set; }

    }

    public class DatosFormatoCampo: CabeceraHorizontal
    {

        public string IdDetalleInicioCampo { get; set; }
        public string Fecha { get; set; }
        public string IdRegistroConsola { get; set; }
        public string IdOperario { get; set; }
        public string IdformatoConsola { get; set; }
        public int Fila { get; set; }
    }
}
