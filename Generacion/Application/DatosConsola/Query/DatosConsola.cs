using Generacion.Application.DataBase;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Generacion.Models.DatosConsola;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using System.Data;

namespace Generacion.Application.DatosConsola.Query
{
    public class DatosConsola
    {
        private readonly IConexionBD _conexion;
        public DatosConsola(IConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<Respuesta<Dictionary<string, List<DatosFormatoConsola>>>> ObtenerRegistroDeConsola(string fechaInicio, string fechaFin)
        {
            Respuesta<Dictionary<string, List<DatosFormatoConsola>>> respuesta = new Respuesta<Dictionary<string, List<DatosFormatoConsola>>>();
            Dictionary<string, List<DatosFormatoConsola>> datosFormatoConsola = new Dictionary<string, List<DatosFormatoConsola>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdDetalleConsola,
                                            P , Q, E, EQ, IL1, IL2 ,
                                            IL3, U12 , U23 , U31 ,Hora,fila
                                        from tbl_Det_Consola
                                        WHERE fecha BETWEEN :fechaInicio AND :fechaFin ";

                    string orderBy = fechaInicio == fechaFin ? string.Empty : " ORDER BY  fila asc ";
                    string condicionalMedianoche = " and hora !='0:00'";
                    sqlQuery = $"{sqlQuery}{condicionalMedianoche}{orderBy}";


                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaInicio", OracleDbType.Varchar2).Value = fechaInicio;
                        command.Parameters.Add(":fechaFin", OracleDbType.Varchar2).Value = fechaFin;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<DatosFormatoConsola> listaRegistroConsola = new List<DatosFormatoConsola>();
                            DatosFormatoConsola registroConsola = new DatosFormatoConsola();
                            while (reader.Read())
                            {
                                registroConsola = new DatosFormatoConsola();
                                registroConsola.IdDetalleConsola = reader["IdDetalleConsola"].ToString();
                                registroConsola.PotenciaActiva = int.Parse(reader["P"].ToString());
                                registroConsola.PotenciaReactiva = int.Parse(reader["Q"].ToString());
                                registroConsola.EnergiaActiva = int.Parse(reader["E"].ToString());
                                registroConsola.EnergiaReactiva = int.Parse(reader["EQ"].ToString());
                                registroConsola.CorrienteLinea1 = int.Parse(reader["IL1"].ToString());
                                registroConsola.CorrienteLinea2 = int.Parse(reader["IL2"].ToString());
                                registroConsola.CorrienteLinea3 = int.Parse(reader["IL3"].ToString());
                                registroConsola.Voltaje = int.Parse(reader["U12"].ToString());
                                registroConsola.Voltaje23 = int.Parse(reader["U23"].ToString());
                                registroConsola.Voltaje31 = int.Parse(reader["U31"].ToString());
                                registroConsola.Hora = reader["Hora"].ToString();
                                registroConsola.Fila = int.Parse(reader["Fila"].ToString());

                                listaRegistroConsola.Add(registroConsola);

                            }
                            // Obtener la primera línea para ID "BAA901"
                            var primeraLineaBAA = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith("BAA901"))
                                .ToList();

                            // Obtener la primera línea para ID "EG01"
                            var primeraLineaEG1 = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith("EG01"))
                                .ToList();

                            // Obtener la primera línea para ID "EG02"
                            var primeraLineaEG2 = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith("EG02"))
                                .ToList();

                            // Obtener la primera línea para ID "EG01"
                            var primeraLineaBFA = listaRegistroConsola
                                .Where(x => x.IdDetalleConsola.StartsWith("BFA901"))
                                .ToList();

                            primeraLineaBAA = primeraLineaBAA
                                .OrderBy(h => h.Fila)
                                .ToList();


                            primeraLineaEG1 = primeraLineaEG1
                                .OrderBy(h => h.Fila)
                                .ToList();

                            primeraLineaEG2 = primeraLineaEG2
                                .OrderBy(h => h.Fila)
                                .ToList();

                            primeraLineaBFA = primeraLineaBFA
                                .OrderBy(h => h.Fila)
                                .ToList();


