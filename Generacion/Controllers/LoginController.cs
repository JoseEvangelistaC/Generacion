using Generacion.Application.Usuario.Session;
using Generacion.Application.Usuario.Session.SessionStatus;
using Generacion.Application.ValidationSession.Login;
using Generacion.Models;
using Generacion.Models.Session;
using Generacion.Models.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

            var respuesta = await _validateUser.ValidateSessionUser(usuario);
            if (respuesta.IdRespuesta == 0)
            {
                HttpContext.Session.SetString("usuarioDetail", JsonConvert.SerializeObject(respuesta.Detalle));

                // Dentro del controlador
                return Json(new { Success = true, RedirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Json(new { Success = respuesta.IdRespuesta });
            }
        }
    }
}
