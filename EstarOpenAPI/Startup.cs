
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
                ValidIssuer = "https://estar.com",      // ������ȷ�ķ�����
                ValidAudience = "https://estar.com/api" ,// ������ȷ
                ClockSkew = TimeSpan.Zero        // ����ʱ��ƫ�ƣ�Ĭ���� 5 ����
            };
            x.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // ��֤�û��Ƿ���ڻ�����ҵ���߼�
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
