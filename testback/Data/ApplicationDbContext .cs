    using Microsoft.EntityFrameworkCore;
    using testback.Models;

    namespace testback.Data
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

            public DbSet<Empleado> Empleado { get; set; }
            public DbSet<Obra> Obra { get; set; }
            public DbSet<Usuario> Usuario { get; set; }
            public DbSet<IngresosPersonal> IngresosPersonal { get; set; }
            public DbSet<SalidasPersonal> SalidasPersonal { get; set; }
            protected override void OnModelCreating(ModelBuilder builder)
            {
                base.OnModelCreating(builder);
            }
        }
    }
