using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.ION;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.ION.Query
{
    public class ConsultarION
    {
        private readonly IConexionBD _conexion;
        public ConsultarION(IConexionBD conexionBD)
        {
            _conexion = conexionBD;
        }

        public async Task<Respuesta<List<Revenue>>> ObtenerDatosION(string fechaMes)
        {
            Respuesta<List<Revenue>> respuesta = new Respuesta<List<Revenue>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_consultarIONPorFecha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fechaMes;
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            List<Revenue> revenues = new List<Revenue>();
                            Revenue revenue = new Revenue();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                revenue = new Revenue();
                                revenue.Date_Time = row["Date_Time"].ToString();
                                revenue.kWhDelInt = decimal.Parse(row["kWh_del_int"].ToString());
                                revenue.kVARhDelInt = decimal.Parse(row["kVARh_del_int"].ToString());
                                revenue.kWhDelInt = decimal.Parse(row["kWh_rec_int"].ToString());
                                revenue.kVARhRecInt = decimal.Parse(row["kVARh_rec_int"].ToString());
                                revenue.VllAvg = decimal.Parse(row["Vll_avg"].ToString());
                                revenue.Freq = decimal.Parse(row["Freq"].ToString());

                                revenue.KWDelInt = revenue.kWhDelInt * 4;
                                revenue.KVARDelInt = revenue.kVARhDelInt * 4;
                                revenue.KWRecInt = revenue.kWhRecInt * 4;
                                revenue.KVARRecInt = (revenue.kVARhRecInt * revenue.kVARhDelInt) *4;

                                revenues.Add(revenue);
                            }

                            respuesta.Detalle = revenues;
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
