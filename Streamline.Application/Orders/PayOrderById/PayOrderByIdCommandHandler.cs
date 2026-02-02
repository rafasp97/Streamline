using MediatR;
using Streamline.Application.Repositories;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.PayOrderById
{
    public class PayOrderByIdCommandHandler
        : IRequestHandler<PayOrderByIdCommand, OrderResult>
    {
        private readonly IOrderRepository _orderRepository;

        public PayOrderByIdCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResult> Handle(PayOrderByIdCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetById(request.Id)
                ?? throw new InvalidOperationException("Order not found.");
            
            order.Pay();
            await _orderRepository.Update(order);

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
