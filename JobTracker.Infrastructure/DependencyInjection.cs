using JobTracker.Application.Interfaces.Repository;
using JobTracker.Application.Interfaces.Service;
using JobTracker.Infrastructure.Persistence.Context;
using JobTracker.Infrastructure.Repositories;
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

            // Password Hash
            services.AddScoped<IPasswordHasher, PasswordHasherService>();

            // JWT
            services.AddScoped<IJwtService, JwtService>();

            // User Context
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();



            return services;
        }
    }
}
