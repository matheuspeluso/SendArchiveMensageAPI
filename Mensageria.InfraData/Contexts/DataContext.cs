using Mensageria.InfraData.Mappings;
using Microsoft.EntityFrameworkCore;


namespace Mensageria.InfraData.Contexts
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=DBMensageria;User=DevMatheus;Password=FuscaAzul;",
                ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=DBMensageria;User=DevMatheus;Password=FuscaAzul;")
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ArchiveMap());
        }
    }
}
