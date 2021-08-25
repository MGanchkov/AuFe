using AuFe.Models;
using AuFe.Models.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuFe
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            //services.AddDbContext<UserContext>(options => options.UseSqlServer(connection));
            services.AddTransient<IUsers, UserContext>(_ => new UserContext(connection));


            // установка конфигурации подключения
            //Здесь надо отметить два момента. Во-первых, в методе ConfigureServices() производится установка и настройка всех необходимых сервисов:
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            //Второй момент - в методе Configure() внедряем в конвейер необходимые компоненты middleware:
            app.UseAuthentication();    // аутентификация
            app.UseAuthorization();     // авторизация
            //Метод app.UseAuthentication() встраивает в конвейер компонент AuthenticationMiddleware, который управляет аутентификацией.
            //Его вызов позволяет установить значение для свойства HttpContext.User.
            //И метод app.UseAuthorization() встраивает в конвейер компонент AuthorizationMiddleware,
            // который управляет авторизацией пользователей и разграничивает доступ к ресурсам.

            //В данном случае стоит различать понятия "аутентификация" и "авторизация".
            //Аутентификация отвечает на вопрос, кто пользователь. То есть посредством аутентификации мы идентифицируем пользователя, узнаем, кто он.
            //А авторизация отвечает на вопрос, какие права в системе имеет пользователь, позволяет разграничить доступ к ресурсам приложения.

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/Hello", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
