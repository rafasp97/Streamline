using MediatR;
using Streamline.Domain.Enums;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderResult>
    {
        public required int CustomerId { get; set; }
        public required List<CreateOrderProductCommand> Products { get; set; }
    }

    public class CreateOrderProductCommand
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}
