using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SubcategoryConfiguration : IEntityTypeConfiguration<Subcategory>
{
    public void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        builder.ToTable("Subcategory");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("subcategory_id")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(255);

        // Relationships
        builder.HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Contacts)
            .WithOne(c => c.Subcategory)
            .HasForeignKey(c => c.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
