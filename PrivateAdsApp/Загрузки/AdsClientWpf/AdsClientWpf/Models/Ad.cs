using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsClientWpf.Models
{
    public class Ad
    {
        public int AdId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // FK
        public int UserId { get; set; }
        public User? User { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? CityId { get; set; }
        public City? City { get; set; }

        public int AdTypeId { get; set; }
        public AdType? AdType { get; set; }

        public int AdStatusId { get; set; }
        public AdStatus? AdStatus { get; set; }

        public int? CompletedAmount { get; set; }

        public ICollection<Photo>? Photos { get; set; }

        [NotMapped]
        public byte[]? MainImageBytes => Photos?.FirstOrDefault(p => p.IsMain)?.ImageData ?? Photos?.FirstOrDefault()?.ImageData;
    }
}
