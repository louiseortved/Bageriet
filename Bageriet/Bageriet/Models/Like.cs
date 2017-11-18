using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Bageriet.Models
{
    public class Like
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}