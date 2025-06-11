using Microsoft.EntityFrameworkCore;
using NimGame.Data;

namespace NimGame
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
            // Adicione seu DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            // Adicione controllers (API)
            services.AddControllers();

            // Configurar CORS para liberar acesso do frontend
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:3000") // Coloque o endereço do seu frontend aqui
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            // Adicione autenticação e autorização aqui, se for usar
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Habilitar CORS com a política definida
            app.UseCors("AllowFrontend");

            // Middleware de autenticação, autorização se usar
            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
