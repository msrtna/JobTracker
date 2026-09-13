using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Infrastructure.Persistence.Context;
using JobTracker.Infrastructure.Repositories;
using JobTracker.Infrastructure.Security;
using JobTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            // DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            // Repositories
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IJobCategoryRepository, JobCategoryRepository>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Password Hash
            services.AddScoped<IPasswordHasher, PasswordHasherService>();

            // JWT & Refresh token
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            // User Context
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();



            return services;
        }
    }
}
