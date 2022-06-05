using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPBasicProjectWithAuth.Models;
using ASPBasicProjectWithAuth.Security;
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

            services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                   
                })                
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddMvc(
                options =>
                {
                    options.EnableEndpointRouting = false;
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                });

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "318526017543-kuaqts12e8veefturvjuosiptvj8elta.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("DeleteRolePolicy",
                    policy => policy.RequireClaim("Delete Role","true"));
                //option.AddPolicy("EditRolePolicy",
                //    policy => policy.RequireAssertion(context =>
                //        (context.User.IsInRole("Admin") && context.User.HasClaim(x => x.Type == "Edit Role" && x.Value == "true"))
                //        || context.User.IsInRole("SuperAdmin")

                //    ));
                option.AddPolicy("EditRolePolicy",
                    policy => policy.AddRequirements(new CustomAuthorizationRoleAndClaimsRequirement()));

                option.AddPolicy("DynamicRoleClaim",
                    policy => policy.AddRequirements(new CustomDynamicAuthoriationWithRoleRequiremnt())
                    ) ;

            });
            services.AddTransient<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationRoleAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationSuperAdminHandler>();
            services.AddSingleton<IAuthorizationHandler, CustomDynamicAuthoriationWithRoleHandler>();
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
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
                //db.Database.Migrate();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=home}/{action=index}/{id?}");
  
            });
//            app.UseRouting();
            
        }
    }
}
