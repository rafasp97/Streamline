namespace Streamline.Application.Results
{
    public class ListOrderResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public List<OrderResult> Orders { get; set; } = new();
    }
}
