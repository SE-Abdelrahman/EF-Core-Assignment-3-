using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment_3.Configurations
{
    internal class InstructorConfigs : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(i => i.FullName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(i => i.Email)
                   .IsRequired()
                   .HasMaxLength(150);
            builder.Property(i => i.Bio)
                   .HasMaxLength(500);

            builder.HasData(
                new Instructor { Id = 1, FullName = "Ahmed Ali", Email = "ahmed@test.com", JoinDate = new DateTime(2019, 7, 1) },
                new Instructor { Id = 2, FullName = "Sara Said", Email = "sara@test.com", JoinDate = new DateTime(2020, 8, 1) },
                new Instructor { Id = 3, FullName = "Omar Zaki", Email = "omar@test.com", JoinDate = new DateTime(2021, 9, 1) }
            );
        }
    }
}