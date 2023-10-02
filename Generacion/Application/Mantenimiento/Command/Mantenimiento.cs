using Generacion.Application.DataBase;
using Generacion.Models.Mantenimiento;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Generacion.Application.Mantenimiento.Command
{
    public class Mantenimiento : IMantenimiento
    {
        private readonly IConexionBD _conexion;
        public Mantenimiento(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<string> GuardarDatosMotoGenerador(List<MotoGenerador> listaMotoGeneradores)
        {
            string mensaje = string.Empty;
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("PROC_InsertarMotoGenerador", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter inputDescripDiario = new OracleParameter("p_idDescripDiario", OracleDbType.Varchar2);
                        OracleParameter inputReporteDiario = new OracleParameter("p_idReporteDiario", OracleDbType.Varchar2);
                        OracleParameter inputNumeroGenerador = new OracleParameter("p_NumeroGenerador", OracleDbType.Int32);
                        OracleParameter inputFecha = new OracleParameter("p_fecha", OracleDbType.Varchar2);
                        OracleParameter inputHora = new OracleParameter("p_hora", OracleDbType.Varchar2);
                        OracleParameter inputDescripcion = new OracleParameter("p_descripcion", OracleDbType.Varchar2);

                        OracleParameter inputCasillaid = new OracleParameter("p_casillaid", OracleDbType.Varchar2);
                        OracleParameter inputCasilladiv = new OracleParameter("p_casilladiv", OracleDbType.Varchar2);
                        OracleParameter inputPosicion = new OracleParameter("p_posicion", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            inputDescripDiario,
                            inputReporteDiario,
                            inputNumeroGenerador,
                            inputFecha,
                            inputHora,
                            inputDescripcion,
                            inputCasillaid,
                            inputCasilladiv,
                            inputPosicion,
                            outputParameter
                        });

                        foreach (MotoGenerador item in listaMotoGeneradores)
                        {
                            if (string.IsNullOrEmpty(item.IdDescripDiario))
                                continue;

                            inputDescripDiario.Value = item.IdDescripDiario;
                            inputReporteDiario.Value = item.IdReporteDiario;
                            inputNumeroGenerador.Value = item.NumeroGenerador;
                            inputFecha.Value = item.Fecha.ToString();
                            inputHora.Value = item.Hora;
                            inputDescripcion.Value = item.Descripcion;

                            inputCasillaid.Value = item.detalleContenido.Casillaid;
                            inputCasilladiv.Value = item.detalleContenido.Casilladiv;
                            inputPosicion.Value = item.detalleContenido.Posicion;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)outputParameter.Value;

                            int resultado = (int)oracleDecimalValue.Value;

                            if (resultado == 0)
                            {
                                mensaje = "Correcto";
                            }
                            else
                            {
                                mensaje = "Error al Guardar ";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return mensaje;
        }

        public async Task<string> GuardarDatosServ(List<MantenimientoServicios> listaMotoGeneradores)
        {
            string mensaje = string.Empty;
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("PROC_InsertarServAux", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter idServiciosAux = new OracleParameter("p_idServiciosAux", OracleDbType.Varchar2);
                        OracleParameter inDescripcion = new OracleParameter("p_descripcion", OracleDbType.Varchar2);
                        OracleParameter inputHora = new OracleParameter("p_fecha", OracleDbType.Varchar2);
                        OracleParameter idReporteDiario = new OracleParameter("p_idReporteDiario", OracleDbType.Varchar2);

                        OracleParameter inputCasillaid = new OracleParameter("p_casillaid", OracleDbType.Varchar2);
                        OracleParameter inputCasilladiv = new OracleParameter("p_casilladiv", OracleDbType.Varchar2);
                        OracleParameter inputPosicion = new OracleParameter("p_posicion", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idServiciosAux,
                            inDescripcion,
                            inputHora,
                            idReporteDiario,
                            inputCasillaid,
                            inputCasilladiv,
                            inputPosicion,
                            outputParameter
                        });


                        foreach (MantenimientoServicios item in listaMotoGeneradores)
                        {
                            if (string.IsNullOrEmpty(item.IdServiciosAux))
                                continue;

                            idServiciosAux.Value = item.IdServiciosAux;
                            inDescripcion.Value = item.Descripcion;
                            inputHora.Value = item.Fecha;
                            idReporteDiario.Value = item.IdReporteDiario;

                            inputCasillaid.Value = item.detalleContenido.Casillaid;
                            inputCasilladiv.Value = item.detalleContenido.Casilladiv;
                            inputPosicion.Value = item.detalleContenido.Posicion;

                            command.ExecuteNonQuery();

                            OracleDecimal oracleDecimalValue = (OracleDecimal)outputParameter.Value;

                            int resultado = (int)oracleDecimalValue.Value;

                            if (resultado != 0)
                            {
                                break; 
                            }
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return mensaje;
        }
    }
}
