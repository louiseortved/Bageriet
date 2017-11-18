using System.Collections.Generic;

namespace Bageriet.Models
{
    public class Size
    {

        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<SizeProduct> SizeProducts { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}