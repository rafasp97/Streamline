namespace Streamline.Domain.Entities.Customers
{
    public class CustomerAddress
    {
        public int CustomerId { get; private set; }
        public string Neighborhood { get; private set; }
        public int Number { get; private set; }
        public string? Complement { get; set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public Customer Customer { get; private set; }

        protected CustomerAddress()
        {
            Neighborhood = null!;
            City = null!;
            State = null!;
            Customer = null!;
        }

        public CustomerAddress(Customer customer, string neighborhood, int number, string city, string state, string? complement = null)
        {
            Customer = customer;
            Neighborhood = neighborhood; 
            Number = number;             
            City = city;                
            State = state.ToUpper();    
            Complement = complement;
        }

    }
}
