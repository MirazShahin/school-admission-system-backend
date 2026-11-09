using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Infrastructure.Configurations;

public sealed class HoldConfiguration : IEntityTypeConfiguration<Hold>
{
    private static readonly ValueConverter<HoldStatus, string> StatusConverter =
        new(s => s.ToDb(), v => HoldStatusExtensions.FromDb(v));

    public void Configure(EntityTypeBuilder<Hold> b)
    {
        b.ToTable("holds", t => t.HasCheckConstraint(
            "holds_status_ck",
            "status IN ('HELD','CONFIRMED','EXPIRED','CANCELLED')"));

        b.HasKey(h => h.HoldId);
        b.Property(h => h.HoldId).HasColumnName("hold_id").HasMaxLength(40);
        b.Property(h => h.ClassId).HasColumnName("class_id").HasMaxLength(40);
        b.Property(h => h.StudentId).HasColumnName("student_id").HasMaxLength(17);
        b.Property(h => h.Status).HasColumnName("status").HasMaxLength(12).HasConversion(StatusConverter);
        b.Property(h => h.ExpiresAt).HasColumnName("expires_at");
        b.Property(h => h.CreatedAt).HasColumnName("created_at");
        b.Property(h => h.UpdatedAt).HasColumnName("updated_at");

        b.HasOne<Class>().WithMany().HasForeignKey(h => h.ClassId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Student>().WithMany().HasForeignKey(h => h.StudentId).OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(h => new { h.ClassId, h.Status }).HasDatabaseName("holds_class_status_idx");
        b.HasIndex(h => new { h.StudentId, h.CreatedAt }).HasDatabaseName("holds_student_created_idx");

        b.HasIndex(h => new { h.ClassId, h.StudentId })
            .IsUnique()
            .HasFilter("status IN ('HELD','CONFIRMED')")
            .HasDatabaseName("holds_one_live_per_student_class_uq");
    }
}