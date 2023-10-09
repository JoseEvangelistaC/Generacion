using Generacion.Application.DataBase;
using Generacion.Models;
using Oracle.ManagedDataAccess.Client;
using Generacion.Models.LecturasCampo;
using Generacion.Models.Usuario;
using Oracle.ManagedDataAccess.Types;
using Generacion.Models.DatosConsola;

namespace Generacion.Application.LecturaCampo.Query
{
    public class LecturaCampo
    {
        private readonly IConexionBD _conexion;
        public LecturaCampo(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        // AQUI TRAE DATA DE LA TABLA DET_CONSOLA DONDE LO ORDENA SEGUN LA FILE DE FOMRA DESCENDIENTE
        public async Task<Respuesta<Dictionary<string, List<DatosFormatoCampo>>>> ObtenerTiposDeRegistro(string fechaInicio, string fechaFin)
        {
            Respuesta<Dictionary<string, List<DatosFormatoCampo>>> respuesta = new Respuesta<Dictionary<string, List<DatosFormatoCampo>>>();
            Dictionary<string, List<DatosFormatoCampo>> datosFormatoConsola = new Dictionary<string, List<DatosFormatoCampo>>();
            try
            {
                using (OracleConnection connection = _conexion.ObtenerConexion())
                {
                    connection.Open();

                    string sqlQuery = @"select IdDetalleInicioCampo,
                                            P_Activa , H_Operacion, Temp_Ambiente, Humed_Relativa,Hora,Fila
                                        from tbl_Det_Principal_Campo
                                        WHERE fecha BETWEEN :fechaInicio AND :fechaFin ";

                    string orderBy = fechaInicio == fechaFin ? string.Empty : " ORDER BY  Fila asc ";
                    string condicionalMedianoche = " and hora !='0:00'";
                    sqlQuery = $"{sqlQuery}{condicionalMedianoche}{orderBy}";


                    using (OracleCommand command = new OracleCommand(sqlQuery, connection))
                    {

                        command.Parameters.Add(":fechaInicio", OracleDbType.Varchar2).Value = fechaInicio;
                        command.Parameters.Add(":fechaFin", OracleDbType.Varchar2).Value = fechaFin;
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            List<DatosFormatoCampo> listaRegistroCampo= new List<DatosFormatoCampo>();
                            DatosFormatoCampo registroCampo = new DatosFormatoCampo();
                            while (reader.Read())
                            {
                                registroCampo = new DatosFormatoCampo();
                                registroCampo.IdDetalleInicioCampo = reader["IdDetalleInicioCampo"].ToString();
                                registroCampo.P_Activa = int.Parse(reader["P_Activa"].ToString());
                                registroCampo.H_Operacion = int.Parse(reader["H_Operacion"].ToString());
                                registroCampo.Temp_Ambiente = int.Parse(reader["Temp_Ambiente"].ToString());
                                registroCampo.Humed_Relativa = int.Parse(reader["Humed_Relativa"].ToString());
                                registroCampo.Hora = reader["Hora"].ToString();
                                registroCampo.Fila = int.Parse(reader["Fila"].ToString());

                                listaRegistroCampo.Add(registroCampo);

                            }
                            // Obtener la primera línea para ID "SIST_GAS_COMBUSTIBLE"
                            var primeraLineaGasCombustible = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("SIST_GAS_COMBUSTIBLE"))
                                .ToList();

                            // Obtener la primera línea para ID "SIST_AGUA_REFRIG_MOTOR"
                            var primeraAguaRefrig = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("SIST_AGUA_REFRIG_MOTOR"))
                                .ToList();
                            // Obtener la primera línea para ID "SIST_AGUA_REFRIG_LT"
                            var primeraAguaRefrigLt = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("SIST_AGUA_REFRIG_LT"))
                                .ToList();
                            // Obtener la primera línea para ID "SIST_AGUA_REFRIG_HT"
                            var primeraAguaRefrigHt = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("SIST_AGUA_REFRIG_HT"))
                                .ToList();
                            // Obtener la primera línea para ID "ENTRADA_SALIDA_AGUA_RADIADORES"
                            var primeraEntradaSalidaRadiadores = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("ENTRADA_SALIDA_AGUA_RADIADORES"))
                                .ToList();
                            // Obtener la primera línea para ID "COMPRESOR_INSTRUMENTACION"
                            var primeraCompresInstrumentacion = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("COMPRESOR_INSTRUMENTACION"))
                                .ToList();
                            // Obtener la primera línea para ID "COMPRESOR_AIRE_ARRANQUE"
                            var primeraCompresArranque = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("COMPRESOR_AIRE_ARRANQUE"))
                                .ToList();
                            // Obtener la primera línea para ID "GENERADOR"
                            var primeraGenerador = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("GENERADOR"))
                                .ToList();
                            // Obtener la primera línea para ID "COMPLEMENTARIOS"
                            var primeraComplementarios = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("COMPLEMENTARIOS"))
                                .ToList();
                            // Obtener la primera línea para ID "INSPECCIONES"
                            var primeraInspecciones = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("INSPECCIONES"))
                                .ToList();
                            // Obtener la primera línea para ID "SIST_COMUNES"
                            var primeraSistComunes = listaRegistroCampo
                                .Where(x => x.IdDetalleInicioCampo.StartsWith("SIST_COMUNES"))
                                .ToList();


                            //respuesta.Detalle = new Dictionary<string, List<DatosFormatoConsola>>();
                            primeraLineaGasCombustible = primeraLineaGasCombustible
                                .OrderBy(h => h.Fila)
                                .ToList();

                            primeraAguaRefrig = primeraAguaRefrig
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraAguaRefrigLt = primeraAguaRefrigLt
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraAguaRefrigHt = primeraAguaRefrigHt
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraEntradaSalidaRadiadores = primeraEntradaSalidaRadiadores
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraCompresInstrumentacion = primeraCompresInstrumentacion
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraCompresArranque = primeraCompresArranque
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraGenerador = primeraGenerador
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraComplementarios = primeraComplementarios
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraInspecciones = primeraInspecciones
                                .OrderBy(h => h.Fila)
                                .ToList();
                            primeraSistComunes = primeraSistComunes
                                .OrderBy(h => h.Fila)
                                .ToList();

                            //primeraLineaBAA = primeraLineaBAA.OrderBy(h => TimeSpan.Parse(h.Hora)).ToList();
                            //primeraLineaEG1 = primeraLineaEG1.OrderBy(h => TimeSpan.Parse(h.Hora)).ToList();

                            datosFormatoConsola.Add("SIST_GAS_COMBUSTIBLE", primeraLineaGasCombustible);
                            datosFormatoConsola.Add("SIST_AGUA_REFRIG_MOTOR", primeraAguaRefrig);
                            datosFormatoConsola.Add("SIST_AGUA_REFRIG_LT", primeraAguaRefrigLt);
                            datosFormatoConsola.Add("SIST_AGUA_REFRIG_HT", primeraAguaRefrigHt);
                            datosFormatoConsola.Add("ENTRADA_SALIDA_AGUA_RADIADORES", primeraEntradaSalidaRadiadores);
                            datosFormatoConsola.Add("COMPRESOR_INSTRUMENTACION", primeraCompresInstrumentacion);
                            datosFormatoConsola.Add("COMPRESOR_AIRE_ARRANQUE", primeraCompresArranque);
                            datosFormatoConsola.Add("GENERADOR", primeraGenerador);
                            datosFormatoConsola.Add("COMPLEMENTARIOS", primeraComplementarios);
                            datosFormatoConsola.Add("INSPECCIONES", primeraInspecciones);
                            datosFormatoConsola.Add("SIST_COMUNES", primeraSistComunes);


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


        public async Task<Respuesta<List<TiposRegistroCampo>>> ObtenerTiposDeRegistro()
        {
            Respuesta<List<TiposRegistroCampo>> respuesta = new Respuesta<List<TiposRegistroCampo>>();
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
                            List<TiposRegistroCampo> listaRegistroCampo = new List<TiposRegistroCampo>();
                            TiposRegistroCampo registroConsola = new TiposRegistroCampo();
                            while (reader.Read())
                            {
                                registroConsola = new TiposRegistroCampo();
                                registroConsola.IdRegistroConsola = reader["IdRegistroConsola"].ToString();
                                registroConsola.TipoRegistro = reader["TipoRegistro"].ToString();
                                registroConsola.Descripcion = reader["Descripcion"].ToString();
                                listaRegistroCampo.Add(registroConsola);
                            }
                            respuesta.Detalle = new List<TiposRegistroCampo>();
                            respuesta.Detalle = listaRegistroCampo;
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

    }
}
