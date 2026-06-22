using Microsoft.EntityFrameworkCore;
using SmartBank.Authentication.Models;

namespace SmartBank.Authentication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(

                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin",
                    Description = "System Administrator"
                },

                new Role
                {
                    RoleId = 2,
                    RoleName = "Manager",
                    Description = "Branch Manager"
                },

                new Role
                {
                    RoleId = 3,
                    RoleName = "Customer",
                    Description = "Bank Customer"
                }
            );
        }
    }
}