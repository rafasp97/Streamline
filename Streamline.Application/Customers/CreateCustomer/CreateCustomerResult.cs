namespace Streamline.Application.Customers.CreateCustomer
{
    public class CreateCustomerResult
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        
    }
}
