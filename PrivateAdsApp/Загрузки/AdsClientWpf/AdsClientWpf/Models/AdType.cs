using System.Collections.Generic;

namespace AdsClientWpf.Models
{
    public class AdType
    {
        public int AdTypeId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Ad>? Ads { get; set; }
    }
}