                            datosFormatoConsola.Add("BAA901", primeraLineaBAA);
                            datosFormatoConsola.Add("EG01", primeraLineaEG1);
                            datosFormatoConsola.Add("BFA901", primeraLineaBFA);
                            datosFormatoConsola.Add("EG02", primeraLineaEG2);

                            respuesta.Detalle = datosFormatoConsola;
                            respuesta.IdRespuesta = 0;
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
        public async Task<Respuesta<List<TiposDeRegistroConsola>>> ObtenerTiposDeRegistro()
        {
            Respuesta<List<TiposDeRegistroConsola>> respuesta = new Respuesta<List<TiposDeRegistroConsola>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdRegistroConsola,TipoRegistro,Descripcion  
                                        from tbl_Registro_Consola";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<TiposDeRegistroConsola> tiposDeRegistroConsola = new List<TiposDeRegistroConsola>();
                            TiposDeRegistroConsola registroConsola = new TiposDeRegistroConsola();
                            while (reader.Read())
                            {
                                registroConsola = new TiposDeRegistroConsola();
                                registroConsola.IdRegistroConsola = reader["IdRegistroConsola"].ToString();
                                registroConsola.TipoRegistro = reader["TipoRegistro"].ToString();
                                registroConsola.Descripcion = reader["Descripcion"].ToString();
                                tiposDeRegistroConsola.Add(registroConsola);
                            }
                            respuesta.Detalle = new List<TiposDeRegistroConsola>();
                            respuesta.Detalle = tiposDeRegistroConsola;
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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();

            }

