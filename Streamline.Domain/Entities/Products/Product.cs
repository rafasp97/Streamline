using Streamline.Domain.Entities;

namespace Streamline.Domain.Entities.Products
{
    public class Product : Base
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public string? Description { get; private set; }
        public int StockQuantity { get; private set; }
        public bool Active { get; private set; }

        protected Product()
        {
            Name = null!;
        }

        public Product(
            string name,
            decimal price,
            int stockQuantity,
            bool active,
            string? description = null)
        {
            Name = name;
            Price = price;
            StockQuantity = stockQuantity;
            Active = active;
            Description = description;
        }


        public void ConsumeStock(int quantity)
        {
            var updatedStock = StockQuantity - quantity;

            if (updatedStock <= 0)
                throw new InvalidOperationException("Stock quantity cannot be negative.");

            if(updatedStock == 0) 
                Deactivate();

            else {
                StockQuantity = updatedStock;
                Activate();
            };
        }

        public void Activate() => Active = true;

        public void Deactivate() {
            Active = false;
            //TODO: adicionar lógica para retirar de todos pedidos
        }
    }
}
