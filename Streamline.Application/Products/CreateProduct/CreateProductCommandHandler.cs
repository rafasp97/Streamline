using Streamline.Application.Interfaces.Repositories;
using Streamline.Domain.Entities.Products;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Streamline.Application.Products.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogRepository _logger;

        public CreateProductCommandHandler(IProductRepository productRepository, ILogRepository logRepository)
        {
            _productRepository = productRepository;
            _logger = logRepository;
        }

        public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {

            await _logger.Low(
                "Product creation request started: " +
                $"Name = {request.Name}, Price = {request.Price}, StockQuantity = {request.StockQuantity}, Active = {request.Active}."
            );

            //TODO: adicionar regra de negócio (domain) para o nome do produto seja unique no banco.

            var product = new Product(
                request.Name,
                request.Price,
                request.StockQuantity,
                request.Active,
                request.Description
            );

            _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();

            await _logger.Low($"Product created successfully. ProductId = {product.Id}.");

            return new CreateProductResult
            {
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Active = product.Active,
                Description = product.Description
            };
        }
    }
}
