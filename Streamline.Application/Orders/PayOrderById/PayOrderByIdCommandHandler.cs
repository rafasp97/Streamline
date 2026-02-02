using MediatR;
using Streamline.Application.Repositories;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.PayOrderById
{
    public class PayOrderByIdCommandHandler
        : IRequestHandler<PayOrderByIdCommand, OrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogRepository _logger;

        public PayOrderByIdCommandHandler(IOrderRepository orderRepository, ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _logger = logRepository;
        }

        public async Task<OrderResult> Handle(PayOrderByIdCommand request, CancellationToken cancellationToken)
        {
            await _logger.Low($"Payment process started for OrderId = {request.Id}.");

            var order = await _orderRepository.GetById(request.Id);

            if(order == null)
            {
                await _logger.Medium($"Payment failed: OrderId = {request.Id} not found.");
                throw new InvalidOperationException("Order not found.");
            }

            order.StartProcessing();

            await _orderRepository.Update(order);

            //CHAMAR O JOB PARA PROCESSAR O PEDIDO...

            await _logger.Low($"Payment process completed for OrderId = {request.Id}.");

            return new OrderResult
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                Customer = new CustomerResult
                {
                    Name = order.Customer.Name,
                    Email = order.Customer.Contact.Email,
                    Phone = order.Customer.Contact.Phone
                },
                Products = order.OrderProduct.Select(orderProduct => new ProductResult
                {
                    Name = orderProduct.Product.Name,
                    UnitPrice = orderProduct.UnitPrice
                }).ToList(),
                Total = order.Total,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
