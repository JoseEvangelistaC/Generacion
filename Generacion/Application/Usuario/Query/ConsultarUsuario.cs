using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.Usuario.Query
{
    public class ConsultarUsuario
    {
        private readonly IConexionBD _conexion;
        public ConsultarUsuario(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<List<HistorialUsuario>>> ObtenerDatosHistorial(string Idusuario)
        {
            Respuesta<List<HistorialUsuario>> respuesta = new Respuesta<List<HistorialUsuario>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_consultarHistorial", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parámetro de entrada
                        command.Parameters.Add(new OracleParameter("p_Idusuario", OracleDbType.Varchar2, Idusuario, System.Data.ParameterDirection.Input));

                        // Parámetro de salida (cursor)
                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<HistorialUsuario> historialUsuarios = new List<HistorialUsuario>();
                            HistorialUsuario historial = new HistorialUsuario();
                            while (reader.Read())
                            {
                                historial = new HistorialUsuario();

                                historial.Fecha = reader["fecha"].ToString();
                                historial.Hora =reader["hora"].ToString();
                                historial.Descripcion = reader["descripcion"].ToString();
                                historial.IdHistorialOperario = reader["IdHistorialOperario"].ToString();

                                historialUsuarios.Add(historial);
                            }

                            // Obtener el valor del parámetro de salida p_resultado
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            }

                            respuesta.Detalle = new List<HistorialUsuario>();
                            respuesta.Detalle = historialUsuarios;
                        }
                    }
                    if (respuesta.IdRespuesta == 0)
                    {
                        respuesta.IdRespuesta = 0;
                        respuesta.Mensaje = "Ok";
                    }
                    else
                    {
                        respuesta.IdRespuesta = 2;
                        respuesta.Mensaje = "Error al consultar.";
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }
    }
}
