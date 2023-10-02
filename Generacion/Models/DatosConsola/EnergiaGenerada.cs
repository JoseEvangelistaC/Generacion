namespace Generacion.Models.DatosConsola
{
    public class EnergiaGenerada
    {
        public string Hora { get; set; }
        /// <summary>
        /// Codigo : P
        /// </summary>
        public int PotenciaActiva { get; set; }
        /// <summary>
        /// Codigo : Q
        /// </summary>
        public int PotenciaReactiva { get; set; }
        /// <summary>
        /// Codigo : E+
        /// </summary>
        public int EnergiaActiva { get; set; }
        /// <summary>
        /// Codigo : Eq+
        /// </summary>
        public int EnergiaReactiva { get; set; }
        /// <summary>
        /// Codigo : lL1
        /// </summary>
        public int CorrienteLinea1 { get; set; }

        /// <summary>
        /// Codigo : lL2
        /// </summary>
        public int CorrienteLinea2 { get; set; }

        /// <summary>
        /// Codigo : lL3
        /// </summary>
        public int CorrienteLinea3 { get; set; }

        /// <summary>
        /// Codigo : U12
        /// </summary>
        public int Voltaje { get; set; }

        /// <summary>
        /// Codigo : U23
        /// </summary>
        public int Voltaje23 { get; set; }

        /// <summary>
        /// Codigo : U31
        /// </summary>
        public int Voltaje31 { get; set; }
    }

    public class DatosFormatoConsola : EnergiaGenerada
    {

        public string IdDetalleConsola { get; set; }
        public string Fecha { get; set; }       
        public string IdRegistroConsola { get; set; }
        public string IdOperario { get; set; }
        public string IdformatoConsola { get; set; }
        public int Fila { get; set; }
    }
}
