namespace CoffeShop.Domain.Entities
{
    public class Client : Base
    {
        public int Id { get; set; }           
        public string Name { get; set; }
        public string Document { get; set; } 
    }
}