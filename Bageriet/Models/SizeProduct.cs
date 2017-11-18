using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bageriet.Models
{
    public class SizeProduct
    {
        [Key]
        [Column(Order = 1)]
        public int SizeId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ProductId { get; set; }

        public virtual Size Size { get; set; }
        public virtual Product Product { get; set; }
    }
}