using Generacion.Application.Common;
using Generacion.Application.DashBoard;
using Generacion.Application.DashBoard.CambioAceite.Command;
using Generacion.Application.DashBoard.ControlGAS.Command;
using Generacion.Application.DashBoard.Filtro;
using Generacion.Application.DashBoard.Filtro.Command;
using Generacion.Application.DashBoard.Filtro.Query;
using Generacion.Application.DataBase.cache;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.FiltroCentrifugo.Command;
using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Application.Funciones;
using Generacion.Application.Usuario.Query;
using Generacion.Infraestructura;
using Generacion.Models;
using Generacion.Models.Aceite;
using Generacion.Models.DashBoard;
using Generacion.Models.DatosConsola;
using Generacion.Models.FiltroCentrifugo;
using Generacion.Models.Usuario;
using Generacion.Views.ControlGAS;
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
        private readonly DatosConsola _datosConsola;
        private readonly IConfiguration _configuration;
        private readonly CacheDatos _cacheDatos;
        private readonly ConsultarUsuario _consultarUsuario;
        private readonly Function _function;
        public HomeController(
            ILogger<HomeController> logger,
            DatosConsola datosConsola,
            IConfiguration configuration,
            CacheDatos cacheDatos, 
            ConsultarUsuario consultarUsuario,
            Function function
            )
        {
            _consultarUsuario = consultarUsuario;
            _configuration = configuration;
            _cacheDatos = cacheDatos;
            _logger = logger;
            _datosConsola = datosConsola;
            _function = function;
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

            var respuestaCentrifugo = await Mediator.Send(new ObtenerDatosDashBoard());
             
            ViewData["datosFiltroCentrifugo"] = respuestaCentrifugo.Detalle["datosFiltroCentrifugo"];
            ViewData["datosFiltroAutomatico"] = respuestaCentrifugo.Detalle["datosFiltroAutomatico"];
            ViewData["datoContrato"] = respuestaCentrifugo.Detalle["datoContrato"];
            ViewData["datosConsumo"] = respuestaCentrifugo.Detalle["datosConsumo"];
            ViewData["datodetalleConsumo"] = respuestaCentrifugo.Detalle["datodetalleConsumo"];

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
            var respuesta = await Mediator.Send(new GuardarDatosDashboard()
            {
                detalleFiltro = detalleFiltro
            });

            return Json(new { respuesta = respuesta });
        }


        [HttpPost]
        public async Task<JsonResult> GuardarContratoGAS([FromBody] ContratoGas datos)
        {
            var respuesta = await Mediator.Send(new GuardarContratoGas()
            {
                contratoGas = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosGAS([FromBody] ConsumoGas datos)
        {
            var respuesta = await Mediator.Send(new GuardarControlGas()
            {
                consumoGas = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDetalleGAS([FromBody] DetalleConsumoGas datos)
        {
            var respuesta = await Mediator.Send(new GuardarDetalleGas()
            {
                detalleConsumoGas = datos
            });

            return Json(new { respuesta = respuesta });
        }

        [HttpPost]
        public async Task<JsonResult> GuardarDatosControlAceite([FromBody] List<ControlCambioAceite> datos)
        {
            var respuesta = await Mediator.Send(new ListaControlCambioAceite()
            {
               datosControl = datos
            });

            return Json(new { respuesta = respuesta });
        }//IRegistroControlAceite

    }
}
