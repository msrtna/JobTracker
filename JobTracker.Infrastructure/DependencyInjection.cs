using JobTracker.Application.Interfaces;
using JobTracker.Infrastructure.Persistence.Context;
using JobTracker.Infrastructure.Repositories;
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


            return services;
        }
    }
}
