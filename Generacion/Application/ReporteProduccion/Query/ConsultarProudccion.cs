using Generacion.Models.ReporteProduccion;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Generacion.Application.DataBase;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;

namespace Generacion.Application.ReporteProduccion.Query
{
    public class ConsultarProduccion
    {
        private readonly IConexionBD _conexion;
        public ConsultarProduccion(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<List<EnergiaProducida>>> ObtenerRegistroProduccion(string fechaActual)
        {
            Respuesta<List<EnergiaProducida>> respuesta = new Respuesta<List<EnergiaProducida>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();
                    string sqlQuery = @"SELECT * FROM Tbl_EnergiaProducida WHERE Fecha = TO_CHAR(TO_DATE(:fechaActual, 'DD_MM_YYYY') - 1, 'DD_MM_YYYY') AND TipoEnergia = 'READING TODAY'";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<EnergiaProducida> listaProducida = new List<EnergiaProducida>();
                            EnergiaProducida registroProduccion = new EnergiaProducida();
                            while (reader.Read())
                            {
                                registroProduccion = new EnergiaProducida();
                                registroProduccion.IdEnergyProduce = reader["IdEnergyProduce"].ToString();
                                registroProduccion.TipoEnergia = reader["TipoEnergia"].ToString();
                                registroProduccion.Fecha = reader["Fecha"].ToString();
                                registroProduccion.PmuEng_01 = decimal.Parse(reader["PmuEng_01"].ToString());
                                registroProduccion.PmuEng_02 = decimal.Parse(reader["PmuEng_02"].ToString());
                                registroProduccion.GrosEnergy = decimal.Parse(reader["GrosEnergy"].ToString());

                                listaProducida.Add(registroProduccion);
                            }
                            respuesta.Detalle = new List<EnergiaProducida>();
                            respuesta.Detalle = listaProducida;
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
                if (respuesta.IdRespuesta == 0)
                {
                    respuesta.IdRespuesta = 0;
                    respuesta.Mensaje = "Ok";
                }
                else
                {
                    respuesta.IdRespuesta = 2;
                    respuesta.Mensaje = "Error al consultar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Respuesta<List<LevelLubeOilCartel>>> ObtenerRegistroLevelCartel(string fechaActual)
        {

            Respuesta<List<LevelLubeOilCartel>> respuesta = new Respuesta<List<LevelLubeOilCartel>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"SELECT * FROM tbl_Level_Oil_Carter WHERE Fecha = TO_CHAR(TO_DATE(:fechaActual, 'DD_MM_YYYY') - 1, 'DD_MM_YYYY') AND TipoCarter = 'TODAY'";


                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<LevelLubeOilCartel> listaLevelCartel = new List<LevelLubeOilCartel>();
                            LevelLubeOilCartel registroProduccionCartel = new LevelLubeOilCartel();
                            while (reader.Read())
                            {
                                registroProduccionCartel = new LevelLubeOilCartel();
                                registroProduccionCartel.IdLevelOilCarter = reader["IdLevelOilCarter"].ToString();
                                registroProduccionCartel.Fecha = reader["Fecha"].ToString();
                                registroProduccionCartel.TipoCarter = reader["TipoCarter"].ToString();
                                registroProduccionCartel.Generador1 = decimal.Parse(reader["Generador1"].ToString());
                                registroProduccionCartel.Generador2 = decimal.Parse(reader["Generador2"].ToString());
                                registroProduccionCartel.TotalAdded = decimal.Parse(reader["TotalAdded"].ToString());

                                listaLevelCartel.Add(registroProduccionCartel);

                            }

                            respuesta.Detalle = new List<LevelLubeOilCartel>();
                            respuesta.Detalle = listaLevelCartel;
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
                if (respuesta.IdRespuesta == 0)
                {
                    respuesta.IdRespuesta = 0;
                    respuesta.Mensaje = "Ok";
                }
                else
                {
                    respuesta.IdRespuesta = 2;
                    respuesta.Mensaje = "Error al consultar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }


        public async Task<Respuesta<List<CityGateFlow>>> ObtenerRegistroCityGate(string fechaActual)
        {

            Respuesta<List<CityGateFlow>> respuesta = new Respuesta<List<CityGateFlow>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"SELECT * FROM tbl_CityGate WHERE Fecha = TO_CHAR(TO_DATE(:fechaActual, 'DD_MM_YYYY') - 1, 'DD_MM_YYYY') AND Tipo = 'READING TODAY'";


                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<CityGateFlow> listaCity = new List<CityGateFlow>();
                            CityGateFlow registroProduccionCity = new CityGateFlow();
                            while (reader.Read())
                            {
                                registroProduccionCity = new CityGateFlow();
                                registroProduccionCity.IdCityGate = reader["IdCityGate"].ToString();
                                registroProduccionCity.Fecha = reader["Fecha"].ToString();
                                registroProduccionCity.Tipo = reader["Tipo"].ToString();
                                registroProduccionCity.KgEng1 = decimal.Parse(reader["KgEng1"].ToString());
                                registroProduccionCity.KgEng2 = decimal.Parse(reader["KgEng2"].ToString());

                                listaCity.Add(registroProduccionCity);

                            }

                            respuesta.Detalle = new List<CityGateFlow>();
                            respuesta.Detalle = listaCity;
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
                if (respuesta.IdRespuesta == 0)
                {
                    respuesta.IdRespuesta = 0;
                    respuesta.Mensaje = "Ok";
                }
                else
                {
                    respuesta.IdRespuesta = 2;
                    respuesta.Mensaje = "Error al consultar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }
        public async Task<Respuesta<List<TkCleanLube>>> ObtenerRegistroTkClean(string fechaActual)
        {

            Respuesta<List<TkCleanLube>> respuesta = new Respuesta<List<TkCleanLube>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"SELECT * FROM tbl_TkCleanLubeOil WHERE Fecha = TO_CHAR(TO_DATE(:fechaActual, 'DD_MM_YYYY') - 1, 'DD_MM_YYYY') AND Tipo = 'READING TODAY'";


                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaActual", OracleDbType.Varchar2).Value = fechaActual;

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<TkCleanLube> listaTkClean = new List<TkCleanLube>();
                            TkCleanLube registroProduccionTkClean = new TkCleanLube();
                            while (reader.Read())
                            {
                                registroProduccionTkClean = new TkCleanLube();
                                registroProduccionTkClean.IdTkCleanLube = reader["IdTkCleanLube"].ToString();
                                registroProduccionTkClean.Fecha = reader["Fecha"].ToString();
                                registroProduccionTkClean.Tipo = reader["Tipo"].ToString();
                                registroProduccionTkClean.TkLevel = decimal.Parse(reader["TkLevel"].ToString());
                                registroProduccionTkClean.TkRead = decimal.Parse(reader["TkRead"].ToString());

                                listaTkClean.Add(registroProduccionTkClean);

                            }

                            respuesta.Detalle = new List<TkCleanLube>();
                            respuesta.Detalle = listaTkClean;
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
                if (respuesta.IdRespuesta == 0)
                {
                    respuesta.IdRespuesta = 0;
                    respuesta.Mensaje = "Ok";
                }
                else
                {
                    respuesta.IdRespuesta = 2;
                    respuesta.Mensaje = "Error al consultar.";
                }
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }
        public async Task<Respuesta<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>>> ObtenerNumeroArranque(string idReporte)
        {
            Respuesta<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>> respuesta = new Respuesta<Dictionary<string, Dictionary<int, ArranqueSincronizacion>>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("ObtenerNumeroArranque", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdReporteProduccion", OracleDbType.Varchar2).Value = idReporte;

                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            Dictionary<string, Dictionary<int, ArranqueSincronizacion>> detalle = new Dictionary<string, Dictionary<int, ArranqueSincronizacion>>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                int anual = Convert.ToInt32(row["anual"]);
                                int mensual = Convert.ToInt32(row["mensual"]);
                                int numeroGenerador = Convert.ToInt32(row["numeroGenerador"]);
                                string tipo = row["tipo"].ToString();
                                string idReporteProduccion = row["idReporteProduccion"].ToString();

                                ArranqueSincronizacion arranqueSincronizacion = new ArranqueSincronizacion
                                {
                                    Anual = anual,
                                    Mensual = mensual,
                                    NumeroGenerador = numeroGenerador,
                                    Tipo = tipo,
                                    IdReporteProduccion = idReporteProduccion
                                };

                                if (!detalle.ContainsKey(arranqueSincronizacion.Tipo))
                                {
                                    detalle[arranqueSincronizacion.Tipo] = new Dictionary<int, ArranqueSincronizacion>();
                                }
                                detalle[arranqueSincronizacion.Tipo][arranqueSincronizacion.NumeroGenerador] = arranqueSincronizacion;
                            }
                            respuesta.Detalle = detalle;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return respuesta;
        }

        public async Task<Respuesta<Dictionary<string, ManttoVessel>>> ObtenerMantoTKVessel(string fecha)
        {
            Respuesta<Dictionary<string, ManttoVessel>> respuesta = new Respuesta<Dictionary<string, ManttoVessel>>();

            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GetManttoVesselData", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();


                    Dictionary<string, ManttoVessel> mantoVessel = new Dictionary<string, ManttoVessel>();
                    ManttoVessel dato = new ManttoVessel();
                    while (reader.Read())
                    {
                        dato = new ManttoVessel()
                        {
                            IdTkManttoVessel = reader["idtkmanttovessel"].ToString(),
                            Fecha = reader["fecha"].ToString(),
                            TipoTk = reader["tipotk"].ToString(),
                            Eg1_Yesterday = decimal.Parse(reader["eg1_yesterday"].ToString()),
                            Eg1_Today = decimal.Parse(reader["eg1_today"].ToString()),
                            Eg2_Yesterday = decimal.Parse(reader["eg2_yesterday"].ToString()),
                            Eg2_Today = decimal.Parse(reader["eg2_today"].ToString())
                        };

                        mantoVessel.Add(dato.TipoTk,dato);
                    }


                    respuesta.Detalle = new Dictionary<string, ManttoVessel>();
                    respuesta.Detalle = mantoVessel;
                }
                catch (Exception ex)
                {
                }
            }
            return respuesta;
        }
    }
}
