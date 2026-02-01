namespace Streamline.Application.Orders
{
    public class OrderResult
    {
        public int Id { get; set; }
        public required string Status { get; set; }
        public required CustomerResult Customer { get; set; }
        public List<ProductResult> Products { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerResult
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }

    public class ProductResult
    {
        public required string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
