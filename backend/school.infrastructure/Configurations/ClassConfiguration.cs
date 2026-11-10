using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Configurations;

public sealed class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> b)
    {
        b.ToTable("classes", t => t.HasCheckConstraint("classes_total_seats_ck", "total_seats >= 1"));

        b.HasKey(c => c.ClassId);
        b.Property(c => c.ClassId).HasColumnName("class_id").HasMaxLength(40);
        b.Property(c => c.Name).HasColumnName("class_name").HasMaxLength(120).IsRequired();
        b.Property(c => c.TotalSeats).HasColumnName("total_seats");
        b.Property(c => c.CreatedAt).HasColumnName("created_at");
    }
}