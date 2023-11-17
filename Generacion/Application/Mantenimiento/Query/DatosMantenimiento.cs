using Generacion.Application.DataBase;
using Generacion.Models.ION;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Generacion.Models.Mantenimiento;
using Generacion.Models.DatosConsola;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.Mantenimiento.Query
{
    public class DatosMantenimiento
    {
        private readonly IConexionBD _conexion;
        public DatosMantenimiento(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Respuesta<ReporteDiarioMantenimiento>> ObtenerDatosMGD(string fecha)
        {
            Respuesta<ReporteDiarioMantenimiento> respuesta = new Respuesta<ReporteDiarioMantenimiento>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ConsultarReporteDiario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                                {
                                    OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                    respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                                }
                                if (respuesta.IdRespuesta == 0)
                                {
                                    respuesta.Detalle = new ReporteDiarioMantenimiento();
                                    while (reader.Read())
                                    {
                                        respuesta.Detalle.IdReporteDiario = reader["idreportediario"].ToString();
                                        respuesta.Detalle.Fecha = reader["fecha"].ToString();
                                        respuesta.Detalle.HorasMotor01 = int.Parse(reader["horasmotor01"].ToString());
                                        respuesta.Detalle.HorasMotor02 = int.Parse(reader["horasmotor02"].ToString());
                                        respuesta.Detalle.IdOperario = reader["idoperario"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return respuesta;
        }
    }
}
