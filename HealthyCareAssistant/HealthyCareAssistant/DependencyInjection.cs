using Microsoft.AspNetCore.Identity;
using System.Text;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.Repo.Context;
using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Service.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HealthyCareAssistant.Core.Base;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Repo.UnitOfWork;
using Microsoft.OpenApi.Models;

namespace HealthyCareAssistant.API
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddJwtAuthentication(configuration);
            services.AddServices();
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("MyCnn");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Chuỗi kết nối SQL Server không được để trống.");
            }

            services.AddDbContext<HealthCareAssistantContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }


        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration["JwtSettings:Secret"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("JwtSettings:Secret", "JWT Secret key không được để trống trong appsettings.json");
            }
            var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:Secret"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = BaseResponse<string>.UnauthorizeResponse("Bạn cần đăng nhập trước khi thực hiện thao tác này.");
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = BaseResponse<string>.ForbiddenResponse("Bạn không có quyền truy cập tài nguyên này.");
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"]
                };
            });
        }
        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.EnableAnnotations();
                option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
                    securityScheme: new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter Bearer Authorization: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },new string[]{}
                    }

                });
            });
        }
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IUserService, UserService>(); 
        }
    }
}
