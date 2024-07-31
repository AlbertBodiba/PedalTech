using AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static AuthAPI.Models.BikeDropEvent;


namespace AuthAPI.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BikeDropEvent> BikeDropEvents { get; set; }
        public DbSet<StationTask> StationTasks { get; set; }
        public DbSet<BikeStation> BikeStations { get; set; }
        public DbSet<EmailModel> Emails { get; set; }
        public DbSet<Bicycle> Bicycles { get; set; }
        public DbSet<Tool> Tools { get; set; }
         public DbSet<BikeDropSchool> BikeDropSchools { get; set; }
         public DbSet<Meeting> Meetings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BikeDropEvent>(entity =>
            {
                entity.HasKey(e => e.BikeDropEventId); // Define primary key

            });
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id); // Define primary key

            });
            modelBuilder.Entity<StationTask>(entity =>
            {
                entity.HasKey(e => e.Id); // Define primary key

            });
            modelBuilder.Entity<BikeStation>(entity =>
            {
                entity.HasKey(e => e.BikeStationId); // Define primary key

            });
            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.HasKey(e => e.MeetingId); // Define primary key

            });

        }
    }
}