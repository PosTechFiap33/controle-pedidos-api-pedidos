using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace CP.Pedidos.Infra.Configurations;

[ExcludeFromCodeCoverage]
public class Integrations
{
    public IEnumerable<ApiConfiguration> ApiConfigurations { get; set; }

    public Integrations()
    {
        ApiConfigurations = GetEnvironmentVariableOrDefault("INTEGRATIONS", ApiConfigurations) ?? new List<ApiConfiguration>();
    }

    private T GetEnvironmentVariableOrDefault<T>(string variableName, T defaultValue)
    {
        string value = Environment.GetEnvironmentVariable(variableName);
        return !string.IsNullOrEmpty(value) ? JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        }) : defaultValue;
    }
}

public class ApiConfiguration
{
    public string ServiceName { get; set; }
    public string Url { get; set; }
}
