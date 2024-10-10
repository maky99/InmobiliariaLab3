
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaLab3.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Propietario> Propietario { get; set; }
        public DbSet<Inquilino> Inquilino { get; set; }


    }
}
