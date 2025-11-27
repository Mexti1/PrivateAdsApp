using System.Data.Entity;

namespace WpfApp2.Data
{
    public class Entities : DbContext
    {
        // В строке подключения можно указать LocalDB или имя сервера
        // Пример: "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Aksenova_DB_Payment;Integrated Security=True;"
        public Entities() : base("Aksenova_DB_Payment_Connection")
        {
            // Отключаем стратегию миграций — проект ожидает, что БД создана заранее (мы выполнили скрипт).
            Database.SetInitializer<Entities>(null);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Payment> Payment { get; set; }

        // Утилита для получения контекста
        private static Entities _context;
        public static Entities GetContext()
        {
            if (_context == null) _context = new Entities();
            return _context;
        }
    }
}
