using MediatR;
using Streamline.Application.Repositories;
using Streamline.Domain.Entities.Orders;
using Streamline.Domain.Entities.Products;
using Streamline.Application.Orders;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Streamline.Application.Orders.CreateOrder
{
    public class CreateOrderCommandHandler 
        : IRequestHandler<CreateOrderCommand, OrderResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderResult> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetById(request.CustomerId)
                ?? throw new InvalidOperationException("Customer not found.");

            var order = new Order(customer);

            foreach (var item in request.Products)
            {
                var product = await _productRepository.GetById(item.ProductId)
                    ?? throw new InvalidOperationException($"Product {item.ProductId} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Not enough stock for product '{product.Name}'."
                    );

                order.AddProduct(product, item.Quantity);
            }

            _orderRepository.Add(order);
            await _orderRepository.SaveChangesAsync();

            return new OrderResult
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                Customer = new CustomerResult
                {
                    Name = order.Customer.Name,
                    Email = order.Customer.Contact.Email,
                    Phone = order.Customer.Contact.Phone
                },
                Products = order.OrderProduct.Select(orderProduct => new ProductResult
                {
                    Name = orderProduct.Product.Name,
                    UnitPrice = orderProduct.UnitPrice
                }).ToList(),
                Total = order.Total,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
