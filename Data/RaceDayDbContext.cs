
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<EventType> EventTypes { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<EventCategory> EventCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(u => u.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(u => u.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsRequired(false);

                entity.Property(u => u.Location)
                    .HasColumnName("location")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired(false);


                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("password")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasColumnName("role")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsRequired();
            });

            modelBuilder.Entity<EventType>(entity =>
            {
                entity.ToTable("EventType");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EventName)
                    .HasColumnName("event_name")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasIndex(c => c.Name)
                    .IsUnique();

                entity.Property(c => c.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.EventDate)
                    .HasColumnName("event_date")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("time");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Distance)
                    .HasColumnName("distance")
                    .HasColumnType("decimal(6,2)");
                entity.Property(e => e.EntryFee)
                   .HasColumnName("entry_fee")
                   .HasColumnType("decimal(10,2)")
                   .IsRequired(false);

                entity.Property(e => e.EventTypeId)
                    .HasColumnName("event_type_id");

                entity.Property(e => e.OrganizerId)
                    .HasColumnName("organizer_id");

                entity.Property(e => e.RegistrationDeadline)
                    .HasColumnName("registration_deadline")
                    .HasColumnType("date");

                entity.Property(e => e.MaxParticipants)
                    .HasColumnName("max_participants");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsRequired();

                // Foreign key: Event -> EventType
                entity.HasOne<EventType>()
                    .WithMany()
                    .HasForeignKey(e => e.EventTypeId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Foreign key: Event -> Users (Organiser)
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.ToTable("Enrollment");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EventId)
                    .HasColumnName("event_id")
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.CategoryId)
                   .HasColumnName("category_id")
                   .IsRequired();

                entity.Property(e => e.RegistrationDate)
                    .HasColumnName("registration_date")
                    .HasColumnType("datetime2")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.RaceNumber)
                    .HasColumnName("race_number")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsRequired();

                // Each enrollment belongs to one event.
                entity.HasOne<Event>()
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.NoAction);


                entity.HasOne<EventCategory>()
                    .WithMany()
                    .HasForeignKey(e => new
                    {
                        e.EventId,
                        e.CategoryId
                    })
                    .HasPrincipalKey(ec => new
                    {
                        ec.EventId,
                        ec.CategoryId
                    })
                    .OnDelete(DeleteBehavior.NoAction);


                // Each enrollment belongs to one participant.
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("Results");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(r => r.EnrollmentId)
                    .HasColumnName("enrollment_id")
                    .IsRequired();

                entity.Property(r => r.FinishTime)
                    .HasColumnName("finish_time")
                    .HasColumnType("time");

                entity.Property(r => r.Position)
                    .HasColumnName("position");

                entity.Property(r => r.Pace)
                    .HasColumnName("pace")
                    .HasColumnType("decimal(6,2)");

                entity.Property(r => r.Status)
                    .HasColumnName("status")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                // One enrollment can have at most one result.
                entity.HasOne<Enrollment>()
                    .WithOne()
                    .HasForeignKey<Result>(r => r.EnrollmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<EventCategory>(entity =>
            {
                entity.ToTable("EventCategory");

                // Composite primary key
                entity.HasKey(ec => new
                {
                    ec.EventId,
                    ec.CategoryId
                });

                entity.Property(ec => ec.EventId)
                    .HasColumnName("event_id");

                entity.Property(ec => ec.CategoryId)
                    .HasColumnName("category_id");

                // Connect EventCategory to Event
                entity.HasOne<Event>()
                    .WithMany()
                    .HasForeignKey(ec => ec.EventId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Connect EventCategory to Category
                entity.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(ec => ec.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });


        }

    }
}
