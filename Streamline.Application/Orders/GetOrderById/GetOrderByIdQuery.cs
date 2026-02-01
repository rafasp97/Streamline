using MediatR;
using Streamline.Domain.Enums;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderResult>
    {
        public int Id { get; set; }
    }
}
