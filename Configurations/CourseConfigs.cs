using Assignment_3.Enums;
using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment_3.Configurations
{
    internal class CourseConfigs : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(c => c.Price)
                   .HasColumnType("decimal")
                   .HasPrecision(18, 2);

            builder.HasOne(c => c.Instructor)
                   .WithMany(i => i.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Category)
                   .WithMany(c => c.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Course { Id = 1, Title = "C# Basics", Price = 500, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2020, 3, 1), InstructorId = 1, CategoryId = 3 },
                new Course { Id = 2, Title = "Advanced EF Core", Price = 800, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2024, 1, 1), InstructorId = 1, CategoryId = 4 },
                new Course { Id = 3, Title = "Figma for Beginners", Price = 400, Level = CourseLevel.Beginner, IsPublished = false, InstructorId = 2, CategoryId = 5 },
                new Course { Id = 4, Title = "SQL Server Mastery", Price = 600, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2021, 5, 1), InstructorId = 3, CategoryId = 3 },
                new Course { Id = 5, Title = "Web APIs with .NET", Price = 900, Level = CourseLevel.Advanced, IsPublished = false, InstructorId = 1, CategoryId = 4 },
                new Course { Id = 6, Title = "Design Principles", Price = 300, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2022, 2, 1), InstructorId = 2, CategoryId = 2 }
            );
        }
    }
}