using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bageriet.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<RecipeItem>  Parts{ get; set; }
    }
}