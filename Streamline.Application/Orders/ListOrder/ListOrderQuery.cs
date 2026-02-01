using MediatR;
using Streamline.Domain.Enums;

namespace Streamline.Application.Orders.ListOrder
{
    public class ListOrderQuery : IRequest<ListOrderResult>
    {
        public EStatusOrder? Status { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
