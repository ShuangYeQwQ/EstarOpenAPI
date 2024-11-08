
using EstarOpenAPI.Application.Interfaces;
using EstarOpenAPI.Infrastructure.Identity.Services;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration _config { get; }
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var key = Encoding.UTF8.GetBytes(System.Configuration.ConfigurationManager.AppSettings["appSecret"] + "");
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = "https://estar.com",      // 设置正确的发行者
                ValidAudience = "https://estar.com/api" ,// 设置正确
                ClockSkew = TimeSpan.Zero        // 设置时间偏移，默认是 5 分钟
            };
            x.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // 验证用户是否存在或其他业务逻辑
                   // if (!UserExistsInDatabase(userId))
                   // {
                   //     context.Fail("Unauthorized");
                   // }

                    return Task.CompletedTask;
                }
            };
        });


            services.AddControllers();
            services.AddHealthChecks();
            services.AddScoped<IHomePageService, HomePageService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/health");

           app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
   
    
    }
}
