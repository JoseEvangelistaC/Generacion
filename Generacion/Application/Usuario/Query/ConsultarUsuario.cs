using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;

namespace Generacion.Application.Usuario.Query
{
    public class ConsultarUsuario
    {
        private readonly IConexionBD _conexion;
        public ConsultarUsuario(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<Dictionary<string, List<HistorialUsuario>>>> ObtenerDatosHistorial(DetalleOperario usuario, string fecha)
        {
            Respuesta<Dictionary<string, List<HistorialUsuario>>> respuesta = new Respuesta<Dictionary<string, List<HistorialUsuario>>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_consultarHistorial", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parámetro de entrada
                        command.Parameters.Add(new OracleParameter("p_Idusuario", OracleDbType.Varchar2, usuario.IdOperario, System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));

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
                                historial.Hora = reader["hora"].ToString();
                                historial.Descripcion = reader["descripcion"].ToString();
                                historial.IdHistorialOperario = reader["IdHistorialOperario"].ToString();
                                historial.idUsuario = reader["idusuario"].ToString();

                                historialUsuarios.Add(historial);
                            }

                            // Obtener el valor del parámetro de salida p_resultado
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            }
                            respuesta.Detalle = new Dictionary<string, List<HistorialUsuario>>();
                            respuesta.Detalle.Add(usuario.IdOperario, historialUsuarios);

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

        public async Task<Respuesta<Dictionary<string, List<HistorialUsuario>>>> ObtenerDatosHistorialGeneral(string fecha)
        {
            Respuesta<Dictionary<string, List<HistorialUsuario>>> respuesta = new Respuesta<Dictionary<string, List<HistorialUsuario>>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_consultarHistorialGeneral", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Parámetro de entrada
                        command.Parameters.Add(new OracleParameter("p_fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));

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
                                historial.Hora = reader["hora"].ToString();
                                historial.Descripcion = reader["descripcion"].ToString();
                                historial.IdHistorialOperario = reader["IdHistorialOperario"].ToString();
                                historial.idUsuario = reader["idUsuario"].ToString();

                                historialUsuarios.Add(historial);
                            }

                            // Obtener el valor del parámetro de salida p_resultado
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                            }

                            List<string> idOperarios = historialUsuarios.Select(x => x.idUsuario).Distinct().ToList();

                            respuesta.Detalle = new Dictionary<string, List<HistorialUsuario>>();


                            foreach (string item in idOperarios)
                            {
                                var lista = historialUsuarios.Where(x => x.idUsuario.Equals(item)).Select(x => x).ToList();
                                respuesta.Detalle.Add(item, lista);
                            }



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

        public async Task<Respuesta<List<DetalleOperario>>> ObtenerOperarios()
        {
            Respuesta<List<DetalleOperario>> respuesta = new Respuesta<List<DetalleOperario>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerOperarios", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_Resultado", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            respuesta.Detalle = new List<DetalleOperario>();
                            DetalleOperario operario = new DetalleOperario();
                            while (reader.Read())
                            {
                                operario = new DetalleOperario();

                                operario.IdOperario = reader["idOperario"].ToString();
                                operario.Nombre = reader["nombre"].ToString();
                                operario.Apellidos = reader["apellidos"].ToString();

                                respuesta.Detalle.Add(operario);
                            }
                        }
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
