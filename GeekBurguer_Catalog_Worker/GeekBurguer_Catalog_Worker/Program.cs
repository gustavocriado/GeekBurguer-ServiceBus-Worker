using GeekBurguer_Catalog_Model.ClientUtils;
using GeekBurguer_Catalog_Model.Models;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace GeekBurguer_SendCatalog_Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
             SendMessagesAsync(new Catalog(), new ServiceBusClient(false)).GetAwaiter().GetResult();
        }

        private static async Task SendMessagesAsync(Catalog catalog, ServiceBusClient serviceBus) 
        {
            var message = Encoding.UTF8.GetBytes(catalog.CatalogProduct);

            var sendTask = serviceBus._queueClient.SendAsync(new Message(message));
            await sendTask;
            serviceBus.CheckCommunicationExceptions(sendTask);

            var closeTask = serviceBus._queueClient.CloseAsync();
            await closeTask;
            serviceBus.CheckCommunicationExceptions(closeTask);
        }
    }
}
