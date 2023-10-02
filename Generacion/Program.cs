using Generacion.Application.DataBase;
using Generacion.Application.DatosConsola;
using Generacion.Application.DatosConsola.Command;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.Funciones;
using Generacion.Application.Mantenimiento;
using Generacion.Application.Mantenimiento.Command;
using Generacion.Application.Usuario;
using Generacion.Application.Usuario.Command;
using Generacion.Application.Usuario.Query;

namespace Generacion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               })
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                   config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                   config.AddEnvironmentVariables();
               })
               .ConfigureServices((context, services) =>
               {
                   IConfiguration configuration = context.Configuration;
                   string cadenaConexion = configuration.GetConnectionString("DefaultConnection");
                   services.AddSession();
                   services.AddScoped<IConexionBD>(_ => new ConexionBD(cadenaConexion));
                   services.AddScoped<IMantenimiento, Mantenimiento>();
                   services.AddScoped<IUsuario, DatosUsuario>();
                   services.AddScoped<IDatosRegistroConsola, DatosRegistroConsola>();
                   
                   services.AddScoped(typeof(FotoServidor));
                   services.AddScoped(typeof(ConsultarUsuario));
                   services.AddScoped(typeof(DatosConsola));
                   
                   // services.AddScoped(typeof(CacheDatos));
               });

    }
}

