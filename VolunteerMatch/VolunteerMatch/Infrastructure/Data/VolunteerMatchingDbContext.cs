using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Data;

public partial class VolunteerMatchingDbContext : DbContext
{
    public VolunteerMatchingDbContext()
    {
    }

    public VolunteerMatchingDbContext(DbContextOptions<VolunteerMatchingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventTag> EventTags { get; set; }

    public virtual DbSet<FavoriteEvent> FavoriteEvents { get; set; }

    //public virtual DbSet<MatchingSuggestion> MatchingSuggestions { get; set; }

    public virtual DbSet<OrganizationProfile> OrganizationProfiles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VolunteerProfile> VolunteerProfiles { get; set; }

    public virtual DbSet<VolunteerTag> VolunteerTags { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<VolunteerEventMatch> VolunteerEventMatches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-QARO7VF5;Database=VolunteerMatchingDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {// აქ დავამატე ველები და შეზღუდვები
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId);

            entity.Property(e => e.EventId)
                .HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.OrganizationId);

            entity.Property(e => e.Title)
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Requirements)
                .HasMaxLength(1000);

            entity.Property(e => e.Location)
                .HasMaxLength(100);

            entity.Property(e => e.StartDate)
                .HasColumnType("datetimeoffset");

            entity.Property(e => e.EndDate)
                .HasColumnType("datetimeoffset");

            entity.Property(e => e.DailyStartTime)
                .HasColumnType("time");

            entity.Property(e => e.DailyEndTime)
                .HasColumnType("time");

            entity.Property(e => e.VolunteersAmount);

            entity.Property(e => e.Benefits)
                .HasMaxLength(1000);

            //entity.Property(e => e.SpeakersJson)
            //    .HasColumnType("nvarchar(max)");

            entity.Property(e => e.MainPhotoUrl)
                .HasMaxLength(500);

            entity.Property(e => e.Photo2Url)
                .HasMaxLength(500);

            entity.Property(e => e.Photo3Url)
                .HasMaxLength(500);

            entity.Property(e => e.AdditionalInfo)
                .HasMaxLength(1000);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.Organization)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Events_Organization");

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CHK_Events_EndDate_After_StartDate", "[EndDate] >= [StartDate]");
                t.HasCheckConstraint("CHK_Events_DailyEndTime_After_DailyStartTime", "[DailyEndTime] > [DailyStartTime]");
                t.HasCheckConstraint("CHK_Events_VolunteersAmount_Positive", "[VolunteersAmount] > 0");
            });
        });

        modelBuilder.Entity<EventTag>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.TagId });

            entity.HasOne(d => d.Event).WithMany(p => p.EventTags)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTags_Event");

            entity.HasOne(d => d.Tag).WithMany(p => p.EventTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTags_Tag");
        });

        modelBuilder.Entity<FavoriteEvent>(entity =>
        {
            entity.HasKey(e => new { e.VolunteerId, e.EventId });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");

            entity.HasOne(d => d.Event).WithMany(p => p.FavoriteEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FavoriteEvents_Event");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.FavoriteEvents)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FavoriteEvents_Volunteer");
        });

        //modelBuilder.Entity<MatchingSuggestion>(entity =>
        //{
        //    entity.HasKey(e => e.SuggestionId);

        //    entity.Property(e => e.SuggestionId).HasDefaultValueSql("(newsequentialid())");
        //    entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
        //    entity.Property(e => e.Initiator).HasMaxLength(20);
        //    entity.Property(e => e.Status).HasMaxLength(30);

        //    entity.HasOne(d => d.Event).WithMany(p => p.MatchingSuggestions)
        //        .HasForeignKey(d => d.EventId)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK_MS_Event");

        //    //entity.HasOne(d => d.Volunteer).WithMany(p => p.MatchingSuggestions)
        //    //    .HasForeignKey(d => d.VolunteerId)
        //    //    .OnDelete(DeleteBehavior.ClientSetNull)
        //    //    .HasConstraintName("FK_MS_Volunteer");
        //});

        modelBuilder.Entity<OrganizationProfile>(entity =>
        {
            entity.HasKey(e => e.OrganizationId);

            entity.Property(e => e.OrganizationId).ValueGeneratedNever();
            entity.Property(e => e.LinkedInUrl).HasMaxLength(300);
            entity.Property(e => e.OrganizationName).HasMaxLength(200);
            entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);

            entity.HasOne(d => d.Organization).WithOne(p => p.OrganizationProfile)
                .HasForeignKey<OrganizationProfile>(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrganizationProfiles_Users");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Tags_Name").IsUnique();

            entity.Property(e => e.TagId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<VolunteerProfile>(entity =>
        {
            entity.HasKey(e => e.VolunteerId);

            entity.Property(e => e.VolunteerId).ValueGeneratedNever();
            entity.Property(e => e.Citizenship).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Languages).HasMaxLength(500);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Profession).HasMaxLength(200);
            entity.Property(e => e.Education).HasMaxLength(500);
            entity.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);
            entity.Property(e => e.LinkedInUrl).HasMaxLength(300);
            entity.Property(e => e.Technologies).HasMaxLength(500);

            entity.HasOne(d => d.Volunteer).WithOne(p => p.VolunteerProfile)
                .HasForeignKey<VolunteerProfile>(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VolunteerProfiles_Users");
        });

        modelBuilder.Entity<VolunteerTag>(entity =>
        {
            entity.HasKey(e => new { e.VolunteerId, e.TagId });

            entity.HasOne(d => d.Tag).WithMany(p => p.VolunteerTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VolunteerTags_Tag");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.VolunteerTags)
                .HasForeignKey(d => d.VolunteerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VolunteerTags_Volunteer");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");

            entity.HasKey(n => n.NotificationId);

            entity.Property(n => n.NotificationId)
                .HasDefaultValueSql("newsequentialid()");

            entity.Property(n => n.Type)
                .HasConversion<byte>()
                .IsRequired();

            entity.Property(n => n.Message)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(n => n.ExpiresAt)
                .IsRequired();

            entity.Property(n => n.CreatedAt)
                .HasDefaultValueSql("sysdatetimeoffset()")
                .IsRequired();

            entity.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(n => n.Event)
                .WithMany()
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(n => n.RelatedUser)
                .WithMany()
                .HasForeignKey(n => n.RelatedUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<VolunteerEventMatch>(entity =>
        {
            entity.ToTable("VolunteerEventMatches");

            entity.HasKey(m => m.VolunteerEventMatchId);

            entity.Property(m => m.VolunteerEventMatchId)
                .HasDefaultValueSql("newsequentialid()");

            entity.Property(m => m.RequestedByRole)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(m => m.Status)
                .HasConversion<byte>()
                .IsRequired();

            entity.Property(m => m.MatchScore)
                .IsRequired(false);

            entity.Property(m => m.CreatedAt)
                .HasDefaultValueSql("sysdatetimeoffset()")
                .IsRequired();

            entity.Property(m => m.ExpiresAt)
                .IsRequired();

            entity.HasOne(m => m.Volunteer)
                .WithMany()
                .HasForeignKey(m => m.VolunteerId);

            entity.HasOne(m => m.Event)
                .WithMany()
                .HasForeignKey(m => m.EventId);

            entity.HasIndex(m => new { m.VolunteerId, m.EventId })
                .IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
