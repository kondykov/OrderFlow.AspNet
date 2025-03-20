using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OrderFlow.Identity.Config;
using OrderFlow.Identity.Infrastructure.Repositories;
using OrderFlow.Identity.Interfaces;
using OrderFlow.Identity.Services;
using OrderFlow.Shared.Infrastructure.Data;
using OrderFlow.Shared.Models.Identity;

namespace OrderFlow.Identity;

public static class IdentityModule
{
    public static void AddIdentityModule(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection("Identity"));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IRefreshTokensRepository, RefreshTokenRepository>();
        
        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Установить true в продакшене
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        builder.Configuration["Identity:Jwt:Secret"] ?? throw new NullReferenceException())),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.Headers.AccessControlAllowOrigin = "*";
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            Error = "Unauthorized",
                            StatusCode = 401
                        }));
                    },
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["JWTCookie"];
                        return Task.CompletedTask;
                    }
                };
            });
    }
}