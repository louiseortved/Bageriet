using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.VisualBasic.ApplicationServices;

namespace Bageriet.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Required]
        public string Content { get; set; }

      
        public int? NewsId { get; set; }
        public News News { get; set; }

        public int? ProductId { get; set; }
        public Product Product{ get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}