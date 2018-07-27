using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitMQ.Client;
using System;
using MessengerApplication.Common.Models;
using MessengerApplication.Common.Services.ConsoleWrapper.Interface;
using MessengerApplication.Common.Services.MessageService;
using MessengerApplication.Common.Services.MessageService.Interface;

namespace MessengerApplication.Tests
{
    [TestClass]
    public class MessengerUnitTests
    {
        private MessageModel _messageModel;
        private IMessageService _messageService;
        private Mock<IConsole> _mockConsole;
        private Mock<IConfigurationRoot> _mockConfig;
        private Mock<ConnectionFactory> _mockConnectionFactory;
        private Guid _id;
        private string _message;
        private string _sender;
        private DateTime _sentAt;
        private string _exchangeName;
        private string _queueName;
        private string _routeKey;

        [TestInitialize]
        public void SetupTests()
        {
            _id = Guid.NewGuid();
            _message = "TestMessage";
            _queueName = "TestQueue";
            _routeKey = "TestRouteKey";
            _exchangeName = "TestExchange";
            _sentAt = DateTime.Now;
            _sender = "TestSender";

            _messageModel = new MessageModel
            {
                Id = _id,
                MessageBody = _message,
                Sender = _sender,
                SentAt = _sentAt
            };
            _mockConsole = new Mock<IConsole>();
            _mockConsole.SetupAllProperties();

            _mockConfig = new Mock<IConfigurationRoot>();
            _mockConfig.SetupGet(x => x[It.IsAny<string>()]).Returns("string value");

            _mockConnectionFactory = new Mock<ConnectionFactory>();
            _mockConnectionFactory.SetupAllProperties();
            var connection = new Mock<IConnection>();
            var model = new Mock<IModel>();
            model.Setup(c => c.CreateBasicProperties()).Returns((new Mock<IBasicProperties>()).Object);
            connection.Setup(c => c.CreateModel()).Returns(model.Object);
            _mockConnectionFactory.Setup(c => c.CreateConnection()).Returns(connection.Object);

            _messageService = new MessageService(_mockConfig.Object, _mockConsole.Object, _mockConnectionFactory.Object);
        }

        [TestMethod]
        public void BroadcastTest()
        {
            _messageService.CreateQueue();
            _mockConsole.Verify(m => m.WriteLine(It.Is<string>(c => c == "Queue Created.")), Times.Once);
            _messageService.SendMessage(_messageModel);
            _mockConsole.Verify(m => m.WriteLine(It.Is<string>(c => c.Contains("Message Sent"))), Times.Once);
        }

        [TestMethod]
        public void ReceiveTest()
        {
            _messageService.CreateQueue();
            _mockConsole.Verify(m => m.WriteLine(It.Is<string>(c => c == "Queue Created.")), Times.Once);
            _messageService.CreateConsumer();
            _messageService.Listen();
            _mockConsole.Verify(m => m.WriteLine(It.Is<string>(c => c.Contains("Tag"))), Times.Once);
        }
    }
}