namespace CP.Pedidos.CrpssCutting.Configuration;

public class AWSConfiguration
{
    public string Region { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string ServiceUrl { get; set; }
    public string PagamentoQueueUrl {get; set;}
}
