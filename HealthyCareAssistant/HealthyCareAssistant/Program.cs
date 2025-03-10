using HealthyCareAssistant.API;
using HealthyCareAssistant.Middleware;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using DotNetEnv;
using HealthyCareAssistant.ModelViews.MailModelViews;
using HealthyCareAssistant.ModelViews.FirebaseSetting;
namespace HealthyCareAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));
            builder.Services.AddControllers();

            // Đăng ký các dịch vụ cần thiết
            builder.Services.AddConfig(builder.Configuration); 

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
           
            //builder.Services.AddSwaggerGen();

            // Cấu hình CORS cho phép tất cả các nguồn gốc
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });


            var app = builder.Build();
            app.UseCors("AllowAll");
            app.Use(async (context, next) =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                Console.WriteLine($"[JWT] Extracted Token: {token}");
                await next();
            });

            // Configure the HTTP request pipeline.
            app.UseMiddleware<LoggingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

                app.UseSwagger();
                app.UseSwaggerUI();

            app.UseHttpsRedirection();


            app.MapControllers();

            app.Run();
        }
    }
}
