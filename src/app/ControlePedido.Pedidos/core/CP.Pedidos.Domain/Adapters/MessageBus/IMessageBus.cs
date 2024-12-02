namespace CP.Pedidos.Domain.Adapters.MessageBus;

public class MessageResult<T>
{
    public T data { get; private set; }
    public string Id { get; private set; }

    public MessageResult(T data, string id)
    {
        this.data = data;
        Id = id;
    }
}

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string topicOrQueue);
    Task<IEnumerable<MessageResult<T>>> ReceiveMessagesAsync<T>(string topicOrQueue, int maxMessages = 10, int waitTimeSeconds = 5);
    Task DeleteMessage(string topicOrQueue, string messageId);
}
