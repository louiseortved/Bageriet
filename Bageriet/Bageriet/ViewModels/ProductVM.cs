using System.Collections.Generic;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class ProductVM
    {
        public virtual List<Category> Categories { get; set; }
        public virtual List<Category> CategoryWomen { get; set; }

        public virtual List<Ingredient> Ingredienses { get; set; }
        //public virtual List<Amount> Amounts { get; set; }
        //public virtual Amount Amount { get; set; }

        //public virtual List<Recipe> Recipes { get; set; }
        //public virtual List<Brand> Brands { get; set; }
        //public virtual List<ProductImage> Images { get; set; }


        public virtual List<Product> AllProducts { get; set; }
        public virtual List<Product> ProductsOnSale { get; set; }
        public virtual List<Product> ProductsNew { get; set; }


        //public virtual List<SizeProduct> SizeProduct { get; set; }

        public virtual Product Product { get; set; }
        public virtual Product ProductId { get; set; }

        //public virtual Cart Cart { get; set; }

    }
}