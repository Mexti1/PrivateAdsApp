using System.Collections.Generic;

namespace AdsClientWpf.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Ad>? Ads { get; set; }
    }
}
