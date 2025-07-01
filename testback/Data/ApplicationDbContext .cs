using Microsoft.EntityFrameworkCore;
using testback.Models;

namespace testback.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Obra> Obra { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<IngresosPersonal> IngresosPersonal { get; set; }
        public DbSet<SalidasPersonal> SalidasPersonal { get; set; }
        public DbSet<DocumentoPermiso> DocumentoPermisos { get; set; }
        public DbSet<Solicitud> Solicitud { get; set; }
        public DbSet<SolicitudItem> SolicitudItem { get; set; }
        public DbSet<Movimiento> Movimiento { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<InventarioInterno> InventarioInterno { get; set; }
        public DbSet<RevisionInventario> RevisionInventario { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Solicitud>()
                .Property(s => s.Estado)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsUnicode(false);
        }
    }
}
