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

            if (quantity < 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var productInOrder = GetProductInOrder(product.Id);

            if (productInOrder != null)
            {
                productInOrder.UpdateQuantity(quantity);
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

            if (quantity < 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var productInOrder = GetProductInOrder(product.Id);

            if(productInOrder == null)
                throw new InvalidOperationException("Product not found.");

            productInOrder.UpdateQuantity(quantity);
            RecalculateTotal();

            if (_orderProduct.All(p => p.DeletedAt != null) || Total == 0)
                MarkAsDeleted(); 
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

        public void StartProcessing()
        {
            if (Status != EStatusOrder.Pending && Status != EStatusOrder.Failed)
                throw new InvalidOperationException("Only pending or failed orders can be processing.");

            if(IsDeleted)
                throw new InvalidOperationException("Cannot pay a deleted order.");
            
            ValidateCanBeProcessingBasedOnStock();

            Status = EStatusOrder.Processing;
        }

        private void ValidateCanBeProcessingBasedOnStock() {

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
        }

        public void Pay()
        {
            if (Status != EStatusOrder.Processing)
                throw new InvalidOperationException("Only processing orders can be paid.");
            
            ConsumeProductStock();

            Status = EStatusOrder.Processing;
        }

        private void ConsumeProductStock() {

            var listByOrderProduct = _orderProduct.Where(p => p.DeletedAt == null);

            foreach (var item in listByOrderProduct)
            {
                item.Product.ConsumeStock(item.Quantity);
            }
        }

        public void Fail()
        {
            if (Status != EStatusOrder.Processing)
                throw new InvalidOperationException("Only processing orders can be Failed.");

            Status = EStatusOrder.Failed;
        }

        public void Process()
        {
            if (Status != EStatusOrder.Paid)
                throw new InvalidOperationException("Only paid orders can be processed.");

            Status = EStatusOrder.Processed;
        }

        public void Ship()
        {
            if (Status != EStatusOrder.Processed)
                throw new InvalidOperationException("Only Processed orders can be shipped.");

            Status = EStatusOrder.Shipped;
        }

        public void Completed()
        {
            if (Status != EStatusOrder.Shipped)
                throw new InvalidOperationException("Only shipped orders can be completed.");

            Status = EStatusOrder.Completed;
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

        public void CheckIfCanUpdateProducts()
        {
            if(Status != EStatusOrder.Pending) 
                throw new InvalidOperationException("Only pending orders can be update products.");
        }

    }
}
