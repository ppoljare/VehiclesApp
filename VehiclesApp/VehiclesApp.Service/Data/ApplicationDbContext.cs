using Microsoft.EntityFrameworkCore;
using VehiclesApp.Model.Entities;

namespace VehiclesApp.Service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<VehicleMake> VehicleMakes { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VehicleMake>().ToTable("VehicleMake");
            modelBuilder.Entity<VehicleModel>().ToTable("VehicleModel");
        }
    }
}
