using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CP.Pedidos.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace CP.Pedidos.Api.Configurations;

[ExcludeFromCodeCoverage]
public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddControllers(options =>
            options.Filters.Add<CustomModelStateValidationFilter>()
        ).AddJsonOptions(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        services.AddCors(option =>
        {
            option.AddPolicy("Total",
                builder =>
                  builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                );
        });

        return services;
    }
}
