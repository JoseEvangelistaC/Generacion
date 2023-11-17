using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.LecturasCampo;
using Generacion.Models.Mantenimiento;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

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

        public async Task<string> GuardarDatosAceiteCarter(List<CilindroAceiteCarter> datos)
        {
            string mensaje = string.Empty;
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsertarAceiteCarter", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter idCarter = new OracleParameter("p_IdCarter", OracleDbType.Varchar2);
                        OracleParameter numeroGenerador = new OracleParameter("p_NumeroGenerador", OracleDbType.Varchar2);
                        OracleParameter nivelCarterNuevo = new OracleParameter("p_NivelCarterNuevo", OracleDbType.Varchar2);
                        OracleParameter nivelCarterAnt = new OracleParameter("p_NivelCarterAnt", OracleDbType.Varchar2);
                        OracleParameter nivelTKNuevoNuevo = new OracleParameter("p_NivelTKNuevoNuevo", OracleDbType.Varchar2);
                        OracleParameter nivelTKNuevoAnt = new OracleParameter("p_NivelTKNuevoAnt", OracleDbType.Varchar2);
                        OracleParameter contometroNuevo = new OracleParameter("p_ContometroNuevo", OracleDbType.Varchar2);
                        OracleParameter contometroAnt = new OracleParameter("p_ContometroAnt", OracleDbType.Varchar2);
                        OracleParameter idReporteDiario = new OracleParameter("p_idReporteDiario", OracleDbType.Varchar2);
                        OracleParameter fecha = new OracleParameter("p_Fecha", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idCarter,
                            numeroGenerador,
                            nivelCarterNuevo,
                            nivelCarterAnt,
                            nivelTKNuevoNuevo,
                            nivelTKNuevoAnt,
                            contometroNuevo,
                            contometroAnt,
                            idReporteDiario,
                            fecha,
                            outputParameter
                        });


                        foreach (CilindroAceiteCarter item in datos)
                        {
                            if (string.IsNullOrEmpty(item.IdCarter))
                                continue;

                            idCarter.Value = item.IdCarter;
                            numeroGenerador.Value = item.NumeroGenerador;
                            nivelCarterNuevo.Value = item.NivelCarterNuevo;
                            nivelCarterAnt.Value = item.NivelCarterAnt;
                            nivelTKNuevoNuevo.Value = item.NivelTKNuevo;
                            nivelTKNuevoAnt.Value = item.NivelTKAnt;
                            contometroNuevo.Value = item.ContometroNuevo;
                            contometroAnt.Value = item.ContometroAnt;
                            idReporteDiario.Value = item.IdReporteDiario;
                            fecha.Value = item.Fecha;

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

        public async Task<string> GuardarExpansionVessel(List<ExpansionVersel> listamedicionesVercel)
        {
            string mensaje = string.Empty;
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("PROC_InsertarExpansionVessel", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        OracleParameter idReporteDiario = new OracleParameter("p_idReporteDiario", OracleDbType.Varchar2);
                        OracleParameter numeroGenerador = new OracleParameter("p_NumeroGenerador", OracleDbType.Varchar2);
                        OracleParameter medicion = new OracleParameter("p_medicion", OracleDbType.Varchar2);

                        OracleParameter outputParameter = new OracleParameter("p_resultado", OracleDbType.Decimal);
                        outputParameter.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.AddRange(new OracleParameter[] {
                            idReporteDiario,
                            numeroGenerador,
                            medicion,
                            outputParameter
                        });


                        foreach (ExpansionVersel item in listamedicionesVercel)
                        {
                            if (string.IsNullOrEmpty(item.IdReporteDiario))
                                continue;

                            idReporteDiario.Value = item.IdReporteDiario;
                            numeroGenerador.Value = item.NumeroGenerador;
                            medicion.Value = item.Medicion;

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

        public async Task<Respuesta<string>> GuardarReporteDiario(ReporteDiarioMantenimiento datos)       
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_InsertarReporteDiario", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add("p_idReporteDiario", OracleDbType.Varchar2).Value = datos.IdReporteDiario;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datos.Fecha;
                        command.Parameters.Add("p_HorasMotor01", OracleDbType.Decimal).Value = datos.HorasMotor01;
                        command.Parameters.Add("p_HorasMotor02", OracleDbType.Decimal).Value = datos.HorasMotor02;
                        command.Parameters.Add("p_IdOperario", OracleDbType.Varchar2).Value = datos.IdOperario;

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
                            respuesta.Mensaje = "Error al guardar.";
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
