using System;
using System.ComponentModel.DataAnnotations;

namespace Bageriet.Models
{
    public class Mail
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

     
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool IsNew { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}