            return respuesta;
        }

        public async Task<Respuesta<Dictionary<string, CabecerasTabla>>> ObtenerCabecerasDeTabla()
        {

            Respuesta<Dictionary<string, CabecerasTabla>> respuesta = new Respuesta<Dictionary<string, CabecerasTabla>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdTipoEngine,Detalle,Descripcion from tbl_Tipo_Cabecera";

                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            Dictionary<string, CabecerasTabla> keyValuesHeaders = new Dictionary<string, CabecerasTabla>();
                            CabecerasTabla cabecerasTabla = new CabecerasTabla();
                            while (reader.Read())
                            {
                                cabecerasTabla = new CabecerasTabla();
                                cabecerasTabla.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                cabecerasTabla.Detalle = reader["Detalle"].ToString();
                                cabecerasTabla.Descripcion = reader["Descripcion"].ToString();

                                keyValuesHeaders.Add(cabecerasTabla.IdTipoEngine, cabecerasTabla);
                            }
                            respuesta.Detalle = new Dictionary<string, CabecerasTabla>();
                            respuesta.Detalle = keyValuesHeaders;

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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Respuesta<List<RegistrosDatosGenerator>>> ObtenerDetalleGenerador(string fecha)
        {
            Respuesta<List<RegistrosDatosGenerator>> respuesta = new Respuesta<List<RegistrosDatosGenerator>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerGenPorFecha", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_Fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<RegistrosDatosGenerator> registrosDatosGenerators = new List<RegistrosDatosGenerator>();
                            RegistrosDatosGenerator registrosDatosGenerator = new RegistrosDatosGenerator();
                            registrosDatosGenerator.detalleGeneradores = new Dictionary<string, DetGeneratorTC>();

                            while (reader.Read())
                            {
                                registrosDatosGenerator = new RegistrosDatosGenerator();
                                registrosDatosGenerator.Fecha = reader["Fecha"].ToString();
                                registrosDatosGenerator.Hora = reader["Hora"].ToString();
                                registrosDatosGenerator.L1 = int.Parse(reader["L1"].ToString());
                                registrosDatosGenerator.L2 = int.Parse(reader["L2"].ToString());
                                registrosDatosGenerator.L3 = int.Parse(reader["L3"].ToString());
                                registrosDatosGenerator.D = int.Parse(reader["D"].ToString());
                                registrosDatosGenerator.ND = int.Parse(reader["ND"].ToString());
                                registrosDatosGenerator.TersionalVibration = double.Parse(reader["TersionalVibration"].ToString());
                                registrosDatosGenerator.IdFormatoConsola = reader["IdFormatoConsola"].ToString();
                                registrosDatosGenerator.IdDetGeneratorConsola = reader["IdDetGeneratorConsola"].ToString();

                                registrosDatosGenerator.detalleGeneradores = await ObtenerDetalleTC(registrosDatosGenerator.IdDetGeneratorConsola);
                                registrosDatosGenerators.Add(registrosDatosGenerator);
                            }
                            registrosDatosGenerators = registrosDatosGenerators.OrderBy(h => TimeSpan.Parse(h.Hora)).ToList();

                            respuesta.Detalle = new List<RegistrosDatosGenerator>();
                            respuesta.Detalle = registrosDatosGenerators;
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }

        public async Task<Dictionary<string, DetGeneratorTC>> ObtenerDetalleTC(string id)
        {
            Dictionary<string, DetGeneratorTC> respuesta = new Dictionary<string, DetGeneratorTC>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerGenPorIdConsola", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdDetGeneratorConsola", OracleDbType.Varchar2, id, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            DetGeneratorTC detGenerator = new DetGeneratorTC();

                            while (reader.Read())
                            {
                                detGenerator = new DetGeneratorTC();
                                detGenerator.IdDetGenerator = reader["IdDetGenerator"].ToString();
                                detGenerator.Fecha = reader["fecha"].ToString();
                                detGenerator.IdTipoEngine = reader["idtipoengine"].ToString();
                                detGenerator.TempOut = int.Parse(reader["tempout"].ToString());
                                detGenerator.Speed = int.Parse(reader["speed"].ToString());
                                detGenerator.IdDetGeneratorConsola = reader["IdDetGeneratorConsola"].ToString();

                                respuesta.Add(detGenerator.IdTipoEngine, detGenerator);
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

        public async Task<Respuesta<List<RegistrosDatosEngine>>> ObtenerDatosEngine(string fecha)
        {
            Respuesta<List<RegistrosDatosEngine>> respuesta = new Respuesta<List<RegistrosDatosEngine>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerEngPorFecha", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_Fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<RegistrosDatosEngine> registrosDatosEngines = new List<RegistrosDatosEngine>();
                            RegistrosDatosEngine registros = new RegistrosDatosEngine();
                            registros.detalleEngine = new Dictionary<string, DetalleEngine>();

                            while (reader.Read())
                            {
                                registros = new RegistrosDatosEngine();
                                registros.IdDetEngineConsola = reader["IdDetEngineConsola"].ToString();
                                registros.Hora = reader["Hora"].ToString();
                                registros.RunHours = int.Parse(reader["RunHours"].ToString());
                                registros.CATemp = int.Parse(reader["CaTemp"].ToString());
                                registros.DiffPressJetPulse = double.Parse(reader["DiffPressJetPulse"].ToString());
                                registros.CAPress = double.Parse(reader["CAPress"].ToString());
                                registros.ExhGasAvgTemp = int.Parse(reader["ExhGasAvgTemp"].ToString());
                                registros.CylPressAvg = int.Parse(reader["CylPressAvg"].ToString());
                                registros.Fecha = reader["Fecha"].ToString();
                                registros.IdFormatoConsola = reader["IdFormatoConsola"].ToString();

                                registros.detalleEngine = await ObtenerDetalleEng(registros.IdDetEngineConsola);
                                registrosDatosEngines.Add(registros);
                            }
                            registrosDatosEngines = registrosDatosEngines.OrderBy(h => TimeSpan.Parse(h.Hora)).ToList();

                            respuesta.Detalle = new List<RegistrosDatosEngine>();
                            respuesta.Detalle = registrosDatosEngines;
                            if (!command.Parameters["p_resultado"].Value.Equals(DBNull.Value))
                            {
                                OracleDecimal oracleDecimalValue = (OracleDecimal)command.Parameters["p_resultado"].Value;

                                respuesta.IdRespuesta = (int)oracleDecimalValue.Value;
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
            }
            catch (Exception ex)
            {
                respuesta.IdRespuesta = 99;
                respuesta.Mensaje = ex.Message.ToString();
            }
            return respuesta;
        }


        public async Task<Dictionary<string, DetalleEngine>> ObtenerDetalleEng(string id)
        {
            Dictionary<string, DetalleEngine> respuesta = new Dictionary<string, DetalleEngine>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerDetEgine", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdDetEngineConsola", OracleDbType.Varchar2, id, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            DetalleEngine detalleEngine = new DetalleEngine();

                            while (reader.Read())
                            {
                                detalleEngine = new DetalleEngine();
                                detalleEngine.IdDetEngine = reader["IdDetEngine"].ToString();
                                detalleEngine.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                detalleEngine.Fecha = reader["Fecha"].ToString();
                                detalleEngine.Presion = int.Parse(reader["Presion"].ToString());
                                detalleEngine.Temp = int.Parse(reader["Temp"].ToString());
                                detalleEngine.IdDetEngineConsola = reader["IdDetEngineConsola"].ToString();

                                respuesta.Add(detalleEngine.IdTipoEngine, detalleEngine);
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

        public async Task<Dictionary<string, LecturasMedianoche>> ObtenerLecturaMediaNoche(string id)
        {
            Dictionary<string, LecturasMedianoche> respuesta = new Dictionary<string, LecturasMedianoche>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerLectMed", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdFormatoConsola", OracleDbType.Varchar2, id, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            LecturasMedianoche lecturas = new LecturasMedianoche();

                            while (reader.Read())
                            {
                                lecturas = new LecturasMedianoche();
                                lecturas.IdLecturas = reader["IdLecturas"].ToString();
                                lecturas.NumeroEG = int.Parse(reader["NumeroEG"].ToString());
                                lecturas.GasconsumedEG = decimal.Parse(reader["GasconsumedEG"].ToString());
                                lecturas.GasEnergiaEG = decimal.Parse(reader["GasEnergiaEG"].ToString());
                                lecturas.ReadingToday = decimal.Parse(reader["ReadingToday"].ToString());
                                lecturas.IdFormatoConsola = reader["IdFormatoConsola"].ToString();

                                respuesta.Add(lecturas.NumeroEG.ToString(), lecturas);
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

        public async Task<Respuesta<List<OutGoingFeeder>>> ObtenerDetalleBAO(string id ,string fecha)
        {
            Respuesta<List<OutGoingFeeder>> respuesta = new Respuesta<List<OutGoingFeeder>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ObtenerDetalleBAO", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new OracleParameter("p_IdTipoEngine", OracleDbType.Varchar2, id.ToUpper(), System.Data.ParameterDirection.Input));
                        command.Parameters.Add(new OracleParameter("p_Fecha", OracleDbType.Varchar2, fecha, System.Data.ParameterDirection.Input));

                        command.Parameters.Add(new OracleParameter("p_resultado", OracleDbType.Decimal, System.Data.ParameterDirection.Output));
                        command.Parameters.Add(new OracleParameter("p_Cursor", OracleDbType.RefCursor, System.Data.ParameterDirection.Output));

                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<OutGoingFeeder> datos = new List<OutGoingFeeder>();
                            OutGoingFeeder outGoing = new OutGoingFeeder();

                            while (reader.Read())
                            {
                                outGoing = new OutGoingFeeder();
                                outGoing.IdOutGoing = reader["IdOutGoing"].ToString();
                                outGoing.IdTipoEngine = reader["IdTipoEngine"].ToString();
                                outGoing.kWh = decimal.Parse(reader["kWh"].ToString());
                                outGoing.kVARh = decimal.Parse(reader["kVARh"].ToString());
                                outGoing.Hora = reader["Hora"].ToString();
                                outGoing.IdFormatoConsola = reader["IdFormatoConsola"].ToString();
                                outGoing.Fila = int.Parse(reader["Fila"].ToString());

                                datos.Add(outGoing);
                            }

                            datos = datos.OrderBy(h => h.Fila).ToList();
                            respuesta.Detalle= datos;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return respuesta;
        }
        public async Task<Respuesta<FormatoConsola>> ObtenerFormatoConsola(string id)
        {
            Respuesta<FormatoConsola> respuesta = new Respuesta<FormatoConsola>();  
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("proc_ConsFormatoConsola", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_IdFormatoConsola", OracleDbType.Varchar2).Value = id;
                        command.Parameters.Add("p_Resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            FormatoConsola formatoConsola = new FormatoConsola();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                formatoConsola = new FormatoConsola();
                                formatoConsola.IdFormatoConsola = row["IdFormatoConsola"].ToString();
                                formatoConsola.Observaciones = row["Observaciones"].ToString();
                                formatoConsola.Fecha = row["Fecha"].ToString();
                            }

                            respuesta.Detalle = formatoConsola;
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
