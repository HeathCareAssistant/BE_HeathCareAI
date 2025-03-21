﻿using Microsoft.AspNetCore.Identity;
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
using HealthyCareAssistant.Service.Service.firebase;
using HealthyCareAssistant.ModelViews.FirebaseSetting;
using HealthyCareAssistant.Extention;

namespace HealthyCareAssistant.API
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddJwtAuthentication(configuration);
            services.AddSwaggerGen();
            services.AddServices();
            services.AddLogging();
            
       
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
                throw new ArgumentNullException("JwtSettings:Secret", "JWT Secret key cannot be empty in appsettings.json");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);
            Console.WriteLine($"[JWT] Loaded Secret Key: {secretKey}");

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
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"]
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                        Console.WriteLine($"[JWT] Token Received: {token}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("[JWT] Token validation successful.");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine("[JWT] Unauthorized request.");
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = BaseResponse<string>.UnauthorizeResponse("You must log in before performing this action.");
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    }
                };
            });
        }


        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HealthyCareAssistant API",
                    Version = "v1",
                    Description = "API documentation for HealthyCareAssistant"
                });

                // Cấu hình Bearer Token
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and your JWT token."
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });

                //  Hỗ trợ multipart/form-data cho upload file
                option.OperationFilter<SwaggerFileOperationFilter>();
            });
        }





        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<Drug>, GenericRepository<Drug>>();
            services.AddScoped<IGenericRepository<MedicineCabinet>, GenericRepository<MedicineCabinet>>();
            services.AddScoped<IGenericRepository<MedicineCabinetDrug>, GenericRepository<MedicineCabinetDrug>>();
            services.AddScoped<IGenericRepository<Reminder>, GenericRepository<Reminder>>();
            services.AddScoped<IGenericRepository<ReminderDrug>, GenericRepository<ReminderDrug>>();



            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDrugService, DrugService>();
            services.AddScoped<IMedicineCabinetService, MedicineCabinetService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddMemoryCache();
            services.AddSingleton<FirebaseAuthService>();
            services.AddScoped<AuthService>();
            services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
            services.AddScoped<IChatbotService, Service.Service.ChatbotService>();
            services.AddHttpClient();
            // Firebase Cloud Messaging (FCM) Service
            //services.AddScoped<IFcmService, FcmService>();
        }
    }
}
