using System.Collections.Generic;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class NewsletterVM
    {
        public virtual List<Subscriber> Subscribers { get; set; }
        public virtual Newsletter Newsletter { get; set; }
    }
}