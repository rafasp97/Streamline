using MediatR;
using Streamline.Application.Interfaces.Repositories;
using Streamline.Domain.Entities.Orders;
using Streamline.Domain.Entities.Products;
using Streamline.Application.Orders;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Streamline.Application.Orders.DeleteOrderById
{
    public class DeleteOrderByIdCommandHandler 
        : IRequestHandler<DeleteOrderByIdCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogRepository _logger;

        public DeleteOrderByIdCommandHandler(IOrderRepository orderRepository, ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _logger = logRepository;
        }

        public async Task<Unit> Handle(
            DeleteOrderByIdCommand request,
            CancellationToken cancellationToken)
        {
            await _logger.High($"Deletion process started for OrderId = {request.Id}.");

            var order = await _orderRepository.GetById(request.Id);

            if(order == null)
            {
                await _logger.Medium($"Deletion failed: OrderId = {request.Id} not found.");
                throw new InvalidOperationException("Order not found.");
            }

            order.Delete();
            await _orderRepository.Update(order);

            await _logger.High($"Deletion process completed for OrderId = {request.Id}.");

            return Unit.Value;
        }
    }
}
