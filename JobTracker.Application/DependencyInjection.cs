using System.Reflection;
using FluentValidation;
using JobTracker.Application.Interfaces;
using JobTracker.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // AutoMapper
            services.AddAutoMapper(assembly);


            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);


            // Services
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IJobCategoryService, JobCategoryService>();
            services.AddScoped<IInterviewService, InterviewService>();


            return services;
        }
    }
}
