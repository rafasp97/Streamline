using MediatR;

namespace Streamline.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<CreateCustomerResult>
    {
        public required string Name { get; set; }
        public required string Document { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required string Neighborhood { get; set; }
        public required int Number { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string? Complement { get; set; }
    }
}
