﻿using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Generacion.Application.DataBase;
using Generacion.Models.ReporteProduccion;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Types;
using Microsoft.Win32;

namespace Generacion.Application.ReporteProduccion.Command
{
    public class RegistrarProduccion : IReporteProduccion
    {

        private readonly IConexionBD _conexion;
        public RegistrarProduccion(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Respuesta<string>> GuardarDatosEnergiaProducida(EnergiaProducida datoEnergiaProducida)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertOrUpdateEnergiaProducida", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdEnergyProduce", OracleDbType.Varchar2).Value = datoEnergiaProducida.IdEnergyProduce;
                        command.Parameters.Add("p_TipoEnergia", OracleDbType.Varchar2).Value = datoEnergiaProducida.TipoEnergia;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datoEnergiaProducida.Fecha;
                        command.Parameters.Add("p_PmuEng_01", OracleDbType.Varchar2).Value = datoEnergiaProducida.PmuEng_01;
                        command.Parameters.Add("p_PmuEng_02", OracleDbType.Varchar2).Value = datoEnergiaProducida.PmuEng_02;
                        command.Parameters.Add("p_GrosEnergy", OracleDbType.Int32).Value = datoEnergiaProducida.GrosEnergy;

                        command.ExecuteNonQuery();
                        respuesta.IdRespuesta = 0;
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

