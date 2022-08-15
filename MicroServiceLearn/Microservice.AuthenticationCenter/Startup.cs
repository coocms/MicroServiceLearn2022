using Microservice.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Zhaoxi.NET6.AuthenticationCenter.Utility;
using Zhaoxi.NET6.AuthenticationCenter.Utility.RSA;

namespace Zhaoxi.NET6.AuthenticationCenter
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
            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Zhaoxi.NET6.AuthenticationCenter", Version = "v1" });
            });

            #region HS256 对称可逆加密
            services.AddScoped<IJWTService, JWTHSService>();
            services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTTokenOptions"));
            #endregion

            #region RS256 非对称可逆加密，需要获取一次公钥
            //string keyDir = Directory.GetCurrentDirectory();
            //if (RSAHelper.TryGetKeyParameters(keyDir, true, out RSAParameters keyParams) == false)
            //{
            //    keyParams = RSAHelper.GenerateAndSaveKey(keyDir);
            //}

            //services.AddScoped<IJWTService, JWTRSService>();
            //services.Configure<JWTTokenOptions>(this.Configuration.GetSection("JWTTokenOptions"));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zhaoxi.NET6.AuthenticationCenter v1"));
            }

            app.UseRouting();

            //app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
