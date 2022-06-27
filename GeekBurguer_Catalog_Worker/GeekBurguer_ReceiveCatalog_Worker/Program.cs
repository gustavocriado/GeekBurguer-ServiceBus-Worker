using GeekBurguer_Catalog_Model.ClientUtils;

namespace GeekBurguer_ReceiveCatalog_Worker
{
    public class Program
    {
        private static ServiceBusClient _client = new ServiceBusClient(true);

        public static void Main(string[] args)
        {
            _client.RegisterHandleServiceBus();
        }

    }
}