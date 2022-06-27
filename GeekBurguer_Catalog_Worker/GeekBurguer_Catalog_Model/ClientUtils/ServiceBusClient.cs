using Microsoft.Azure.ServiceBus;
using System.Text;

namespace GeekBurguer_Catalog_Model.ClientUtils
{
    public class ServiceBusClient
    {
        private readonly static string QueueConnectionString = @"";
        private readonly static string QueuePath = "";
        public IQueueClient _queueClient;


        public ServiceBusClient(bool isReceive)
        {
            if (!isReceive) 
                _queueClient = new QueueClient(QueueConnectionString, QueuePath);
            else
                _queueClient = new QueueClient(QueueConnectionString, QueuePath, ReceiveMode.PeekLock);

            _queueClient.OperationTimeout = TimeSpan.FromSeconds(10);
        }

        public async Task RegisterHandleServiceBus()
        {
            _queueClient.RegisterMessageHandler(MessageHandler, new MessageHandlerOptions(ExceptionHandler) { AutoComplete = false });
            Console.ReadLine();

            var closeTask = _queueClient.CloseAsync();
            await closeTask;
            CheckCommunicationExceptions(closeTask);
        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionArgs.Exception}.");
            Console.WriteLine($"Endpoint:{exceptionArgs.ExceptionReceivedContext.Endpoint}, Path:{exceptionArgs.ExceptionReceivedContext.EntityPath}, Action:{exceptionArgs.ExceptionReceivedContext.Action}");
            return Task.CompletedTask;
        }

        private async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(message.Body)} ");

            if (cancellationToken.IsCancellationRequested || _queueClient.IsClosedOrClosing)
                return;

        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public bool CheckCommunicationExceptions(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList()
                .ForEach(innerException =>
                {
                    Console.WriteLine($"Error in SendAsync task: { innerException.Message}. Details: { innerException.StackTrace}");
        

                if (innerException is ServiceBusCommunicationException)
                        Console.WriteLine("Connection Problem with Host");
                });

            return false;
        }



    }
}
