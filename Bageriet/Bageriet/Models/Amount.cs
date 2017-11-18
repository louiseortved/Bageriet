using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bageriet.Models
{
    public class Amount
    {
        public int Id { get; set; }
        public string Title { get; set; }


        public virtual ICollection<Product> Products { get; set; }

        //public int? ProductId { get; set; }
        //public Product Product { get; set; }

    }
}