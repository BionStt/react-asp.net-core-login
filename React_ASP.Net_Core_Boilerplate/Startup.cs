using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using React_ASP.Net_Core_Boilerplate.IoC;
using React_ASP.Net_Core_Boilerplate.Helpers;

using Service_Repository_Layer.Repo;
using Service_Repository_Layer.Repo.Contracts;
using Service_Repository_Layer.Service;
using Service_Repository_Layer.Service.Contracts;

using Newtonsoft.Json;
using System;
using Microsoft.AspNetCore.Http;

namespace React_ASP.Net_Core_Boilerplate
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var databaseConnectionString = Configuration.GetConnectionString("React_ASP.Net_Core_Boilerplate_MsSqlProvider");
            services.AddCors();
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(databaseConnectionString));

            services.AddMvc()
                .AddJsonOptions(options =>
                 {                     
                     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                     options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                     options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                 })                 
                 .AddSessionStateTempDataProvider();


            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            services.AddDistributedMemoryCache();
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(appSettings.SessionTimeout);
                options.Cookie.HttpOnly = true;
            });
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(options =>
            {
                
            });

            // configure DI for application services
            services.AddScoped<IUserRepository, UserRepository>();          
            services.AddScoped<ILogRepository, LogRepository>();            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<DatabaseInitializer>();
            services.AddScoped<RenderView>();

            services.AddScoped<IUserService, UserService>();            

            // Configures the mapper inside the Services
            MapInitializer.Initialize();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DatabaseInitializer databaseInitializer)
        {

            app.UseResponseBuffering();

            loggerFactory.AddLog4Net();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });

                databaseInitializer.Migrate();
                databaseInitializer.SeedDatabase();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                databaseInitializer.Migrate();
                databaseInitializer.SeedDatabase();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
