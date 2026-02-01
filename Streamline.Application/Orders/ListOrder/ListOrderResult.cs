using Streamline.Application.Orders;

namespace Streamline.Application.Orders.ListOrder
{
    public class ListOrderResult
    {
        public List<OrderResult> Orders { get; set; } = new();
    }
}
