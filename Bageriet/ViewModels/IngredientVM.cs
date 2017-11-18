using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class IngredientVM
    {

        public virtual List<Ingredient> Ingredienses { get; set; }

       

        public virtual Product Product { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    
    }
}