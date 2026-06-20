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

    public virtual DbSet<OrganizationProfile> OrganizationProfiles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VolunteerProfile> VolunteerProfiles { get; set; }

    public virtual DbSet<VolunteerTag> VolunteerTags { get; set; }

    public DbSet<VolunteerEventMatch> VolunteerEventMatches { get; set; }

    public DbSet<Skill> Skills { get; set; }

    public DbSet<Interest> Interests { get; set; }

    public DbSet<VolunteerSkill> VolunteerSkills { get; set; }

    public DbSet<VolunteerInterest> VolunteerInterests { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

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

            entity.Property(e => e.MainPhotoUrl)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Photo2Url)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Photo3Url)
                .HasColumnType("nvarchar(max)");

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

        modelBuilder.Entity<OrganizationProfile>(entity =>
        {
            entity.HasKey(e => e.OrganizationId);

            entity.Property(e => e.OrganizationId).ValueGeneratedNever();
            entity.Property(e => e.LinkedInUrl).HasMaxLength(300);
            entity.Property(e => e.OrganizationName).HasMaxLength(200);
            entity.Property(e => e.ProfilePhotoUrl).HasColumnType("nvarchar(max)");

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
            entity.Property(e => e.ProfilePhotoUrl).HasColumnType("nvarchar(max)");
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

            entity.Property(m => m.CreatedAt)
                .HasDefaultValueSql("sysdatetimeoffset()")
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

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(skill => skill.SkillId);

            entity.Property(skill => skill.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Interest>(entity =>
        {
            entity.HasKey(interest => interest.InterestId);

            entity.Property(interest => interest.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<VolunteerSkill>(entity =>
        {
            entity.HasKey(volunteerSkill => new
            {
                volunteerSkill.VolunteerId,
                volunteerSkill.SkillId
            });

            entity.HasOne(volunteerSkill => volunteerSkill.Volunteer)
                .WithMany(volunteer => volunteer.VolunteerSkills)
                .HasForeignKey(volunteerSkill => volunteerSkill.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(volunteerSkill => volunteerSkill.Skill)
                .WithMany(skill => skill.VolunteerSkills)
                .HasForeignKey(volunteerSkill => volunteerSkill.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VolunteerInterest>(entity =>
        {
            entity.HasKey(volunteerInterest => new
            {
                volunteerInterest.VolunteerId,
                volunteerInterest.InterestId
            });

            entity.HasOne(volunteerInterest => volunteerInterest.Volunteer)
                .WithMany(volunteer => volunteer.VolunteerInterests)
                .HasForeignKey(volunteerInterest => volunteerInterest.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(volunteerInterest => volunteerInterest.Interest)
                .WithMany(interest => interest.VolunteerInterests)
                .HasForeignKey(volunteerInterest => volunteerInterest.InterestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(refreshToken => refreshToken.RefreshTokenId);

            entity.Property(refreshToken => refreshToken.TokenHash)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(refreshToken => refreshToken.ExpiresAt)
                .IsRequired();

            entity.Property(refreshToken => refreshToken.CreatedAt)
                .IsRequired();

            entity.HasIndex(refreshToken => refreshToken.TokenHash)
                .IsUnique();

            entity.HasIndex(refreshToken => refreshToken.UserId);

            entity.HasOne(refreshToken => refreshToken.User)
                .WithMany()
                .HasForeignKey(refreshToken => refreshToken.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
