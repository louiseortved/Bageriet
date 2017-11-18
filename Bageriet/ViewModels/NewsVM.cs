using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class NewsVM
    {
        public News News { get; set; }
        public virtual List<News> NewsList { get; set; }

        public virtual ApplicationUser User { get; set; }

        public List<Comment> Comment { get; set; }

    }


}