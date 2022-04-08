using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NorthwindApp.FrontEnd.Mvc.Identity;
using NorthwindApp.FrontEnd.Mvc.Infrastructure;
using NorthwindApp.FrontEnd.Mvc.Services;

namespace NorthwindApp.FrontEnd.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(NorthwindMappingProfile));

            services.AddDbContext<IdentityDbContext>(opt =>
                opt.UseSqlServer(this.Configuration.GetConnectionString("IdentityConnection")));

            services.AddSingleton<IAdminService, AdminService>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });

            var serviceUrl = this.Configuration["ServerHost"];
            services.AddHttpClient<ICategoriesApiClient, CategoriesHttpApiClient>(client =>
            {
                client.BaseAddress = new Uri(serviceUrl);
            });
            
            services.AddHttpClient<IProductsApiClient, ProductsHttpApiClient>(client =>
            {
                client.BaseAddress = new Uri(serviceUrl);
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Categories}/{action=Index}/{id?}");
            });
        }
    }
}
