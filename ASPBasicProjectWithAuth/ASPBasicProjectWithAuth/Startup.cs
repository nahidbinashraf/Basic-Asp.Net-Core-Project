using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPBasicProjectWithAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASPBasicProjectWithAuth
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           services.AddDbContextPool<AppDbContext>(
                                    options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            services.Configure<IdentityOptions>(option =>
            {
                option.Password.RequiredLength = 6;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
               // option.User.RequireUniqueEmail = false;
            });
            services.AddMvc(
                options => {
                    options.EnableEndpointRouting = false;
                    var policy = new AuthorizationPolicyBuilder()
                           .RequireAuthenticatedUser()
                           .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));

                });
            services.AddTransient<IEmployeeRepository, SQLEmployeeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
           
            app.UseStaticFiles();
            // app.UseMvcWithDefaultRoute();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=home}/{action=index}/{id?}");
  
            });
//            app.UseRouting();
            
        }
    }
}
