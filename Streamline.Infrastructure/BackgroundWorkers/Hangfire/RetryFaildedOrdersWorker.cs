using Streamline.Infrastructure.Persistence.SqlServer.Repositories;
using Streamline.Infrastructure.BackgroundWorkers.Workers;

namespace Streamline.Infrastructure.BackgroundWorkers.Hangfire
{
    public class RetryFaildedOrdersWorker
    {
        private readonly OrderRepository _orderRepository;
        private readonly OrderProcessingWorker _orderProcessingWorker;

        public RetryFaildedOrdersWorker(OrderRepository orderRepository, OrderProcessingWorker orderProcessingWorker)
        {
            _orderRepository = orderRepository;
            _orderProcessingWorker = orderProcessingWorker;
        }

        public async Task Run()
        {
            var failedOrders = await _orderRepository.GetFailedOrders();

            if(failedOrders == null)
                return;

            Console.WriteLine($"[RetryFaildedOrdersWorker] Rodando: {DateTime.Now}. Pedidos falhados: {failedOrders.Count}");
            Console.WriteLine($"{failedOrders}");

            foreach (var order in failedOrders)
            {
                order.StartProcessing();
                await _orderRepository.Update(order);

                await _orderProcessingWorker.Execute(order.Id);
            }
        }
    }
}
