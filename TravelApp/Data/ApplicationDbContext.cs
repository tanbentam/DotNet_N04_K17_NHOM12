using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelApp.Models;

namespace TravelApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<HotelModel> Hotels { get; set; }
        public DbSet<DestinationModel> Destinations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ConfigureUsers(modelBuilder);
            ConfigureDestinations(modelBuilder);
            ConfigureHotels(modelBuilder);
            ConfigureBookings(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void ConfigureUsers(DbModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<UserModel>();

            user.ToTable("Users");
            user.HasKey(x => x.Id);
            user.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(254)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UX_Users_Email") { IsUnique = true }));
            user.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UX_Users_Phone") { IsUnique = true }));
            user.Property(x => x.PasswordHash).IsRequired().HasMaxLength(255);
            user.Property(x => x.FullName).IsRequired().HasMaxLength(100);
        }

        private static void ConfigureDestinations(DbModelBuilder modelBuilder)
        {
            var destination = modelBuilder.Entity<DestinationModel>();

            destination.ToTable("Destinations");
            destination.HasKey(x => x.Id);
            destination.Property(x => x.Name).IsRequired().HasMaxLength(150);
            destination.Property(x => x.Country).IsRequired().HasMaxLength(100);
            destination.Property(x => x.Description).IsOptional().HasMaxLength(2000);
            destination.Property(x => x.ImageUrl).IsOptional().HasMaxLength(500);
            destination.Property(x => x.AverageRating).HasPrecision(3, 2);
        }

        private static void ConfigureHotels(DbModelBuilder modelBuilder)
        {
            var hotel = modelBuilder.Entity<HotelModel>();

            hotel.ToTable("Hotels");
            hotel.HasKey(x => x.Id);
            hotel.Property(x => x.Name).IsRequired().HasMaxLength(150);
            hotel.Property(x => x.Address).IsRequired().HasMaxLength(300);
            hotel.Property(x => x.Description).IsOptional().HasMaxLength(2000);
            hotel.Property(x => x.ImageUrl).IsOptional().HasMaxLength(500);
            hotel.Property(x => x.PricePerNight).HasPrecision(18, 2);
            hotel.HasRequired(x => x.Destination)
                .WithMany(x => x.Hotels)
                .HasForeignKey(x => x.DestinationId)
                .WillCascadeOnDelete(false);
        }

        private static void ConfigureBookings(DbModelBuilder modelBuilder)
        {
            var booking = modelBuilder.Entity<BookingModel>();

            booking.ToTable("Bookings");
            booking.HasKey(x => x.Id);
            booking.Property(x => x.BookingId)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UX_Bookings_BookingId") { IsUnique = true }));
            booking.Property(x => x.Price).HasPrecision(18, 2);
            booking.Property(x => x.DestinationName).IsOptional().HasMaxLength(150);
            booking.Property(x => x.UserName).IsOptional().HasMaxLength(100);

            booking.HasRequired(x => x.User)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);
            booking.HasRequired(x => x.Guide)
                .WithMany(x => x.GuidedBookings)
                .HasForeignKey(x => x.GuideId)
                .WillCascadeOnDelete(false);
            booking.HasOptional(x => x.Hotel)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.HotelId)
                .WillCascadeOnDelete(false);
            booking.HasRequired(x => x.Destination)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.DestinationId)
                .WillCascadeOnDelete(false);
        }
    }
}
