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

        public async Task<Respuesta<string>> GuardarDatosPrincipal(List<DatosFormatoCampo> datosFormatoCampo)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var dato in datosFormatoCampo)
                    {
                        using (OracleCommand cmd = new OracleCommand("InsertOrUpdateDetalleCampo", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("p_IdDetalleCampo", OracleDbType.Varchar2).Value = dato.IdDetalleCampo;
                            cmd.Parameters.Add("p_Detalle", OracleDbType.Decimal).Value = dato.Detalle;
                            cmd.Parameters.Add("p_IdSubtituloCampo", OracleDbType.Varchar2).Value = dato.IdSubtituloCampo;
                            cmd.Parameters.Add("p_IdReporteCampo", OracleDbType.Varchar2).Value = dato.IdReporteCampo;
                            cmd.Parameters.Add("p_Hora", OracleDbType.Varchar2).Value = dato.Hora;
                            cmd.Parameters.Add("p_Fila", OracleDbType.Decimal).Value = dato.Fila;
                            cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = dato.Sitio;
                            cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = dato.Fecha;
                            cmd.Parameters.Add("p_NumeroGenerador", OracleDbType.Decimal).Value = dato.NumeroGenerador;

                            OracleParameter p_Respuesta = new OracleParameter("p_resultado", OracleDbType.Decimal);
                            p_Respuesta.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(p_Respuesta);

                            cmd.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_resultado"].Value;

                            respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
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