        public async Task<Respuesta<string>> InsertOrUpdateArranqueSincro(List<ArranqueSincronizacion> detalle)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var arranque in detalle)
                    {
                        using (OracleCommand command = new OracleCommand("InsertOrUpdateNumeroArranque", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_Anual", OracleDbType.Int32).Value = arranque.Anual;
                            command.Parameters.Add("p_Mensual", OracleDbType.Int32).Value = arranque.Mensual;
                            command.Parameters.Add("p_Diario", OracleDbType.Int32).Value = arranque.Diario;
                            command.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = arranque.NumeroGenerador;
                            command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = arranque.Tipo;
                            command.Parameters.Add("p_IdReporteProduccion", OracleDbType.Varchar2).Value = arranque.IdReporteProduccion;

                            command.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                            respuesta.IdRespuesta = (int)outputParameter.Value;
                            respuesta.Mensaje = "Ok";
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

        public async Task<Respuesta<string>> InsertOrUpdateDetalleProduccion(List<DetalleProduccion> detalle)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var detProd in detalle)
                    {
                        using (OracleCommand command = new OracleCommand("InsertOrUpdateDetProd", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdReporteProduccion", OracleDbType.Varchar2).Value = detProd.IdReporteProduccion;
                            command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = detProd.Tipo;
                            command.Parameters.Add("p_Detalle", OracleDbType.Varchar2).Value = detProd.Detalle;

                            command.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                            respuesta.IdRespuesta = (int)outputParameter.Value;
                            respuesta.Mensaje = "Ok";
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


        public async Task<int> InsertOrUpdateProduccionStatus(List<ReporteProduccionStatus> reportes)
        {
            int respuesta = 0;
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var reporte in reportes)
                    {
                        using (OracleCommand command = new OracleCommand("InsertOrUpdateProduccionStatus", connection))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            command.Parameters.Add("p_IdReporteProdStatus", OracleDbType.Varchar2).Value = reporte.IdReporteProdStatus;
                            command.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = reporte.NumeroGenerador;
                            command.Parameters.Add("p_ServiceAccumulated", OracleDbType.Decimal).Value = reporte.ServiceAccumulated;
                            command.Parameters.Add("p_PlannedMainten", OracleDbType.Decimal).Value = reporte.PlannedMainten;
                            command.Parameters.Add("p_ForcedMaintMech", OracleDbType.Decimal).Value = reporte.ForcedMaintMech;
                            command.Parameters.Add("p_ForcedMaintElec", OracleDbType.Decimal).Value = reporte.ForcedMaintElec;
                            command.Parameters.Add("p_ForcedAuxiliaries", OracleDbType.Decimal).Value = reporte.ForcedAuxiliaries;
                            command.Parameters.Add("p_ExternalTrips", OracleDbType.Decimal).Value = reporte.ExternalTrips;
                            command.Parameters.Add("p_StandBy", OracleDbType.Decimal).Value = reporte.StandBy;
                            command.Parameters.Add("p_RunningHours", OracleDbType.Decimal).Value = reporte.RunningHours;
                            command.Parameters.Add("p_HoursAvailable", OracleDbType.Decimal).Value = reporte.HoursAvailable;
                            command.Parameters.Add("p_IdReporteProduccion", OracleDbType.Varchar2).Value = reporte.IdReporteProduccion;
                            command.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = System.Data.ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                            respuesta = (int)outputParameter.Value;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = 99;
            }
            return respuesta;
        }

        public async Task<Respuesta<string>> InsertOrUpdateRegistroEventos(List<RegistroEventos> registros)
        {
            Respuesta<string> respuesta = new Respuesta<string>();

            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    foreach (var registro in registros)
                    {
                        using (OracleCommand command = new OracleCommand("InsertOrUpdateRegistroEventos", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Parámetros de entrada
                            command.Parameters.Add("p_IdRegEventos", OracleDbType.Varchar2).Value = registro.IdRegEventos;
                            command.Parameters.Add("p_NumeroGenerador", OracleDbType.Int32).Value = registro.NumeroGenerador;
                            command.Parameters.Add("p_FechaParada", OracleDbType.Varchar2).Value = registro.FechaParada;
                            command.Parameters.Add("p_FechaArranque", OracleDbType.Varchar2).Value = registro.FechaArranque;
                            command.Parameters.Add("p_Sistema", OracleDbType.Varchar2).Value = registro.Sistema;
                            command.Parameters.Add("p_UnidadFuncional", OracleDbType.Varchar2).Value = registro.UnidadFuncional;
                            command.Parameters.Add("p_ExternalTrips", OracleDbType.Varchar2).Value = registro.ExternalTrips;
                            command.Parameters.Add("p_ForcedMaint", OracleDbType.Varchar2).Value = registro.ForcedMaint;
                            command.Parameters.Add("p_PlannedMaint", OracleDbType.Varchar2).Value = registro.PlannedMaint;
                            command.Parameters.Add("p_StandBy", OracleDbType.Varchar2).Value = registro.StandBy;
                            command.Parameters.Add("p_DescripcionEvento", OracleDbType.Varchar2).Value = registro.DescripcionEvento;
                            command.Parameters.Add("p_IdReporte", OracleDbType.Varchar2).Value = registro.IdReporte;

                            // Parámetro de salida
                            command.Parameters.Add("p_Resultado", OracleDbType.Int32).Direction = ParameterDirection.Output;

                            // Ejecuta el comando
                            command.ExecuteNonQuery();

                            OracleDecimal outputParameter = (OracleDecimal)command.Parameters["p_Resultado"].Value;
                            int resultado = Convert.ToInt32(outputParameter.Value);
                            respuesta.IdRespuesta = resultado;
                            respuesta.Mensaje = resultado == 1 ? "Actualizado exitosamente" : "Insertado exitosamente";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }


        public async Task<Respuesta<string>> GuardarDatosLevelOilCarter(LevelLubeOilCartel datoLevelOilCarter)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();



                    using (OracleCommand command = new OracleCommand("InsertOrUpdateLevelOilCarter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdLevelOilCarter", OracleDbType.Varchar2).Value = datoLevelOilCarter.IdLevelOilCarter;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datoLevelOilCarter.Fecha;
                        command.Parameters.Add("p_TipoCarter", OracleDbType.Varchar2).Value = datoLevelOilCarter.TipoCarter;
                        command.Parameters.Add("p_Generador1", OracleDbType.Decimal).Value = datoLevelOilCarter.Generador1;
                        command.Parameters.Add("p_Generador2", OracleDbType.Decimal).Value = datoLevelOilCarter.Generador2;
                        command.Parameters.Add("p_TotalAdded", OracleDbType.Decimal).Value = datoLevelOilCarter.TotalAdded;

                        command.ExecuteNonQuery();
                        respuesta.IdRespuesta = 0;
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

        public async Task<Respuesta<string>> GuardarDatosRefCarter(RefrescamientoCartel datoRefCarter)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("InsertOrUpdateRefCarter", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdRefrescamientoCarter", OracleDbType.Varchar2).Value = datoRefCarter.IdRefrescamientoCarter;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datoRefCarter.Fecha;
                        command.Parameters.Add("p_TipoRefrescamiento", OracleDbType.Varchar2).Value = datoRefCarter.TipoRefrescamiento;
                        command.Parameters.Add("p_Generador1", OracleDbType.Decimal).Value = datoRefCarter.Generador1;
                        command.Parameters.Add("p_Generador2", OracleDbType.Decimal).Value = datoRefCarter.Generador2;
                        command.Parameters.Add("p_TotalRefrescamiento", OracleDbType.Decimal).Value = datoRefCarter.TotalRefrescamiento;

                        command.ExecuteNonQuery();
                        respuesta.IdRespuesta = 0;
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

        public async Task<Respuesta<string>> GuardarDatosCityGate(CityGateFlow datoCityGate)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();



                    using (OracleCommand command = new OracleCommand("InsertOrUpdateCityGate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdCityGate", OracleDbType.Varchar2).Value = datoCityGate.IdCityGate;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datoCityGate.Fecha;
                        command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = datoCityGate.Tipo;
                        command.Parameters.Add("p_KgEng1", OracleDbType.Decimal).Value = datoCityGate.KgEng1;
                        command.Parameters.Add("p_KgEng2", OracleDbType.Decimal).Value = datoCityGate.KgEng2;

                        command.ExecuteNonQuery();
                        respuesta.IdRespuesta = 0;
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




        public async Task<Respuesta<string>> GuardarDatosManttoVessel(ManttoVessel datoManttoVessel)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();



                    using (OracleCommand command = new OracleCommand("InsertOrUpdateMantto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdTkManttoVessel", OracleDbType.Varchar2).Value = datoManttoVessel.IdTkManttoVessel;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datoManttoVessel.Fecha;
                        command.Parameters.Add("p_TipoTk", OracleDbType.Varchar2).Value = datoManttoVessel.TipoTk;
                        command.Parameters.Add("p_Eg1_Yesterday", OracleDbType.Decimal).Value = datoManttoVessel.Eg1_Yesterday;
                        command.Parameters.Add("p_Eg1_Today", OracleDbType.Decimal).Value = datoManttoVessel.Eg1_Today;
                        command.Parameters.Add("p_Eg2_Yesterday", OracleDbType.Decimal).Value = datoManttoVessel.Eg2_Yesterday;
                        command.Parameters.Add("p_Eg2_Today", OracleDbType.Decimal).Value = datoManttoVessel.Eg2_Today;

                        command.ExecuteNonQuery();
                        respuesta.IdRespuesta = 0;
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

        public async Task<Respuesta<string>> GuardarDatosTkCleanLube(TkCleanLube datoTkCleanLube)
        {
            Respuesta<string> respuesta = new Respuesta<string>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();



                    using (OracleCommand command = new OracleCommand("InsertOrUpdateTkClean", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de entrada
                        command.Parameters.Add("p_IdTkCleanLube", OracleDbType.Varchar2).Value = datoTkCleanLube.IdTkCleanLube;
                        command.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = datoTkCleanLube.Fecha;
                        command.Parameters.Add("p_Tipo", OracleDbType.Varchar2).Value = datoTkCleanLube.Tipo;
                        command.Parameters.Add("p_TkLevel", OracleDbType.Decimal).Value = datoTkCleanLube.TkLevel;
                        command.Parameters.Add("p_TkRead", OracleDbType.Decimal).Value = datoTkCleanLube.TkRead;

                        command.ExecuteNonQuery();
                        respuesta.IdRespuesta = 0;
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
