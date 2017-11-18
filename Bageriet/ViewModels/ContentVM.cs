using System.Collections.Generic;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class ContentVM
    {
        public virtual List<Slider> Sliders { get; set; }

        //public virtual List<Event> Events{ get; set; }
        public virtual List<About> Abouts { get; set; }

     public virtual List<News> News { get; set; }

        public virtual List<Product> Products { get; set; }

    }
}