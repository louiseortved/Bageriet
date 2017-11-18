using System;
using System.Collections.Generic;

namespace Bageriet.Models
{
    public class Product
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    
       
   
        public string ImageUrl { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }


        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    

        public virtual ICollection<Comment> Comments { get; set; }

    }


}