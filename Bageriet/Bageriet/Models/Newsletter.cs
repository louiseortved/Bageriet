using System.Web.Mvc;

namespace Bageriet.Models
{
    public class Newsletter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string HtmlContent { get; set; }


    }
}