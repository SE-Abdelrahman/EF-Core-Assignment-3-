using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment_3.Configurations
{
    internal class StudentConfigs : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(s => s.FullName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(s => s.Email)
                   .IsRequired()
                   .HasMaxLength(150);
        }
    }
}