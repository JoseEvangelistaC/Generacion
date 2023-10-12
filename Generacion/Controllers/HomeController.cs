using Generacion.Application.DataBase.cache;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.Usuario;
using Generacion.Models;
using Generacion.Models.DatosConsola;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace Generacion.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsuario _usuario;
        private readonly DatosConsola _datosConsola;
        private readonly IConfiguration _configuration;
        private readonly CacheDatos _cacheDatos;
        public HomeController(
            ILogger<HomeController> logger,
            IUsuario usuario,
            DatosConsola datosConsola,
            IConfiguration configuration,
            CacheDatos cacheDatos)
        {
            _configuration = configuration;
            _cacheDatos = cacheDatos;
            _logger = logger;
            _usuario = usuario;
            _datosConsola = datosConsola;
        }

        public async Task<IActionResult> Index()
        {
            string usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            if (user != null)
            {
                Respuesta<Dictionary<string, CabecerasTabla>> datoscabecera = await _datosConsola.ObtenerCabecerasDeTabla();
                HttpContext.Session.SetString("datoscabecera", JsonConvert.SerializeObject(datoscabecera.Detalle));

                int horario = ObtenerTurnoHorario();
                usuarioDetail = HttpContext.Session.GetString("usuarioDetail");
                DetalleOperario datos = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);

                datos.IdTurno = horario;

                GuardarDatosHorario($"{datos.Nombre} {datos.Apellidos}", horario);

                ViewData["DetalleOperario"] = datos;

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
    }
}