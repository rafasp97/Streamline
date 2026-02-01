using Streamline.Domain.Entities.Products;
using Streamline.Domain.Entities;

namespace Streamline.Domain.Entities.Orders
{
    public class OrderProduct : Base
    {
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public Order Order { get; private set; }
        public int ProductId { get; private set; }
        public Product Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Subtotal { get; private set; }

        protected OrderProduct()
        {
            Order = null!;
            Product = null!;
        }

        public OrderProduct(Order order, Product product, int quantity)
        {
            Order = order;
            Product = product;
            ProductId = product.Id;
            UnitPrice = product.Price;
            Quantity = quantity;
            Subtotal = CalculateSubtotal();
        }

        private decimal CalculateSubtotal() => Quantity * UnitPrice;

        public void UpdateQuantity(int quantity)
        {
            if (quantity < 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");
            
            if(quantity == 0)
                Delete();
        
            Quantity = quantity;
            CalculateSubtotal();
        }

        public void Delete()
        {
            MarkAsDeleted();
        }
    }
}
