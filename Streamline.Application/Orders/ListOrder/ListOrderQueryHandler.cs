using MediatR;
using Streamline.Application.Interfaces.Repositories;

namespace Streamline.Application.Orders.ListOrder
{
    public class ListOrderQueryHandler
        : IRequestHandler<ListOrderQuery, ListOrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogRepository _logger;

        public ListOrderQueryHandler(IOrderRepository orderRepository, ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _logger = logRepository;
        }

        public async Task<ListOrderResult> Handle(ListOrderQuery request, CancellationToken cancellationToken)
        {

            await _logger.Low(
                "Order listing initiated with filters: " +
                $"Status = {request.Status}, " +
                $"CustomerId = {request.CustomerId}, " +
                $"CreatedFrom = {request.CreatedFrom}, " +
                $"CreatedTo = {request.CreatedTo}."
            );
            
            var orders = await _orderRepository.GetAll(
                request.Status,
                request.CustomerId,
                request.CreatedFrom,
                request.CreatedTo
            );

            await _logger.Low("Order listing query completed successfully.");
            
            return new ListOrderResult
            {
                Orders = orders.Select(order => new OrderResult
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
                        UnitPrice = orderProduct.UnitPrice,
                        Quantity = orderProduct.Quantity,
                        Subtotal = orderProduct.Subtotal,
                    }).ToList(),
                    Total = order.Total,
                    CreatedAt = order.CreatedAt
                }).ToList()
            };
        }
    }
}
