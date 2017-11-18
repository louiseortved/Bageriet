using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.ApplicationServices;


namespace Bageriet.Models
{
    public class News
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string ImageUrl { get; set; }



        public virtual ICollection<Comment> Comments { get; set; }
    }
}