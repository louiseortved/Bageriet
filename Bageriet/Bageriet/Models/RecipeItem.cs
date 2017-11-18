using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bageriet.Models
{
    public class RecipeItem
    {

        public int Id { get; set; }

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        // You'll want to think what unit this is meant to be in... another field?
        public decimal Quantity { get; set; }
    }


}