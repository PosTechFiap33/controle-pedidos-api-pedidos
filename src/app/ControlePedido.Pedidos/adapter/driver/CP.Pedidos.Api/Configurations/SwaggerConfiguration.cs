using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CP.Pedidos.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace CP.Pedidos.Api;

[ExcludeFromCodeCoverage]
public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Controle de pedidos",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "DiscordName: Flavio - rm353688"
                }
            });

            c.SchemaFilter<EnumDescriptionFilter>();

            c.CustomSchemaIds(x =>
            {
                var displayName = x.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault()?.DisplayName;

                if (!string.IsNullOrEmpty(displayName))
                    return x.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault()?.DisplayName;

                return x.Name;
            });

            // Inclui a documentação XML gerada nos comentários do código
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.MapType<ValidationProblemDetails>(() => new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                        { "title", new OpenApiSchema { Type = "string" } },
                        { "errors", new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    { "Mensagens", new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string" } } }
                                }
                            }
                        }
                }
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerApp(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}
