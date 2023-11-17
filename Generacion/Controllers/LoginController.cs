using Generacion.Application.Usuario.Session;
using Generacion.Application.Usuario.Session.SessionStatus;
using Generacion.Application.ValidationSession.Login;
using Generacion.Models;
using Generacion.Models.Session;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.ComponentModel;
using System.Reflection;

namespace Generacion.Controllers
{
    public class LoginController : Controller
    {
        private readonly IValidateUser _validateUser; 
        public LoginController(IValidateUser validateUser)
        {
            _validateUser = validateUser;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ValidarSession(UsuarioSession usuario)
        {
            string mensajesError = usuario.ValidarPropiedadesNulasOVacias();

            if (mensajesError.Any())
            {
                return Json(new { Success = false, message = mensajesError });
            }
            else
            {
                var respuesta = await _validateUser.ValidateSessionUser(usuario);
                if (respuesta.IdRespuesta == 0 && respuesta.Detalle != null)
                {
                    respuesta.Detalle.IdSitio = usuario.Sitio;
                    HttpContext.Session.SetString("usuarioDetail", JsonConvert.SerializeObject(respuesta.Detalle));

                    return Json(new { Success = true, RedirectUrl = Url.Action("Index", "Home") });
                }
                else
                {
                    return Json(new { Success = false, message = respuesta.Mensaje});
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> CerrarSession()
        {
            HttpContext.Session.Clear();

            // Deshabilitar la caché
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return Json(new { Success = true, RedirectUrl = Url.Action("Index", "Login") });
        }

    }
}
