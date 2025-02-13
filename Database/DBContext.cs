using Microsoft.EntityFrameworkCore;

namespace autenticacion.Database
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Models.User> Users { get; set; }
    }
}
