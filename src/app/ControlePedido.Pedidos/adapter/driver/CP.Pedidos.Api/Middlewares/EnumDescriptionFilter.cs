using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CP.Pedidos.Api.Middlewares;

[ExcludeFromCodeCoverage]
public class EnumDescriptionFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var value in Enum.GetValues(context.Type))
            {
                var enumValue = Convert.ToInt32(value);
                var enumName = Enum.GetName(context.Type, value);
                var enumDescription = GetEnumDescription(context.Type.GetField(enumName));

                schema.Enum.Add(new OpenApiInteger(enumValue));
                schema.Description += $" {enumValue}: {enumDescription}; ";
            }
        }
    }

    private string GetEnumDescription(FieldInfo fieldInfo)
    {
        var descriptionAttribute = (DescriptionAttribute)fieldInfo
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .FirstOrDefault();

        return descriptionAttribute?.Description ?? fieldInfo.Name;
    }
}
