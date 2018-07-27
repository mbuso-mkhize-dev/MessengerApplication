using System;
using Unity;
using Unity.Lifetime;
using MessengerApplication.Common.DependencyInjection;
using MessengerApplication.Common.Services.MessageService.Interface;

namespace MessengerApplication.Receiver
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello there!");
            Console.WriteLine("Waiting for messages ... ");

            var container = new UnityContainer();
            DIContainerHelper.RegisterComponents<HierarchicalLifetimeManager>(container);

            var messageService = container.Resolve<IMessageService>();
            messageService.CreateQueue();
            messageService.CreateConsumer();
            messageService.Listen();
        }
    }
}