using Streamline.Domain.Entities;
using Streamline.Domain.Entities.Customers;
using Streamline.Domain.Entities.Orders;
using Streamline.Domain.Entities.Products;
using Streamline.Domain.Enums;

namespace Streamline.Domain.Entities.Orders
{
    public class Order : Base
    {
        public int Id { get; private set; }
        public EStatusOrder Status { get; private set; }
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        private readonly List<OrderProduct> _orderProduct = new();
        public IReadOnlyCollection<OrderProduct> OrderProduct => _orderProduct.AsReadOnly();
        public decimal Total { get; private set; }

        protected Order()
        {
            Customer = null!;
        }

        public Order(Customer customer)
        {
            Customer = customer;
            CustomerId = customer.Id;
            Status = EStatusOrder.Pending;
            Total = 0;
        }

        public void AddProduct(Product product, int quantity)
        {
            if (Status != EStatusOrder.Pending)
                throw new InvalidOperationException(
                    "Only pending orders can be add products.");

            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var productInOrder = GetProductInOrder(product.Id);

            if (productInOrder != null)
            {
                productInOrder.UpdateQuantity(productInOrder.Quantity + quantity);
                RecalculateTotal();
                return;
            }

            _orderProduct.Add(
                new OrderProduct(this, product, quantity)
            );
            RecalculateTotal();
        }
    
        public void RemoveProduct(Product product, int quantity)
        {
            if (Status != EStatusOrder.Pending)
                throw new InvalidOperationException(
                    "Only pending orders can be add products.");

            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var productInOrder = GetProductInOrder(product.Id);

            if(productInOrder == null)
                throw new InvalidOperationException("Product not found.");

            productInOrder.UpdateQuantity(productInOrder.Quantity - quantity);
            RecalculateTotal();
            return;
        }

        private OrderProduct? GetProductInOrder(int productId)
        {
            return _orderProduct.FirstOrDefault(p => p.ProductId == productId && p.DeletedAt == null);
        }

        private void RecalculateTotal()
        {
            Total = _orderProduct
                        .Where(p => p.DeletedAt == null)
                        .Sum(p => p.Subtotal);
        }

        public void Pay()
        {
            if (Status != EStatusOrder.Pending)
                throw new InvalidOperationException("Only pending orders can be paid.");

            if(IsDeleted)
                throw new InvalidOperationException("Cannot pay a deleted order.");
            
            ValidateIfCanBePayAndConsumeProductStock();

            Status = EStatusOrder.Paid;
        }

        private void ValidateIfCanBePayAndConsumeProductStock() {

            var listByOrderProduct = _orderProduct.Where(p => p.DeletedAt == null);

            foreach (var item in listByOrderProduct)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product '{item.Product.Name}'. Requested: {item.Quantity}, Available: {item.Product.StockQuantity}"
                    );
                }
            }

            foreach (var item in listByOrderProduct)
            {
                item.Product.ConsumeStock(item.Quantity);
            }
        }

        public void Process()
        {
            if (Status != EStatusOrder.Paid)
                throw new InvalidOperationException("Only paid orders can be processed.");

            Status = EStatusOrder.Processing;
        }

        public void Ship()
        {
            if (Status != EStatusOrder.Processing)
                throw new InvalidOperationException("Only Processing orders can be shipped.");

            Status = EStatusOrder.Processing;
        }

        public void Completed()
        {
            if (Status != EStatusOrder.Shipped)
                throw new InvalidOperationException("Only shipped orders can be completed.");

            Status = EStatusOrder.Processing;
        }

        public void Cancel()
        {
            if (Status != EStatusOrder.Paid)
                throw new InvalidOperationException("Only paid orders can be cancelled.");

            Status = EStatusOrder.Cancelled;
        }

        public void Delete()
        {
            if (Status != EStatusOrder.Pending)
                throw new InvalidOperationException("Only pending orders can be deleted.");

            MarkAsDeleted();
        }

    }
}
