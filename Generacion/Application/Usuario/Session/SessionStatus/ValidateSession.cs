using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Generacion.Application.Usuario.Session.SessionStatus
{
    public class ValidarSesion : ActionFilterAttribute
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public ValidarSesion(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var actionName = filterContext.ActionDescriptor.DisplayName;
  //          if (actionName.Equals("Generacion.Controllers.LoginController.Index (Generacion)", StringComparison.OrdinalIgnoreCase))
         //   {
                if (filterContext.ActionArguments.Values.Count() == 0)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
        //    }

            string usuarioDetail = httpContext.Session.GetString("usuarioDetail");

            // Verifica si el usuario est� intentando cerrar sesi�n
            bool esCerrarSesion = actionName.EndsWith("Generacion.Controllers.LoginController.CerrarSesion", StringComparison.OrdinalIgnoreCase);

            if (filterContext.ActionArguments.Values.Count() == 0 && usuarioDetail == null && !esCerrarSesion)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }

            base.OnActionExecuting(filterContext);
        }


    }

}
