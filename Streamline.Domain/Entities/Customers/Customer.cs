using BrazilianDocs;

namespace Streamline.Domain.Entities.Customers
{
    public class Customer : Base
    {
        public int Id { get; private set; } 
        public string Name { get; private set; }
        public string Document { get; private set; }
        public CustomerAddress Address { get; private set; }
        public CustomerContact Contact { get; private set; }

        protected Customer()
        {
            Name = null!;
            Document = null!;
            Address = null!;
            Contact = null!;
        }

        public Customer(
            string name,
            string document,
            string phone,
            string email,
            string neighborhood,
            int number,
            string city,
            string state,
            string? complement = null)
        {
            Name = name; 
            Document = SetDocument(document); 
            Contact = new CustomerContact(this, phone, email);
            Address = new CustomerAddress(this, neighborhood, number, city, state, complement);
        }

        private string SetDocument(string document)
        {
            if (!Cpf.IsValid(document))
                throw new InvalidOperationException("Document must be a valid CPF.");

            return document;
        }
    }
}
