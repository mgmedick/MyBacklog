﻿using Lamar;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using GameStatsApp.Interfaces.Helpers;
using GameStatsApp.Repository.Configuration;
using System;

namespace GameStatsApp
{
    public class Startup
    {
        public IConfiguration _config { get; }
        public IWebHostEnvironment _env { get; }

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public void ConfigureContainer(ServiceRegistry services)
        {
            // Add your ASP.Net Core services as usual
            services.AddLogging();
            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc().AddRazorRuntimeCompilation().AddViewOptions(options =>
             {
                 if (_env.IsDevelopment())
                 {
                     options.HtmlHelperOptions.ClientValidationEnabled = true;
                 }
             });

            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/Home/Login"));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.Assembly("GameStatsApp.Interfaces");
                scanner.Assembly("GameStatsApp.Service");
                scanner.Assembly("GameStatsApp.Repository");
                scanner.WithDefaultConventions();
                scanner.SingleImplementationsOfInterface();                
            });

            var connString = _config.GetSection("ConnectionStrings").GetSection("DBConnectionString").Value;
            NPocoBootstrapper.Configure(connString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllerRoute("actions", "{action}", new { controller = "Home", action = "Index", id = "" });
                endpoints.MapControllerRoute("Activate", "Activate", new { controller = "Home", action = "Activate" });
                endpoints.MapControllerRoute("ChangePassword", "ChangePassword", new { controller = "Home", action = "ChangePassword" });
                endpoints.MapControllerRoute("ImportGames", "ImportGames", new { controller = "Home", action = "ImportGames" });
                endpoints.MapControllerRoute("Login", "Login", new { controller = "Home", action = "Login" });
                endpoints.MapControllerRoute("ResetPassword", "ResetPassword", new { controller = "Home", action = "ResetPassword" });
                endpoints.MapControllerRoute("SignUp", "SignUp", new { controller = "Home", action = "SignUp" });
                endpoints.MapControllerRoute("Welcome", "Welcome", new { controller = "Home", action = "Welcome" });
                endpoints.MapControllerRoute("default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = "" });
            });
        }
    }
}
