using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using School.Domain.Entities;

namespace School.Infrastructure.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> b)
    {
        b.ToTable("students", t => t.HasCheckConstraint("students_student_id_ck", "student_id ~ '^[0-9]{17}$'"));

        b.HasKey(s => s.StudentId);
        b.Property(s => s.StudentId).HasColumnName("student_id").HasMaxLength(17);
        b.Property(s => s.CreatedAt).HasColumnName("created_at");
    }
}