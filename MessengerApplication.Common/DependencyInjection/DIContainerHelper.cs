using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.IO;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using MessengerApplication.Common.Services.ConsoleWrapper;
using MessengerApplication.Common.Services.ConsoleWrapper.Interface;
using MessengerApplication.Common.Services.MessageService;
using MessengerApplication.Common.Services.MessageService.Interface;

namespace MessengerApplication.Common.DependencyInjection
{
    public static class DIContainerHelper
    {
        public static void RegisterComponents<T>(UnityContainer container) where T : LifetimeManager, new()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            container.RegisterType<IConsole, ConsoleWrapper>(new T());
            container.RegisterType<IMessageService, MessageService>(new T());
            container.RegisterType<IConfigurationRoot>(new T(), new InjectionFactory(c =>
            {
                return configuration;
            }));
            container.RegisterType<ConnectionFactory>(new T(), new InjectionFactory(c =>
            {
                return new ConnectionFactory
                {
                    UserName = configuration["Username"],
                    Password = configuration["Password"],
                    HostName = configuration["Host"],
                    VirtualHost = configuration["VHost"]
                };
            }));
        }
    }
}