using MediatR;
using Streamline.Domain.Enums;
using Streamline.Application.Orders;

namespace Streamline.Application.Orders.CancelOrderById
{
    public class CancelOrderByIdCommand : IRequest<OrderResult>
    {
        public int Id { get; set; }
    }
}