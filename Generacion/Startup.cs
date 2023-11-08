using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Generacion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string cadenaConexion = Configuration.GetConnectionString("DefaultConnection");

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();


            // LOGS
            Log.Logger = new LoggerConfiguration()
           .WriteTo.File("Logs/mylogfile.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddLogging(logging =>
            {
                logging.AddSerilog(); // Agrega el proveedor de Serilog
                logging.AddEventLog();
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}");
            });
        }
    }
}
