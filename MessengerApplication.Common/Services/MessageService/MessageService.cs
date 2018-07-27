using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MessengerApplication.Common.Models;
using MessengerApplication.Common.Services.ConsoleWrapper.Interface;
using MessengerApplication.Common.Services.MessageService.Interface;

namespace MessengerApplication.Common.Services.MessageService
{
    /// <summary>
    /// Message Service
    /// </summary>
    public class MessageService : IMessageService
    {
        private ConnectionFactory _connectionFactory;
        private IConnection connection;
        private IModel model;
        private EventingBasicConsumer consumer;
        public IConsole _console;
        private IConfigurationRoot _configuration;

        private string queueName;
        private string exchangeName;
        private string routeKey;

        public MessageService(IConfigurationRoot configuration, IConsole console, ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _console = console;
            _configuration = configuration;
            Initialise();
        }

        public void Initialise()
        {
            connection = _connectionFactory.CreateConnection();
            routeKey = _configuration["RoutingKey"];
            queueName = _configuration["QueueName"];
            exchangeName = _configuration["ExchangeName"];
        }

        public void CreateQueue()
        {
            model = connection.CreateModel();
            model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            model.QueueDeclare(queueName, false, false, false, null);
            model.QueueBind(queueName, exchangeName, routeKey, null);
            _console.WriteLine("Queue Created.");
        }

        public void CreateConsumer()
        {
            consumer = new EventingBasicConsumer(model);
            consumer.Received += (channel, eventArgs) =>
            {
                // Validate route key (for the ability to respond back)
                if (eventArgs.Body != null && eventArgs.RoutingKey == routeKey)
                {
                    var body = JsonConvert.DeserializeObject<MessageModel>(Encoding.Default.GetString(eventArgs.Body));
                    _console.WriteLine($"Hello { body.MessageBody }, I am your father!");
                    model.BasicAck(eventArgs.DeliveryTag, false);
                }
            };
        }

        public void SendMessage(MessageModel message)
        {
            var messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            var props = model.CreateBasicProperties();
            props.ContentType = "application/json";
            props.DeliveryMode = 2;
            model.BasicPublish(exchangeName,
                               routeKey,
                               props,
                               messageBodyBytes);
            _console.WriteLine("Message Sent.");
        }

        public void Listen()
        {
            var consumerTag = model.BasicConsume(queueName, false, consumer);
            _console.WriteLine($"Tag : {consumerTag}");
        }
    }
}