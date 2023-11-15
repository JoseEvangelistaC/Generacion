using Generacion.Application.DataBase;
using Generacion.Models.DashBoard;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Generacion.Application.Funciones;
using Generacion.Models.Usuario;

namespace Generacion.Application.FiltroCentrifugo.Query
{
    public class DatosFiltroCentrifugo
    {
        private readonly IConexionBD _conexion;
        private readonly Function _function;
        public DatosFiltroCentrifugo(IConexionBD conexion, Function function)
        {
            _function = function;
            _conexion = conexion;
        }


        public async Task<Respuesta<List<DetalleFiltro>>> ObtenerDatosFiltroPorSitio()
        {
            Respuesta<List<DetalleFiltro>> respuesta = new Respuesta<List<DetalleFiltro>>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerDetalleFiltro", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<DetalleFiltro>();
                        DetalleFiltro detalleFiltro = new DetalleFiltro();
                        while (reader.Read())
                        {
                            detalleFiltro = new DetalleFiltro()
                            {
                                HorasOperacionMantto = decimal.Parse(reader["HorasOperacionMANTTO"].ToString()),
                                IdDetalleFiltro = reader["idDetalleFiltro"].ToString(),
                                Fecha = reader["Fecha"].ToString(),
                                OperadorEjecutor = reader["OperadorEjecutor"].ToString(),
                                Espesor = decimal.Parse(reader["ESPESOR"].ToString()),
                                HorasOperacion = decimal.Parse(reader["HorasOperacion"].ToString()),
                                NumeroGenerador = decimal.Parse(reader["NumeroGenerador"].ToString()),
                                ProximaHoraCambio = decimal.Parse(reader["ProximaHoraCambio"].ToString()),
                                HorasOpUltimoMantto = decimal.Parse(reader["HorasOpUltimoMANTTO"].ToString()),
                                HorasTrabajadasFiltro = decimal.Parse(reader["HorasTrabajadasFiltro"].ToString()),
                                IdReporteFiltro = reader["idReporteFiltro"].ToString(),
                                Observaciones = reader["OBSERVACIONES"].ToString()
                            };

                            respuesta.Detalle.Add(detalleFiltro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
        public async Task<Respuesta<Dictionary<decimal, List<DetalleFiltro>>>> ObtenerDetallePorNumeroGE()
        {
            Respuesta<List<DetalleFiltro>> datosFiltro = await ObtenerDatosFiltroPorSitio();

            Respuesta<Dictionary<decimal, List<DetalleFiltro>>> respuesta = new Respuesta<Dictionary<decimal, List<DetalleFiltro>>>();

            datosFiltro.Detalle = datosFiltro.Detalle.OrderBy(x =>
            {
                DateTime fecha;
                return DateTime.TryParse(x.Fecha, out fecha) ? fecha : DateTime.MinValue;
            }).ToList();

            respuesta.Detalle = datosFiltro.Detalle
            .GroupBy(x => x.NumeroGenerador)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );

            return respuesta;
        }



        /// <summary>
        /// Se requiere el TIPO de componente
        /// </summary>
        /// <example> TIPO : FiltroCentrifugo o FiltroAutomatico</example>
      /*  public async Task<Respuesta<DetalleFiltro>> ObtenerReporteFiltro(string tipoComponente)
        {
            Respuesta<List<DetalleFiltro>> respuesta = new Respuesta<List<DetalleFiltro>>();
            DetalleOperario user = await _function.ObtenerDatosOperario();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ObtenerReporteFiltro", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = user.IdSitio;
                        cmd.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = tipoComponente;

                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = System.Data.ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_cursor"].Value).GetDataReader();

                        respuesta.Detalle = new List<DetalleFiltro>();
                        DetalleFiltro detalleFiltro = new DetalleFiltro();
                        while (reader.Read())
                        {
                            detalleFiltro = new DetalleFiltro()
                            {
                                HorasOperacionMantto = decimal.Parse(reader["HorasOperacionMANTTO"].ToString()),
                                IdDetalleFiltro = reader["idDetalleFiltro"].ToString(),
                                Fecha = reader["Fecha"].ToString()
                            };

                            respuesta.Detalle.Add(detalleFiltro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }**/

    }
}
