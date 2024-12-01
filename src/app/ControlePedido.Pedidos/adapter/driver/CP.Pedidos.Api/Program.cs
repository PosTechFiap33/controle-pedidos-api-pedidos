using CP.Pedidos.Api;
using CP.Pedidos.Api.Configurations;
using CP.Pedidos.Api.Middlewares;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CP.Pedidos.IOC.DependencyInjections;
using CP.Pedidos.Data.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddApiConfiguration();

        builder.Services.RegisterServices(builder.Configuration);
        

        builder.Services.AddSwaggerConfiguration();

        builder.Services.AddHealthChecks()
                        .AddCheck("self", () => HealthCheckResult.Healthy());

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        if (app.Environment.IsProduction() || app.Environment.IsDevelopment())
            services.ConfigureMigrationDatabase();

        app.UseSwaggerApp();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}