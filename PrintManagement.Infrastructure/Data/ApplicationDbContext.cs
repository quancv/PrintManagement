using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.Enumerates;
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
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data 
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = (int)ConstantEnums.Role.Leader, RoleCode = "LEA", RoleName = ConstantEnums.Role.Leader.ToString() },
                new Role { Id = (int)ConstantEnums.Role.Designer, RoleCode = "DES", RoleName = ConstantEnums.Role.Designer.ToString() },
                new Role { Id = (int)ConstantEnums.Role.Deliver, RoleCode = "DEL", RoleName = ConstantEnums.Role.Deliver.ToString() },
                new Role { Id = (int)ConstantEnums.Role.Manager, RoleCode = "MAN", RoleName = ConstantEnums.Role.Manager.ToString() },
                new Role { Id = (int)ConstantEnums.Role.Employee, RoleCode = "EMP", RoleName = ConstantEnums.Role.Employee.ToString() },
                new Role { Id = (int)ConstantEnums.Role.Admin, RoleCode = "ADM", RoleName = ConstantEnums.Role.Admin.ToString() }
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
