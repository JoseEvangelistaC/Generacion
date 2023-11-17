using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Generacion.Infraestructura
{
    public class ApiControllerBase : Controller
    {

        private ISender _mediator = null!;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    }
}