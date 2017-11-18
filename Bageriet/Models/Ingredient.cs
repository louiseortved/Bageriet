using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bageriet.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string  Amount { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        //public virtual ICollection<Product> Products { get; set; }
    }



}