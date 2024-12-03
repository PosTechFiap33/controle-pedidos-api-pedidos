using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using CP.Pedidos.CrossCutting.Factories;
using CP.Pedidos.CrpssCutting.Configuration;
using CP.Pedidos.Domain.Adapters.MessageBus;
using CP.Pedidos.Domain.Base;
using Microsoft.Extensions.Options;

namespace CP.Pedidos.Infra.Messaging;

[ExcludeFromCodeCoverage]
public class SqsMessageBus : IMessageBus
{
    private readonly AWSConfiguration _configuration;

    public SqsMessageBus(IOptions<AWSConfiguration> aWSConfiguration)
    {
        _configuration = aWSConfiguration.Value;
    }

    public async Task DeleteMessage(string topicOrQueue, string messageId)
    {
        var client = CreateClient();

        await client.DeleteMessageAsync(topicOrQueue, messageId);
    }

    public async Task PublishAsync<T>(T message, string queueUrl)
    {
        var client = CreateClient();

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = message is string ? message.ToString() : System.Text.Json.JsonSerializer.Serialize(message)
        };

        await client.SendMessageAsync(sendMessageRequest);
    }

    public async Task<IEnumerable<MessageResult<T>>> ReceiveMessagesAsync<T>(string queueUrl, int maxMessages = 10, int waitTimeSeconds = 5)
    {
        try
        {
            var client = CreateClient();

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = maxMessages,
                WaitTimeSeconds = waitTimeSeconds,
                AttributeNames = new List<string> { "All" }, // Retorna todos os atributos da mensagem
                MessageAttributeNames = new List<string> { "All" } // Retorna todos os atributos customizados da mensagem
            };

            var response = await client.ReceiveMessageAsync(receiveMessageRequest);

            var messages = new List<MessageResult<T>>();

            foreach (var message in response.Messages)
            {
                try
                {
                    messages.Add(new MessageResult<T>(JsonSerializer.Deserialize<T>(message.Body), message.ReceiptHandle));
                }
                catch (JsonException ex)
                {
                    throw new IntegrationExceptions($"Erro ao desserializar mensagem do SQS: {ex.Message}");
                }
            }

            return messages;
        }
        catch (Exception ex)
        {
            //TODO: melhorar
            throw ex;
        }
    }

    private AmazonSQSClient CreateClient()
    {
        var config = new AmazonSQSConfig();

        if (!string.IsNullOrEmpty(_configuration.ServiceUrl))
            config.ServiceURL = _configuration.ServiceUrl;

        return new AmazonSQSClient(config.CreateCredentials(_configuration), config);
    }
}
