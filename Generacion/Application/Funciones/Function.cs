using Generacion.Models.Usuario;
using Newtonsoft.Json;

namespace Generacion.Application.Funciones
{
    public class Function
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Function(IHttpContextAccessor httpContext) {
            _httpContextAccessor = httpContext;
        }

        public async Task<DetalleOperario> ObtenerDatosOperario()
        {
            string usuarioDetail = _httpContextAccessor.HttpContext.Session.GetString("usuarioDetail");
            DetalleOperario user = JsonConvert.DeserializeObject<DetalleOperario>(usuarioDetail);
            return user;
        }

    }
}
