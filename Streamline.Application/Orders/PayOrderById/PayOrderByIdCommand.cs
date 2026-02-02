using MediatR;
using Streamline.Domain.Enums;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.PayOrderById
{
    public class PayOrderByIdCommand : IRequest<OrderResult>
    {
        public int Id { get; set; }
    }
}