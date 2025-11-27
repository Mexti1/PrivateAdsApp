using System.Collections.Generic;

namespace AdsClientWpf.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Ad>? Ads { get; set; }
    }
}
