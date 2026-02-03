using MediatR;
using Streamline.Application.Interfaces.Repositories;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.CancelOrderById
{
    public class CancelOrderByIdCommandHandler
        : IRequestHandler<CancelOrderByIdCommand, OrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogRepository _logger;

        public CancelOrderByIdCommandHandler(IOrderRepository orderRepository, ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _logger = logRepository;
        }

        public async Task<OrderResult> Handle(CancelOrderByIdCommand request, CancellationToken cancellationToken)
        {
            await _logger.High($"Cancellation process started for OrderId = {request.Id}.");

            var order = await _orderRepository.GetById(request.Id);

            if(order == null)
            {
                await _logger.Medium($"Cancellation failed: OrderId = {request.Id} not found.");
                throw new InvalidOperationException("Order not found.");
            }
            
            order.Cancel();
            await _orderRepository.Update(order);

            await _logger.High($"Cancellation process completed for OrderId = {request.Id}.");

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
