using Generacion.Application.Common;
using Generacion.Application.DashBoard;
using Generacion.Application.DashBoard.Query;
using Generacion.Application.DataBase.cache;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.FiltroCentrifugo.Command;
using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Application.Funciones;
using Generacion.Application.Usuario.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.DashBoard;
using Generacion.Models.DatosConsola;
using Generacion.Models.FiltroCentrifugo;
using Generacion.Models.Usuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace Generacion.Controllers
{

    public class HomeController : ApiControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashBoard _dashBoard;
        private readonly DatosConsola _datosConsola;
        private readonly IConfiguration _configuration;
        private readonly CacheDatos _cacheDatos;
        private readonly ConsultarUsuario _consultarUsuario;
        private readonly Function _function; 
        private readonly DatosFiltroCentrifugo _datosFiltroCentrifugo; 
        public HomeController(
            ILogger<HomeController> logger,
            DatosConsola datosConsola,
            IConfiguration configuration,
            CacheDatos cacheDatos, ConsultarUsuario consultarUsuario,
            IDashBoard dashBoard,
            DatosFiltro datosFiltro, DatosFiltroCentrifugo datosFiltroCentrifugo, Function function)
        {
            _consultarUsuario = consultarUsuario;
            _configuration = configuration;
            _cacheDatos = cacheDatos;
            _logger = logger;
            _dashBoard = dashBoard;
            _datosConsola = datosConsola;
            _function = function;
            _datosFiltroCentrifugo = datosFiltroCentrifugo;
        }

        public async Task<IActionResult> Index()
        {
            DetalleOperario user = await _function.ObtenerDatosOperario();

            if (user != null)
            {
                Respuesta<Dictionary<string, CabecerasTabla>> datoscabecera = await _datosConsola.ObtenerCabecerasDeTabla();
                HttpContext.Session.SetString("datoscabecera", JsonConvert.SerializeObject(datoscabecera.Detalle));

                int horario = ObtenerTurnoHorario();
                // DetalleOperario datos = await _function.ObtenerDatosOperario();

                user.IdTurno = horario;

                 GuardarDatosHorario($"{user.Nombre} {user.Apellidos}", horario);

                ViewData["DetalleOperario"] = user;
                ObtenerListaOperarios();
            }

            MantenimientoComponentes requestCentrifugo = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroCentrifugo,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuestaCentrifugo = await Mediator.Send(requestCentrifugo);

            MantenimientoComponentes requestAutomatico = new MantenimientoComponentes()
            {
                TipoComponente = TipoComponente.filtroAutomatico,
                RequiereId = true,
                Seleccion = string.Empty
            };

            var respuestaAutomatico = await Mediator.Send(requestAutomatico);

            ViewData["datosFiltroCentrifugo"] = respuestaCentrifugo.Detalle["datosDashboard"];
            ViewData["datosFiltroAutomatico"] = respuestaAutomatico.Detalle["datosDashboard"];


            return View();
        }
        public async void ObtenerListaOperarios()
        {

            Respuesta<List<DetalleOperario>> operarios = await _consultarUsuario.ObtenerOperarios();

            ViewData["ListaOperarios"] = operarios.Detalle;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public int ObtenerTurnoHorario()
        {
            var horarioOperario = _configuration.GetSection("HorarioOperario");
            var manana = horarioOperario.GetSection("mañana").Get<int[]>();
            var tarde = horarioOperario.GetSection("tarde").Get<int[]>();
            DateTime horaActual = DateTime.Now;
            int hora = horaActual.Hour;

            if (hora >= manana[0] && hora < manana[1])
            {
                return 1;
            }
            else if (hora >= tarde[0] && hora < tarde[1])
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public void GuardarDatosHorario(string idOperario, int idHorario)
        {
            Dictionary<int, List<string>> horarioOperario =
                JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(
                    _cacheDatos.ObtenerContenidoCache("HorarioOperario")
                );
            string jsonConvert = string.Empty;

            if (horarioOperario != null)
            {
                bool operarioExiste = false;
                int horarioActual = 0;

                foreach (var kvp in horarioOperario)
                {
                    if (kvp.Value.Contains(idOperario))
                    {
                        operarioExiste = true;
                        horarioActual = kvp.Key;
                        break;
                    }
                }

                if (operarioExiste)
                {
                    horarioOperario[horarioActual].Remove(idOperario);

                    if (!horarioOperario.ContainsKey(idHorario))
                    {
                        horarioOperario.Add(idHorario, new List<string>());
                    }
                    horarioOperario[idHorario].Add(idOperario);

                    jsonConvert = JsonConvert.SerializeObject(horarioOperario);
                }
                else
                {
                    horarioOperario[idHorario].Add(idOperario);
                    jsonConvert = JsonConvert.SerializeObject(horarioOperario);
                }
            }
            else
            {
                List<string> idOperarios = new List<string>()
                {
                    idOperario
                };
                horarioOperario = new Dictionary<int, List<string>>();
                horarioOperario.Add(idHorario, idOperarios);

                jsonConvert = JsonConvert.SerializeObject(horarioOperario);
            }
            _cacheDatos.GuardarDatosCache("HorarioOperario", jsonConvert);
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosDashBoard([FromBody] List<DashboardDetalleFiltro> detalleFiltro)
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            try
            {
                Respuesta<string> respuesta = new Respuesta<string>();
                foreach (var item in detalleFiltro)
                {
                    string mensajesError = item.ValidarPropiedadesNulasOVacias();
                    if (mensajesError.Any())
                    {
                        respuesta.IdRespuesta = 99;
                        respuesta.Mensaje = mensajesError;
                        return Json(new { respuesta = respuesta });
                    }
                }

                respuesta = await _dashBoard.GuardarDatosFiltro(detalleFiltro, user.IdSitio);
                return Json(new { respuesta = respuesta });

            }
            catch (Exception e)
            {
                return Json(new { respuesta = "" });
            }
        }
    }
}