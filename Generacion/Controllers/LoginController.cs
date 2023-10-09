using Microsoft.AspNetCore.Mvc;
using System;

namespace Generacion.Controllers
{
    public class LoginController : Controller
    {
        [HttpPost]
        public ActionResult Login(/*User request*/)
        {
            /*   ValidateSession validateSession = new ValidateSession();

            var valid = validateSession.ValidateSessionUser(request);
               if (valid.StateId == 0)
               {
                   Session["usuario"] = request;

                   return Json(new { Success = valid, RedirectUrl = Url.Action("Index", "Inicio") });
               }
               else
               {
                   return Json(new { Success = valid });
               }*/
            return View();
        }
    }
}
