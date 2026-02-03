using MediatR;
using Streamline.Application.Interfaces.Repositories;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.GetOrderById
{
    public class GetOrderByIdQueryHandler
        : IRequestHandler<GetOrderByIdQuery, OrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogRepository _logger;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _logger = logRepository;
        }

        public async Task<OrderResult> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {

            await _logger.Low($"Retrieving order details for OrderId = {request.Id}.");

            var order = await _orderRepository.GetById(request.Id);

            if (order == null)
            {
                await _logger.Medium($"Order retrieval failed: Order with Id = {request.Id} not found.");
                throw new InvalidOperationException("Order not found.");
            };

            await _logger.Low($"Order retrieval completed successfully. OrderId = {request.Id}.");

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
