using System;

namespace CP.Pedidos.Domain.Adapters.MessageBus;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string topicOrQueue);
}
