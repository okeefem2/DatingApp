using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
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
            IdentityBuilder identityBuilder = services.AddIdentityCore<User>(opt => { // AddIdentity auto adds these managers. We use this so we can still use our JWT and configure our password stuff
                // Well at least this showcases the options that are default!
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            }); // Recommend using the default password auth system just doing this so we do not have to refactor everything lol

            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(Role), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddRoleValidator<RoleValidator<Role>>();
            identityBuilder.AddRoleManager<RoleManager<Role>>();
            identityBuilder.AddSignInManager<SignInManager<User>>();

            // services.AddScoped<IAuthRepository, AuthRepository>(); //AddScoped: Service is created once per request in a scope (one for each http request, use same instance for other calls within the request)
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc(opts => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opts.Filters.Add(new AuthorizeFilter(policy));
            })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(opt => {
                        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });
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

            services.AddAuthorization(options => {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")); // Could even require a users min age or some such with policy
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                options.AddPolicy("RequireVIPRole", policy => policy.RequireRole("VIP"));
            });
            services.AddScoped<LogUserActivity>();
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
            app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            // app.UseHttpsRedirection();
            app.UseAuthentication(); // Tell the http pipeline to use the registered authentication scheme
            // Now if Headers contains Authorization: Bearer ${token} on it then the request will go through (if the token is valid)
            app.UseMvc(); // Starting middleware between client and API endpoints, routing requests to the correct controller
        }
    }
}
