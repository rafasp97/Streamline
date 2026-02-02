using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Streamline.Infrastructure.Messaging.RabbitMq
{
    public class RabbitMqPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher(RabbitMqSettings settings)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(settings.ConnectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task PublishProcessingOrder<T>(T message)
        {
            return PublishToQueue("processed-orders", message);
        }

        public Task PublishShippedOrder<T>(T message)
        {
            return PublishToQueue("shipped-orders", message);
        }

        public Task PublishCompletedOrder<T>(T message)
        {
            return PublishToQueue("completed-orders", message);
        }

        private Task PublishToQueue<T>(string queueName, T message)
        {
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublish("", queueName, null, body);

            return Task.CompletedTask;
        }
    }
}
