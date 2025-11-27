using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp2.Data
{
    public class Users
    {
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; } // хранится хэш

        [Required, MaxLength(50)]
        public string Role { get; set; }

        [Required, MaxLength(250)]
        public string FIO { get; set; }

        public string Photo { get; set; }

        public virtual ICollection<Payment> Payment { get; set; }
    }

    public class Category
    {
        public int ID { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public virtual ICollection<Payment> Payment { get; set; }
    }

    public class Payment
    {
        public int ID { get; set; }

        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual Users User { get; set; }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Num { get; set; }
        public decimal Price { get; set; }
    }
}
