using Generacion.Application.DataBase;
using Generacion.Models.ION;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Generacion.Application.MGD.Query
{
    public class ConsultarDatosMGD
    {
        private readonly IConexionBD _conexion;
        public ConsultarDatosMGD(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Respuesta<List<DatosION>>> ObtenerDatosMGD(string fechaMes)
        {
            Respuesta<List<DatosION>> respuesta = new Respuesta<List<DatosION>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ConsultarDatosMGD", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fechaMes;
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            List<DatosION> datos = new List<DatosION>();
                            DatosION dato = new DatosION();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                dato = new DatosION();
                                dato.KWDelInt = decimal.Parse(row["Potencia_Activa_MW"].ToString());
                                dato.KVARDelInt = decimal.Parse(row["Potencia_Reactiva_MVAR"].ToString());
                                dato.KWRecInt = decimal.Parse(row["tension_kV"].ToString());
                                dato.KVARRecInt = decimal.Parse(row["Frecuencia_hz"].ToString());

                                datos.Add(dato);
                            }

                            respuesta.Detalle = datos;
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
