using Generacion.Application.DataBase;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Generacion.Models.LecturasCampo;
using System.Data;
using System.Collections.Generic;
using System.Collections;

namespace Generacion.Application.LecturaCampo.Query
{
    public class LecturaCampo
    {
        private readonly IConexionBD _conexion;
        private readonly IConfiguration _configuration;
        public LecturaCampo(IConexionBD conexion, IConfiguration configuration)
        {
            _configuration = configuration;
            _conexion = conexion;
        }

        public async Task<Respuesta<List<TiposRegistroCampo>>> ObtenerTiposDeRegistro()
        {
            Respuesta<List<TiposRegistroCampo>> respuesta = new Respuesta<List<TiposRegistroCampo>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdRegistroConsola,TipoRegistro,Descripcion  
                                        from tbl_Registro_Consola";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<TiposRegistroCampo> listaRegistroCampo = new List<TiposRegistroCampo>();
                            TiposRegistroCampo registroConsola = new TiposRegistroCampo();
                            while (reader.Read())
                            {
                                registroConsola = new TiposRegistroCampo();
                                registroConsola.IdRegistroConsola = reader["IdRegistroConsola"].ToString();
                                registroConsola.TipoRegistro = reader["TipoRegistro"].ToString();
                                registroConsola.Descripcion = reader["Descripcion"].ToString();
                                listaRegistroCampo.Add(registroConsola);
                            }
                            respuesta.Detalle = new List<TiposRegistroCampo>();
                            respuesta.Detalle = listaRegistroCampo;
                            if (respuesta.Detalle.Count() > 0)
                            {
                                respuesta.IdRespuesta = 0;
                                respuesta.Mensaje = "Ok";
                            }
                            else
                            {
                                respuesta.IdRespuesta = 1;
                                respuesta.Mensaje = "No hay registros";
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
        public async Task<Respuesta<Dictionary<int, Dictionary<string, List<DatosFormatoCampo>>>>> ObtenerDetalleCampo(string fecha, string fechaFin, string idSitio)
        {
            Respuesta<Dictionary<int, Dictionary<string, List<DatosFormatoCampo>>>> respuesta = new Respuesta<Dictionary<int, Dictionary<string, List<DatosFormatoCampo>>>>();
            Dictionary<string, List<DatosFormatoCampo>> datosGenerator = new Dictionary<string, List<DatosFormatoCampo>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDetalleCampoPorFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetro de entrada
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = idSitio;

                        // Parámetro de salida (cursor)
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataSet dataSet = new DataSet();

                            adapter.Fill(dataSet);
                            List<DatosFormatoCampo> datos = new List<DatosFormatoCampo>();

                            foreach (DataRow row in dataSet.Tables[0].Rows)
                            {
                                DatosFormatoCampo dato = new DatosFormatoCampo()
                                {
                                    IdReporteCampo = row["IdReporteCampo"].ToString(),
                                    Detalle = decimal.Parse(row["Detalle"].ToString()),
                                    Fecha = row["Fecha"].ToString(),
                                    Fila = int.Parse(row["Fila"].ToString()),
                                    NumeroGenerador = int.Parse(row["NumeroGenerador"].ToString()),
                                    Hora = row["Hora"].ToString(),
                                    IdDetalleCampo = row["IdDetalleCampo"].ToString(),
                                    IdSubtituloCampo = row["IdSubtituloCampo"].ToString(),
                                    Sitio = row["Sitio"].ToString()
                                };
                                datos.Add(dato);
                            }
                            respuesta.Detalle = new Dictionary<int, Dictionary<string, List<DatosFormatoCampo>>>();
                            var idDatosCampo = _configuration.GetSection("IdDatosCampo").Get<List<string>>();
                            for (int i = 1; i <= 2; i++)
                            {
                                datosGenerator = new Dictionary<string, List<DatosFormatoCampo>>();
                                foreach (string item in idDatosCampo)
                                {
                                    var generator = datos
                                        .Where(x => x.IdSubtituloCampo.Equals(item) && x.NumeroGenerador.Equals(i))
                                        .OrderBy(x => x.Fila)
                                        .ToList();

                                    datosGenerator.Add(item, generator);
                                }
                                respuesta.Detalle.Add(i, datosGenerator);
                            }

                            if (respuesta.IdRespuesta == 0)
                            {
                                respuesta.IdRespuesta = 0;
                                respuesta.Mensaje = "Ok";
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

        public async Task<Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>>> ObtenerDetalleCampoPorFecha(string idtipoEngine, int fila, string fecha)
        {
            Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>> respuesta = new Respuesta<Dictionary<int, Dictionary<string, DetalleFecha>>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerDetalleFilaPorFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_fila", OracleDbType.Decimal).Value = fila;
                        command.Parameters.Add("p_idtipoEngine", OracleDbType.Varchar2).Value = idtipoEngine;

                        // Parámetro de salida (cursor)
                        command.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<DetalleFecha> listaDetalle = new List<DetalleFecha>();
                            DetalleFecha detalle = new DetalleFecha();
                            while (reader.Read())
                            {
                                detalle = new DetalleFecha()
                                {
                                    fecha = reader.GetString(0),
                                    detalle = decimal.Parse(reader.GetString(2)),
                                    descripcion = reader.GetString(3),
                                    numeroGenerador = int.Parse(reader.GetString(4)),
                                    codigoDetalle = reader.GetString(5)
                                };
                                listaDetalle.Add(detalle);
                            }

                            respuesta.Detalle = listaDetalle.GroupBy(x => x.numeroGenerador)
                                                     .ToDictionary(
                                                         grupoNumeroGenerador => grupoNumeroGenerador.Key,
                                                         grupoNumeroGenerador => grupoNumeroGenerador
                                                             .GroupBy(x => x.codigoDetalle)
                                                             .ToDictionary(grupoDescripcion => grupoDescripcion.Key, grupoDescripcion => grupoDescripcion.FirstOrDefault())
                                                     );

                            if (respuesta.IdRespuesta == 0)
                            {
                                respuesta.IdRespuesta = 0;
                                respuesta.Mensaje = "Ok";
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
