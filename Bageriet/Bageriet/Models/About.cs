namespace Bageriet.Models
{
    public class About
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }


        public string Content { get; set; }
    }
}