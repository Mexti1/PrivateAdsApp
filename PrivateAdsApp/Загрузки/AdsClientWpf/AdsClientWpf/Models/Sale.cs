using System;

namespace AdsClientWpf.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int AdId { get; set; }
        public Ad? Ad { get; set; }
        public int SaleAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
