using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("category_id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(255);

        // Relationships
        builder.HasMany(c => c.Subcategories)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Contacts)
            .WithOne(con => con.Category)
            .HasForeignKey(con => con.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
