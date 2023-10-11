using Generacion.Application.DataBase;
using Generacion.Application.DataBase.cache;
using Generacion.Application.DatosConsola;
using Generacion.Application.DatosConsola.Command;
using Generacion.Application.DatosConsola.Query;
using Generacion.Application.LecturaCampo.Command;
using Generacion.Application.LecturaCampo;
using Generacion.Application.LecturaCampo.Query;
using Generacion.Application.Funciones;
using Generacion.Application.ION.Query;
using Generacion.Application.Mantenimiento;
using Generacion.Application.Mantenimiento.Command;
using Generacion.Application.MGD;
using Generacion.Application.MGD.Command;
using Generacion.Application.MGD.Query;
using Generacion.Application.ReporteDiarioGAS;
using Generacion.Application.ReporteDiarioGAS.Query;
using Generacion.Application.ReporteGAS.Command;
using Generacion.Application.Usuario;
using Generacion.Application.Usuario.Command;
using Generacion.Application.Usuario.Query;
using System.Reflection;
using WebApi.Application.Common.Interfaces;
using WebApi.Application.ValidatePassword.Queries;
using System.DirectoryServices.AccountManagement;
using Generacion.Application.Usuario.Session;
using Generacion.Application.Usuario.Session.SessionStatus;
using Generacion.Application.ValidationSession.Login;
using static Generacion.Controllers.LoginController;

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
                   config.AddJsonFile("ConstantsOptions.json", false, true);
                   config.AddEnvironmentVariables();
               })
               .ConfigureServices((context, services) =>
               {
                   IConfiguration configuration = context.Configuration;
                   string cadenaConexion = configuration.GetConnectionString("DefaultConnection");
                   //services.AddSession();
                   services.AddSession(options =>
                   {
                       options.IdleTimeout = TimeSpan.FromHours(8);
                   });
               
                  // services.AddScoped<ValidarSesion>();
                   services.AddScoped<IConexionBD>(_ => new ConexionBD(cadenaConexion));
                   services.AddScoped<IMantenimiento, Mantenimiento>();
                   services.AddScoped<IUsuario, DatosUsuario>();

                   services.AddScoped<IValidateUser, ValidateUser>();
                   services.AddScoped<IDatosRegistroConsola, DatosRegistroConsola>();
                   services.AddScoped<IRegistroDatosGAS, RegistroDatosGAS>();
                   services.AddScoped<IDatosMGD, RegistroDatosMGD>();
                   services.AddScoped<ILecturaCampo, DatosRegistroCampo>();

                   services.AddScoped(typeof(FotoServidor));
                   services.AddScoped(typeof(ConsultarUsuario));
                   services.AddScoped(typeof(DatosConsola));
                   services.AddScoped(typeof(ObtenerDatosReporteGAS));
                   services.AddScoped(typeof(ConsultarION));
                   services.AddScoped(typeof(ConsultarDatosMGD));
                   services.AddScoped(typeof(CacheDatos));
                   services.AddScoped(typeof(LecturaCampo));

                   services.AddScoped(typeof(CacheDatos));

                   services.AddSingleton<IActiveDirectoryProvider, ActiveDirectoryProvider>();

                   var principalContext = new PrincipalContext(ContextType.Domain);
                   
                   services.AddSingleton(principalContext);

                  /*  services.AddMvc(options =>
                     {
                         options.Filters.Add(typeof(ValidarSesion));
                     });*/
               });
    }
}

