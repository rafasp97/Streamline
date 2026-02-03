using MediatR;
using Streamline.Domain.Enums;
using Streamline.Application.Results;

namespace Streamline.Application.Queries
{
    public class ListOrderQuery : IRequest<ListOrderResult>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public EStatusOrder? Status { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
