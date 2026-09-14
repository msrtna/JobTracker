using JobTracker.Api;
using JobTracker.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Tests.IntegrationTests
{
    public class JobTrackerWebApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(
                (context, config) =>
                {
                    config.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:DefaultConnection"] =
                                "Server=.;Database=JobTracker_Test;Trusted_Connection=True;TrustServerCertificate=True;"
                        });
                });

            builder.ConfigureServices(services =>
            {
                var serviceProvider =
                    services.BuildServiceProvider();

                using var scope =
                    serviceProvider.CreateScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();

                dbContext.Database.Migrate();
            });
        }
    }
}