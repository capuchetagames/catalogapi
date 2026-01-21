using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using CatalogApi.Config;
using Core.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CatalogApi.Service;

public class RabbitMqService : IRabbitMqService
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IChannel _publishChannel;

    private readonly ConcurrentBag<IChannel> _consumerChannels = new();

    public RabbitMqService(RabbitMqSettings settings)
    {
        _settings = settings;

        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password,
            AutomaticRecoveryEnabled = true
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

        _publishChannel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken)
    {
        await _publishChannel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await _publishChannel.BasicPublishAsync(exchange, routingKey, body, cancellationToken: cancellationToken);
    }

    public async Task ConsumeAsync<T>(string exchange, string queue, string routingKey, Func<T, Task> handler, CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync(cancellationToken:cancellationToken);
        _consumerChannels.Add(channel);

        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, true, cancellationToken:cancellationToken);
        await channel.QueueDeclareAsync(queue, true, false, false, null, cancellationToken:cancellationToken);
        await channel.QueueBindAsync(queue, exchange, routingKey, cancellationToken:cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var message = JsonSerializer.Deserialize<T>(ea.Body.Span);
            await handler(message!);

            await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
        };

        await channel.BasicConsumeAsync(queue, false, consumer, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var channel in _consumerChannels)
            await channel.CloseAsync();

        await _publishChannel.CloseAsync();
        await _connection.CloseAsync();
    }
}
