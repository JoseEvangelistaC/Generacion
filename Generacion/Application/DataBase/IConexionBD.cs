using Oracle.ManagedDataAccess.Client;

namespace Generacion.Application.DataBase
{
    public interface IConexionBD
    {
        OracleConnection ObtenerConexion();
    }
}
