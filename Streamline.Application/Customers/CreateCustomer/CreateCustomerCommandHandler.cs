using Streamline.Application.Interfaces.Repositories;
using Streamline.Domain.Entities.Customers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Streamline.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogRepository _logger;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository, ILogRepository logRepository)
        {
            _customerRepository = customerRepository;
            _logger = logRepository;
        }

        public async Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {

            await _logger.Low(
                "Customer creation request started:" +
                $"Email = {request.Email}, Phone = {request.Phone}, Document = {request.Document}."
            );

            await ValidateCreation(request.Phone, request.Email, request.Document);

            var customer = new Customer(
                request.Name,
                request.Document,
                request.Phone,
                request.Email,
                request.Neighborhood,
                request.Number,
                request.City,
                request.State,
                request.Complement
            );

            _customerRepository.Add(customer);
            await _customerRepository.SaveChangesAsync();

            await _logger.Low($"Customer created successfully. CustomerId = {customer.Id}.");

            return new CreateCustomerResult
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Contact.Email
            };
        }

        private async Task ValidateCreation(string phone, string email, string document)
        {
            if (await _customerRepository.EmailExists(email))
            {
                await _logger.Medium($"Customer creation failed: email {email} already registered.");
                throw new InvalidOperationException("Email is already registered.");
            }

            if (await _customerRepository.PhoneExists(phone))
            {
                await _logger.Medium($"Customer creation failed: phone {phone} number already registered.");
                throw new InvalidOperationException("Phone number is already registered.");
            }

            if (await _customerRepository.DocumentExists(document))
            {
                await _logger.Medium($"Customer creation failed: document {document} already registered.");
                throw new InvalidOperationException("Document is already registered.");
            }
        }

    }
}
