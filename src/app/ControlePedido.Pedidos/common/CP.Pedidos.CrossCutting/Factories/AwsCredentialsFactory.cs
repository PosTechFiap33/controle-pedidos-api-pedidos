using System;
using Amazon.Runtime;
using Amazon.SQS;
using CP.Pedidos.CrpssCutting.Configuration;

namespace CP.Pedidos.CrossCutting.Factories;

public static class AwsCredentialsFactory
{
    public static BasicAWSCredentials CreateCredentials(this ClientConfig config, AWSConfiguration configuration)
    {
        var credentials = new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey);       

        if (!string.IsNullOrEmpty(configuration.ServiceUrl))
            config.ServiceURL = configuration.ServiceUrl;

        return credentials;
    }
}
