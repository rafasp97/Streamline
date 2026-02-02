namespace Streamline.Infrastructure.Messaging.RabbitMq
{
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }

        public RabbitMqSettings(string rabbitConnection)
        {
            ConnectionString = rabbitConnection ?? "amqp://admin:admin@localhost:5672/";
        }
    }
}
