namespace CoffeShop.Domain.Entities
{
    public class Client : Base
    {
        public int CliendId { get; set; }           
        public string Neighborhood { get; set; }
        public number Number { get; set; } 
        public string Complement { get; set; }
        public string City { get; set; }
        public string Store { get; set; }
    }
}