using Oracle.ManagedDataAccess.Client;

namespace Generacion.Application.DataBase
{
    public class ConexionBD : IConexionBD
    {
        private readonly string cadenaConexion;

        public ConexionBD(string cadenaConexion)
        {
            this.cadenaConexion = cadenaConexion;
        }

        public OracleConnection ObtenerConexion()
        {
            return new OracleConnection(cadenaConexion);
        }
    }
}

