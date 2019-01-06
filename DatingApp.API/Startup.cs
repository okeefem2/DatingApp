using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
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
            services.AddScoped<IAuthRepository, AuthRepository>(); //AddScoped: Service is created once per request in a scope (one for each http request, use same instance for other calls within the request)
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(opt => {
                        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            // services.AddSingleton(Configuration);
            // services.AddOptions();
            // services.Configure<AppSettings>(Configuration.GetSection("AppSettings")); // Custom app settings
            // services.Configure<AppInfo>(Configuration.GetSection("AppInfo")); // custom app info
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { // Register authentication scheme
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false, // These are set to false since we are just using localhost right now
                    ValidateAudience = false
                };
            });
            services.AddScoped<LogUserActivity>();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "../DatingApp-SPA/dist/DatingApp-SPA"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Global exception handler for developlment
            }
            else
            {
                // Global exception handler
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        // Context refers to our HTTP request and response
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        // Grab the error from the context
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            // Write the error message into the http response
                            await context.Response.WriteAsync(error.Error.Message);
                            // This would also be a good place to have logging! WOW this is easy
                        }
                    });
                }); // Global try catch
                // app.UseHsts(); // This tells the browser only to send over https
            }
            seeder.SeedUsers(); // Uncomment me to seed the database on app startup, recomment once the data is seeded otherwise it will seed more data every time you start the app
            // Dev settings
            // app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            // app.UseHttpsRedirection();
            app.UseAuthentication(); // Tell the http pipeline to use the registered authentication scheme
            // Now if Headers contains Authorization: Bearer ${token} on it then the request will go through (if the token is valid)
            app.UseStaticFiles();
            app.UseSpaStaticFiles(); // To service static web app
            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     OnPrepareResponse = context =>
            //     {
            //         context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            //         context.Context.Response.Headers.Add("Expires", "-1");
            //     }
            // });
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action=index}/{id}");
            }); // Starting middleware between client and API endpoints, routing requests to the correct controller

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "../DatingApp-SPA";
                // if (env.IsEnvironment("Development"))
                // {
                //     spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                // }
            });
        }
    }
}
