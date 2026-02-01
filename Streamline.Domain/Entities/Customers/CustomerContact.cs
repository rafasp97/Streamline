namespace Streamline.Domain.Entities.Customers
{
    public class CustomerContact
    {
        public int CustomerId { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public Customer Customer { get; private set; }

        protected CustomerContact() 
        {
            Phone = null!;
            Email = null!;
            Customer = null!;
        } 

        public CustomerContact(Customer customer, string phone, string email)
        {
            Customer = customer;
            Phone = phone;
            Email = email; 
        }

    }
}
