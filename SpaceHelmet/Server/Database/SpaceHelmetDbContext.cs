using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpaceHelmet.Server.Database.Entities;
using SpaceHelmet.Server.Database.Support;

namespace SpaceHelmet.Server.Database {
    public interface IDbContext {
        DbSet<DbUser>           Users { get; }
    }

    public class SpaceHelmetDbContext : IdentityDbContext<DbUser>, IDbContext {

        public SpaceHelmetDbContext( DbContextOptions<SpaceHelmetDbContext> options ) :
            base( options ) { }

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {
            base.OnModelCreating( modelBuilder );

            InitializeModelProperties( modelBuilder );

            modelBuilder.ApplyConfiguration( new RoleConfiguration());
        }

        private static void InitializeModelProperties( ModelBuilder modelBuilder ) { }
    }
}
