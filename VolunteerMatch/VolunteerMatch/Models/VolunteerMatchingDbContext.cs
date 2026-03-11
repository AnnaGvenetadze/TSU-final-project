using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VolunteerMatch.Models;

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

    public virtual DbSet<MatchingSuggestion> MatchingSuggestions { get; set; }

    public virtual DbSet<OrganizationProfile> OrganizationProfiles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VolunteerProfile> VolunteerProfiles { get; set; }

    public virtual DbSet<VolunteerTag> VolunteerTags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-QARO7VF5;Database=VolunteerMatchingDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.EventId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(200);

            //entity.HasOne(d => d.Organization).WithMany(p => p.Events)
            //    .HasForeignKey(d => d.OrganizationId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Events_Organization");
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

            //entity.HasOne(d => d.Volunteer).WithMany(p => p.FavoriteEvents)
            //    .HasForeignKey(d => d.VolunteerId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_FavoriteEvents_Volunteer");
        });

        modelBuilder.Entity<MatchingSuggestion>(entity =>
        {
            entity.HasKey(e => e.SuggestionId);

            entity.Property(e => e.SuggestionId).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetimeoffset())");
            entity.Property(e => e.Initiator).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Event).WithMany(p => p.MatchingSuggestions)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MS_Event");

            //entity.HasOne(d => d.Volunteer).WithMany(p => p.MatchingSuggestions)
            //    .HasForeignKey(d => d.VolunteerId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_MS_Volunteer");
        });

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

            //entity.HasOne(d => d.Volunteer).WithMany(p => p.VolunteerTags)
            //    .HasForeignKey(d => d.VolunteerId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_VolunteerTags_Volunteer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
