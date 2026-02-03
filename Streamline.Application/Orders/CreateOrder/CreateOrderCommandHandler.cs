using MediatR;
using Streamline.Application.Interfaces.Repositories;
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
        private readonly ILogRepository _logger;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _logger = logRepository;
        }

        public async Task<OrderResult> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            await _logger.Low($"Order creation process started for CustomerId = {request.CustomerId}.");

            var customer = await _customerRepository.GetById(request.CustomerId);

            if (customer == null)
            {
                await _logger.Medium($"Order creation failed: Customer with Id = {request.CustomerId} not found.");
                throw new InvalidOperationException("Customer not found.");
            }

            var order = new Order(customer);

            foreach (var item in request.Products)
            {
                var product = await _productRepository.GetById(item.ProductId);

                if (product == null)
                {
                    await _logger.Medium($"Order creation failed: Product with Id = {item.ProductId} not found.");
                    throw new InvalidOperationException($"Product {item.ProductId} not found.");
                }
                
                if (!product.EnsureSufficientStock(item.Quantity))
                {
                    await _logger.Medium($"Order creation failed: Insufficient stock for product '{product.Name}' (ProductId = {product.Id}).");
                    throw new InvalidOperationException($"Not enough stock for product '{product.Name}'.");
                }

                order.AddProduct(product, item.Quantity);

                await _logger.Low($"Product added to order: {product.Name}, Quantity = {item.Quantity}.");
            }

            _orderRepository.Add(order);
            await _orderRepository.SaveChanges();

            await _logger.Low($"Order created successfully. OrderId = {order.Id}.");

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
                    UnitPrice = orderProduct.UnitPrice,
                    Quantity = orderProduct.Quantity,
                    Subtotal = orderProduct.Subtotal
                }).ToList(),
                Total = order.Total,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
