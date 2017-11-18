using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class SearchVM
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();


    }
}