namespace Bageriet.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsSubscribed { get; set; } = true;
    }
}