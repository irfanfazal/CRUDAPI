using CRUD1.Model;
using CRUD1.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //this line of code is use to bypass crossorigion 
            services.AddCors(cors => cors.AddPolicy("MyPolicy", builder => {
                builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
            }));

            //here we register the JWT authentication middleware by calling the AddAuthentication method.
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
                {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       //The issuer is the actual server that created the token
                       ValidateIssuer = true,
                       //The receiver of the token is a valid recipient
                       ValidateAudience = true,
                       //The token has not expired
                       ValidateLifetime = true,
                      // The signing key is valid and is trusted by the server
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = "https://localhost:5001",
                      ValidAudience = "https://localhost:5001",
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))
        };
    });
            // register the TokenService for dependency injection 
            services.AddTransient<ITokenService, TokenService>();

            services.AddControllers();
            services.AddDbContext<usersContext>(db => db.UseSqlServer(Configuration["ConnectionString:constr"]));
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors("MyPolicy");

            app.UseAuthorization();

            /*
             Usually, all the files in the wwwroot folder are servable for the client applications. 
            We provide that by adding app.UseStaticFiles() in the Startup class in the Configure method.
            Of course, our uploaded images will be stored in the Resources folder, 
            and due to that, we need to make it servable as well. To do that, let’s modify the startup.cs class:
             */
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
            RequestPath = new PathString("/Resources")
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
