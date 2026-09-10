using System.Text;
using FluentValidation.AspNetCore;
using JobTracker.Api.Middleware;
using JobTracker.Application;
using JobTracker.Infrastructure;
using JobTracker.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace JobTracker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================================================
            // Infrastructure and Application
            // ============================================================

            builder.Services.AddInfrastructure(
                builder.Configuration);

            builder.Services.AddApplication();

            // ============================================================
            // JWT Authentication
            // ============================================================

            var jwtOptions =
                builder.Configuration
                    .GetSection(JwtOptions.SectionName)
                    .Get<JwtOptions>()
                ?? throw new InvalidOperationException(
                    "Jwt configuration is missing.");

            builder.Services
                .AddAuthentication(
                    JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                jwtOptions.Issuer,

                            ValidAudience =
                                jwtOptions.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwtOptions.SecretKey))
                        };
                });

            // ============================================================
            // Controllers and Validation
            // ============================================================

            builder.Services.AddControllers();

            builder.Services.AddFluentValidationAutoValidation();

            // ============================================================
            // Swagger
            // ============================================================

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Job Tracker API",
                        Version = "v1",
                        Description =
                            "RESTful API for Job Tracker."
                    });

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "Enter your JWT token.\n\n" +
                            "Example:\n" +
                            "Bearer eyJhbGciOi..."
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
                    });
            });

            builder.Services.AddOpenApi();

            // ============================================================
            // Build Application
            // ============================================================

            var app = builder.Build();

            // ============================================================
            // HTTP Request Pipeline
            // ============================================================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.MapOpenApi();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}