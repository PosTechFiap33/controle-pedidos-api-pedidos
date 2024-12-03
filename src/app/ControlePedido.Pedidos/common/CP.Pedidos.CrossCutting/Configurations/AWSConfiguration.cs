using System.Diagnostics.CodeAnalysis;

namespace CP.Pedidos.CrpssCutting.Configuration;

[ExcludeFromCodeCoverage]
public class AWSConfiguration
{
    public string Region { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string ServiceUrl { get; set; }
    public string PagamentoQueueUrl { get; set; }

    public AWSConfiguration()
    {
        Region = GetEnvironmentVariableOrDefault("AWS_REGION", Region);
        AccessKey = GetEnvironmentVariableOrDefault("AWS_ACCESS_KEY", AccessKey);
        SecretKey = GetEnvironmentVariableOrDefault("AWS_SECRET_KEY", SecretKey);
        ServiceUrl = GetEnvironmentVariableOrDefault("AWS_SERVICE_URL", ServiceUrl);
        PagamentoQueueUrl = GetEnvironmentVariableOrDefault("AWS_PAGAMENTO_QUEUE", PagamentoQueueUrl);
    }

    private string GetEnvironmentVariableOrDefault(string variableName, string defaultValue = null)
    {
        string value = Environment.GetEnvironmentVariable(variableName);
        return !string.IsNullOrEmpty(value) ? value : defaultValue;
    }
}
