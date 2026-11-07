using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assignment_3.Configurations
{
    internal class CategoryConfigs : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasOne(c => c.ParentCategory)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(c => c.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Category { Id = 1, Name = "Programming" },
                new Category { Id = 2, Name = "Design" },
                new Category { Id = 3, Name = "Backend", ParentId = 1 },
                new Category { Id = 4, Name = "ASP.NET Core", ParentId = 3 },
                new Category { Id = 5, Name = "UI/UX", ParentId = 2 }
            );
        }
    }
}