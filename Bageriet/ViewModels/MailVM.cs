using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bageriet.Models;

namespace Bageriet.ViewModels
{
    public class MailVM
    {
        public virtual List<Mail> InboxList { get; set; }
        public virtual List<Mail> TrashList { get; set; }

        public int InboxCount { get; set; }
        public int TrashCount { get; set; }
    }
}