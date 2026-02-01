using MediatR;
using Streamline.Application.Repositories;

namespace Streamline.Application.Orders.ListOrder
{
    public class ListOrderQueryHandler
        : IRequestHandler<ListOrderQuery, ListOrderResult>
    {
        private readonly IOrderRepository _orderRepository;

        public ListOrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<ListOrderResult> Handle(ListOrderQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAll(
                request.Status,
                request.CustomerId,
                request.CreatedFrom,
                request.CreatedTo
            );
            
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
