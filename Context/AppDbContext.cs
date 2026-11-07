using Assignment_3.Configurations;
using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_3.Context
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=OnlineLearningPlatform;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfigs());
            modelBuilder.ApplyConfiguration(new CourseConfigs());
            modelBuilder.ApplyConfiguration(new EnrollmentConfigs());
            modelBuilder.ApplyConfiguration(new InstructorConfigs());
            modelBuilder.ApplyConfiguration(new StudentConfigs());
        }
    }
}