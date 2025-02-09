using ACME.RestService.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace ACME.RestService.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<Ventas> Ventas { get; set; }
        public DbSet<Visitas> Visitas { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Historial> Historial { get; set; }

    }
}
