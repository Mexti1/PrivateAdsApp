using System;
using System.Collections.Generic;

namespace AdsClientWpf.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Ad>? Ads { get; set; }
    }
}
