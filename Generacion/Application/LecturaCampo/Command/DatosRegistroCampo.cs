using Generacion.Application.DataBase;
using Generacion.Models.LecturasCampo;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Generacion.Application.LecturaCampo.Command
{
    public class DatosRegistroCampo : ILecturaCampo
    {
        private readonly IConexionBD _conexion;
        public DatosRegistroCampo(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Respuesta<string>> GuardarDatosPrincipal(DatosFormatoCampo datosFormatoCampo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsertarDetConsola", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdDetalleInicioCampo", OracleDbType.Varchar2).Value = datosFormatoCampo.IdDetalleInicioCampo;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datosFormatoCampo.Fecha;
                        command.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = datosFormatoCampo.Hora;
                        command.Parameters.Add("p_IdRegistroConsola", OracleDbType.Varchar2).Value = datosFormatoCampo.IdRegistroConsola;
                        command.Parameters.Add("p_Und", OracleDbType.Varchar2).Value = datosFormatoCampo.Und;
                        command.Parameters.Add("p_Rango", OracleDbType.Varchar2).Value = datosFormatoCampo.Rango;
                        command.Parameters.Add("p_P_Activa", OracleDbType.Int32).Value = datosFormatoCampo.P_Activa;
                        command.Parameters.Add("p_H_Operacion ", OracleDbType.Int32).Value = datosFormatoCampo.H_Operacion;
                        command.Parameters.Add("p_Temp_Ambiente", OracleDbType.Int32).Value = datosFormatoCampo.Temp_Ambiente;
                        command.Parameters.Add("p_Humed_Relativa", OracleDbType.Int32).Value = datosFormatoCampo.Humed_Relativa;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datosFormatoCampo.IdOperario;
                        command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = datosFormatoCampo.IdformatoConsola;

                        command.Parameters.Add("p_resultado", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                        respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
                        if (respuesta.IdRespuesta == 0 || respuesta.IdRespuesta == 1)
                        {
                            respuesta.Mensaje = "Ok";
                        }
                        else
                        {
                            respuesta.Mensaje = "No pudo consultar.";
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
