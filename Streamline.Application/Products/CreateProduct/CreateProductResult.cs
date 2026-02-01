namespace Streamline.Application.Products.CreateProduct
{
    public class CreateProductResult
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool Active { get; set; }
        public string? Description { get; set; }
    }
}
