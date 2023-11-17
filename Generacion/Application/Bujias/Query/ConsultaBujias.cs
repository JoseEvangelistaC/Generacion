using Generacion.Application.DataBase;
using Generacion.Models;
using Generacion.Models.Almacen.Bujias;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Generacion.Application.Bujias.Query
{
    public class ConsultaBujias
    {
        private readonly IConexionBD _conexion;
        public ConsultaBujias(IConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Respuesta<List<RegistroBujias>>> ObtenerdetalleControlCambio(string lado, int fila, int GE, string sitio)
        {
            Respuesta<List<RegistroBujias>> respuesta = new Respuesta<List<RegistroBujias>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GetDetalleControlCambio", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Lado", OracleDbType.Varchar2).Value = lado;
                    cmd.Parameters.Add("p_Fila", OracleDbType.Int32).Value = fila;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = sitio;
                    cmd.Parameters.Add("p_NumeroGE", OracleDbType.Int32).Value = GE;

                    cmd.Parameters.Add("p_item", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();

                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    Dictionary<string, List<RegistroBujias>> registros = new Dictionary<string, List<RegistroBujias>>();
                    List<RegistroBujias> listaRegistro = new List<RegistroBujias>();
                    RegistroBujias registro = new RegistroBujias();
                    while (reader.Read())
                    {
                        registro = new RegistroBujias()
                        {
                            IdDetalleControlCambio = reader["IdDetalleControlCambio"].ToString(),
                            Detalle = int.Parse(reader["Detalle"].ToString()),
                            Fecha = reader["Fecha"].ToString(),
                            Lado = reader["Lado"].ToString(),
                            IdSubtituloCampo = reader["IdSubtituloCampo"].ToString(),
                            Fila = int.Parse(reader["Fila"].ToString()),
                            Item = int.Parse(reader["Item"].ToString()),
                            IdControlCambio = reader["IdControlCambio"].ToString(),
                            Nota = reader["nota"].ToString()
                        };
                        listaRegistro.Add(registro);
                    }
                    respuesta.Detalle = new List<RegistroBujias>();
                    respuesta.Detalle = listaRegistro;

                    OracleDecimal oracleDecimalValue = (OracleDecimal)cmd.Parameters["p_item"].Value;
                    respuesta.Mensaje = oracleDecimalValue.Value.ToString();
                }
                catch (Exception ex)
                {

                }
            }
            return respuesta;
        }

        public async Task<Respuesta<Dictionary<int, Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>>>> ObtenerControlCambioPorLado(string sitio)
        {
            Respuesta<Dictionary<int, Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>>> respuesta = new Respuesta<Dictionary<int, Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>>>();
            try
            {
                Respuesta<List<RegistroBujias>> listaEg1 = await ObtenerdetalleControlCambio(string.Empty, 0, 1, sitio);
                Respuesta<List<RegistroBujias>> listaEg2 = await ObtenerdetalleControlCambio(string.Empty, 0, 2, sitio);

                respuesta.Detalle = new Dictionary<int, Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>>();
                respuesta.Detalle.Add(1, new Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>
                {
                    {
                        "A", ObtenerRegistrosPorItem(listaEg1.Detalle, "A")
                    },
                    {
                        "B", ObtenerRegistrosPorItem(listaEg1.Detalle, "B")
                    }
                });
                respuesta.Detalle.Add(2, new Dictionary<string, List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>>>
                {
                    {
                        "A", ObtenerRegistrosPorItem(listaEg2.Detalle, "A")
                    },
                    {
                        "B", ObtenerRegistrosPorItem(listaEg2.Detalle, "B")
                    }
                });
            }
            catch (Exception ex)
            {

            }

            return respuesta;
        }
        List<Dictionary<int, Dictionary<string, List<RegistroBujias>>>> ObtenerRegistrosPorItem(List<RegistroBujias> lista, string lado)
        {
            return lista
                .Where(x => x.Lado.Equals(lado))
                // .OrderBy(x => x.Fecha)
                .GroupBy(x => x.Item)
                .Select(groupByFecha => groupByFecha
                    // .OrderBy(x => x.Item)
                    .GroupBy(x => x.Item)
                    .ToDictionary(
                        groupByItem => groupByItem.Key,
                        groupByItem => groupByItem
                            .GroupBy(x => x.IdSubtituloCampo)
                            .ToDictionary(
                                groupByIdSubtituloCampo => groupByIdSubtituloCampo.Key,
                                groupByIdSubtituloCampo => groupByIdSubtituloCampo.ToList()
                            )
                    )
                )
                .ToList();
        }


        public async Task<Respuesta<List<DetalleRegistroBujias>>> ObtenerControlCambio(string sitio)
        {
            Respuesta<List<DetalleRegistroBujias>> respuesta = new Respuesta<List<DetalleRegistroBujias>>();
            using (OracleConnection connection = _conexion.ObtenerConexion())
            {
                try
                {
                    connection.Open();

                    OracleCommand cmd = new OracleCommand("GETCONTROLCAMBIO", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_Sitio", OracleDbType.Varchar2).Value = sitio;
                    cmd.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    // Ejecutar el procedimiento almacenado
                    cmd.ExecuteNonQuery();

                    // Obtener el resultado
                    OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["p_Cursor"].Value).GetDataReader();

                    respuesta.IdRespuesta = 0;
                    respuesta.Detalle = new List<DetalleRegistroBujias>();
                    DetalleRegistroBujias registro = new DetalleRegistroBujias();
                    while (reader.Read())
                    {
                        registro = new DetalleRegistroBujias()
                        {
                            Horasoperacion = int.Parse(reader["horasoperacion"].ToString()),
                            Numerogenerador = int.Parse(reader["numerogenerador"].ToString()),
                            Bujiasgastadas = int.Parse(reader["bujiasgastadas"].ToString()),
                        };
                        respuesta.Detalle.Add(registro);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return respuesta;
        }
    }
}
