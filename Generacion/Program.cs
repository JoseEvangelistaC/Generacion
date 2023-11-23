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
using WebApi.Application.Common.Interfaces;
using WebApi.Application.ValidatePassword.Queries;
using System.DirectoryServices.AccountManagement;
using Generacion.Application.Usuario.Session;
using Generacion.Application.ValidationSession.Login;
using Generacion.Application.ION;
using Generacion.Application.ION.Command;
using Generacion.Application.Mantenimiento.Query;
using Generacion.Application.ReporteProduccion;
using Generacion.Application.ReporteProduccion.Command;
using Generacion.Application.ReporteProduccion.Query;
using Generacion.Application.Almacen.Query;
using Generacion.Application.Almacen;
using Generacion.Application.Almacen.Command;
using Generacion.Application.Bujias;
using Generacion.Application.Bujias.Command;
using Generacion.Application.Bujias.Query;
using Generacion.Application.Usuario.Session.SessionStatus;
using Generacion.Application.FiltroCentrifugo.Query;
using Generacion.Application.FiltroCentrifugo;
using Generacion.Application.FiltroCentrifugo.Command;
using MediatR;
using Generacion.Application.Common;
using Generacion.Application.DashBoard.Filtro.Query;
using Generacion.Application.DashBoard.Filtro.Command;
using Generacion.Application.DashBoard.Filtro;
using Generacion.Application.DashBoard.ControlGAS;
using Generacion.Application.DashBoard.ControlGAS.Command;
using Generacion.Application.DashBoard.ControlGAS.Query;
using Generacion.Application.DashBoard.CambioAceite;
using Generacion.Application.DashBoard.CambioAceite.Command;

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
                   string cadenaConexionSQL = configuration.GetConnectionString("SQLConnection");
                   //services.AddSession();
                   services.AddSession(options =>
                   {
                       options.IdleTimeout = TimeSpan.FromHours(8);
                   });

                   services.AddMediatR(typeof(Startup));

                   services.AddScoped<ValidarSesion>();
                   services.AddScoped<IConexionBD>(_ => new ConexionBD(cadenaConexion, cadenaConexionSQL));
                   services.AddScoped<IMantenimiento, Mantenimiento>();
                   services.AddScoped<IUsuario, DatosUsuario>();

                   services.AddScoped<IValidateUser, ValidateUser>();
                   services.AddScoped<IDatosRegistroConsola, DatosRegistroConsola>();
                   services.AddScoped<IRegistroDatosGAS, RegistroDatosGAS>();
                   services.AddScoped<IDatosMGD, RegistroDatosMGD>();
                   services.AddScoped<ILecturaCampo, DatosRegistroCampo>();
                   services.AddScoped<IDatosION, RegistroDatosION>();
                   services.AddScoped<IReporteProduccion, RegistrarProduccion>();
                   services.AddScoped<IAlmacen, RegistrosAlmacen>();
                   services.AddScoped<IBujias, RegistoBujias>();
                   services.AddScoped<IDashBoard, RegistroDashBoard>();
                   services.AddScoped<IRegistroFiltroCentrifugo, RegistroFiltroCentrifugo>();
                   services.AddScoped<IRegistroControlGas, RegistroControlGas>();
                   services.AddScoped<IRegistroControlAceite, RegistroControlAceite>();



                   services.AddScoped(typeof(FotoServidor));
                   services.AddScoped(typeof(ConsultarUsuario));
                   services.AddScoped(typeof(DatosConsola));
                   services.AddScoped(typeof(ObtenerDatosReporteGAS));
                   services.AddScoped(typeof(ConsultarION));
                   services.AddScoped(typeof(ConsultarDatosMGD));
                   services.AddScoped(typeof(CacheDatos));
                   services.AddScoped(typeof(LecturaCampo));
                   services.AddScoped(typeof(DatosMantenimiento));
                   services.AddScoped(typeof(ConsultarProduccion));
                   services.AddScoped(typeof(CacheDatos));
                   services.AddScoped(typeof(ConsultasAlmacen));
                   services.AddScoped(typeof(ConsultaBujias));
                   services.AddScoped(typeof(DatosFiltro));
                   services.AddScoped(typeof(Function));
                   services.AddScoped(typeof(DatosFiltroCentrifugo));
                   services.AddScoped(typeof(ProcessExecutionContextExtensions));
                   services.AddScoped(typeof(ObtenerDetalleConsumoGas));
                   

                   services.AddHttpContextAccessor(); 

                   services.AddSingleton<IActiveDirectoryProvider, ActiveDirectoryProvider>();
                   var principalContext = new PrincipalContext(ContextType.Domain);

                   services.AddSingleton(principalContext);

                   /*  services.AddMvc(options =>
                      {
                          options.Filters.Add(typeof(ValidarSesion));
                      });**/
               });
    }
}

