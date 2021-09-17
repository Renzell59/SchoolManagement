using SchoolManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SchoolManagement.Security;
using SchoolManagement.Utilities;
using Microsoft.AspNetCore.Authentication.Google;

namespace SchoolManagement
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            this._config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "1097054168541-elb4bbl76bcp7mug88uk639popm4uett.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "7ZLtzGRUUln3-3qAM3yttigu";
                    googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
                });

            services.AddDataProtection();
            

            var notificationMetaData = _config.GetSection("NotificationMetadata")
                .Get<NotificationMetadata>();

            services.AddSingleton(notificationMetaData);
            services.AddMvc(options=> 
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

                options.Filters.Add(new AuthorizeFilter (policy));
            });

            services.AddScoped<IStudentRepository, SQLStudentRepository>();
            services.AddScoped<ITeacherRepository, SQLTeacherRepository>();
            services.AddScoped<IStudentCommentRepository, SQLStudentCommentRepository>();

            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("SchoolDBConnection"));
            }
            );

            services.AddAuthorization(config =>
            {
                config.AddPolicy("EditRolePolicy", policy =>
                    policy.Requirements.Add(new ManageAdminRolesAndClaimsRequirement()));
            });

            services.Configure<DataProtectionTokenProviderOptions>(options
                => options.TokenLifespan = TimeSpan.FromHours(3));

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(options
                => options.TokenLifespan = TimeSpan.FromDays(3));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {     
                options.SignIn.RequireConfirmedEmail = true;

                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            })
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<IdentityUser>>("CustomEmailConfirmation");


            services.AddHttpContextAccessor();

            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "/",
                    pattern: "{controller=home}/{action=Index}/{id?}"
                    );
            });
        }
    }
}
