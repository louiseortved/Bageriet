using System;

namespace Bageriet.Models
{
    public class Contact
    {
        public int Id { get; set; }
     
        public DateTime DateCreated { get; set; } = DateTime.Now;
       
        public string Content { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool IsNew { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}