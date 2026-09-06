using JobTracker.Application;
using JobTracker.Infrastructure;

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


            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
