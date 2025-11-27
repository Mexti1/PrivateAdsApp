using System.Collections.Generic;

namespace AdsClientWpf.Models
{
    public class AdStatus
    {
        public int AdStatusId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Ad>? Ads { get; set; }
    }
}
