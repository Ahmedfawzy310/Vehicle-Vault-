using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace VehicleVault.Ef.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<State> States { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<AdminRequest> AdminRequests { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Decoration> Decorations { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalDecoration> RentalDecorations { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Vehicle>()
              .HasOne(v => v.Type) 
              .WithMany(vt => vt.Vehicles) // Each VehicleType has many Vehicles
               .HasForeignKey(v => v.TypeId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
              .HasOne(v => v.PaymentMethod)
              .WithMany(vt => vt.Payments) // Each VehicleType has many Vehicles
               .HasForeignKey(v => v.MethodId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Vehicle>()
                 .HasMany(v => v.Decorations)
                 .WithOne(d => d.Vehicle)
                 .HasForeignKey(d => d.VehicleId)  // Nullable foreign key
                 .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<RentalDecoration>()
                 .HasKey(rd => new { rd.RentalId, rd.DecorationId });

            // Configure relationships for RentalDecoration
            builder.Entity<RentalDecoration>()
                .HasOne(rd => rd.Rental)
                .WithMany(r => r.RentalDecorations)
                .HasForeignKey(rd => rd.RentalId);

            builder.Entity<RentalDecoration>()
                .HasOne(rd => rd.Decoration)
                .WithMany() // No navigation back to RentalDecoration from Decoration
                .HasForeignKey(rd => rd.DecorationId);

            // Optional relationship for Rental to Vehicle
            builder.Entity<Rental>()
                .HasOne(r => r.Vehicle)
                .WithMany()
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
              .HasOne(p => p.Rental)
              .WithOne(r => r.Payment)  // Assuming there's a Payment navigation property in Rental
              .HasForeignKey<Payment>(p => p.RentalId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rental>()
              .HasOne(r => r.Offer)
              .WithMany()
              .HasForeignKey(r => r.OfferId)
              .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
