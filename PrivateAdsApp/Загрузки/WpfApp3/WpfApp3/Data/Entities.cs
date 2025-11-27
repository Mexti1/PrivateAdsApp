using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

namespace WpfApp3.Data
{
    public class Entities : DbContext
    {
        public Entities() : base("Djabaorv_DB_Pay_Connection") { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Payment> Payment { get; set; }

        private static Entities _context;
        public static Entities GetContext()
        {
            if (_context == null)
                _context = new Entities();
            return _context;
        }
    }
}
