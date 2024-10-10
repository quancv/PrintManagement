using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public ApplicationDbContext() { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<ConfirmEmail> ConfirmEmails { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleCode = "LEA", RoleName = "Leader" },
                new Role { Id = 2, RoleCode = "DES", RoleName = "Designer" },
                new Role { Id = 3, RoleCode = "DEL", RoleName = "Deliver" },
                new Role { Id = 4, RoleCode = "MAN", RoleName = "Manager" },
                new Role { Id = 5, RoleCode = "EMP", RoleName = "Employee" },
                new Role { Id = 6, RoleCode = "ADM", RoleName = "Admin" }
            );
        }
        public async Task<int> CommitChangeAsync()
        {
            return await base.SaveChangesAsync();
        }


        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}
