using MediatR;
using Streamline.Application.Interfaces.Repositories;
using Streamline.Domain.Entities.Orders;
using Streamline.Domain.Entities.Products;
using Streamline.Domain.Enums;
using Streamline.Application.Orders;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Streamline.Application.Orders.UpdateOrderById
{
    public class UpdateOrderByIdCommandHandler 
        : IRequestHandler<UpdateOrderByIdCommand, UpdateOrderByIdResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogRepository _logger;

        public UpdateOrderByIdCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logRepository;
        }

        public async Task<UpdateOrderByIdResult> Handle(
            UpdateOrderByIdCommand request,
            CancellationToken cancellationToken)
        {
            await _logger.Low($"Update order process started for OrderId = {request.OrderId}.");

            var order = await _orderRepository.GetById(request.OrderId);

            if (order == null)
            {
                await _logger.Medium($"Update failed: Order with Id = {request.OrderId} not found.");
                throw new InvalidOperationException("Order not found.");
            }

            order.CheckIfCanUpdateProducts();
            await _logger.Low("Order is eligible for product updates.");

            foreach (var item in request.Products)
            {
                var product = await _productRepository.GetById(item.ProductId);

                if (product == null)
                {
                    await _logger.Medium($"Update failed: Product with Id = {item.ProductId} not found.");
                    throw new InvalidOperationException($"Product {item.ProductId} not found.");
                }

                if (!product.EnsureSufficientStock(item.Quantity))
                {
                    await _logger.Medium($"Update failed: Insufficient stock for product '{product.Name}' (ProductId = {product.Id}).");
                    throw new InvalidOperationException($"Not enough stock for product '{product.Name}'.");
                }

                var orderProduct = order.OrderProduct
                    .FirstOrDefault(op => op.Product.Id == item.ProductId);
                
                handleUpdateOrderProducts(item, order, product);
                await _logger.Low($"Product updated in order: {product.Name}, New Quantity = {item.Quantity}, UnitPrice = {product.Price}.");
            }

            RemoveProductsNotInRequest(order, request);
            await _logger.Low("Removed products from order that were not included in the update request.");


            await _orderRepository.Update(order);
            await _logger.Low($"Order updated successfully. OrderId = {order.Id}");

            return new UpdateOrderByIdResult
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                Total = order.Total,
                CreatedAt = order.CreatedAt,
                Products = order.OrderProduct
                    .Where(orderProduct => orderProduct.DeletedAt == null)
                    .Select(orderProduct => new ProductResult
                    {
                        Name = orderProduct.Product.Name,
                        UnitPrice = orderProduct.UnitPrice,
                        Quantity = orderProduct.Quantity,
                        Subtotal = orderProduct.Subtotal
                    })
                    .ToList()
            };
        }

        public void handleUpdateOrderProducts(UpdateOrderProductCommand item, Order order, Product product) {
            var orderProduct = order.OrderProduct
                .FirstOrDefault(op => op.Product.Id == item.ProductId);

            if (orderProduct == null || (item.Quantity > orderProduct.Quantity)){
                order.AddProduct(product, item.Quantity);
            } else {
                order.RemoveProduct(product, item.Quantity);
            }
        }

        public void RemoveProductsNotInRequest(Order order, UpdateOrderByIdCommand request) {
            var requestProductIds = request.Products
                .Select(p => p.ProductId)
                .ToHashSet();

            var orderProductsToRemove = order.OrderProduct
                .Where(op => op.DeletedAt == null && !requestProductIds.Contains(op.Product.Id))
                .ToList();

            foreach (var orderProduct in orderProductsToRemove)
            {
                order.RemoveProduct(orderProduct.Product, 0);
            }
        }
    }
}
