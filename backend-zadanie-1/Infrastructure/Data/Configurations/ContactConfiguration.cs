using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contact");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("contact_id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(c => c.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.Property(c => c.Password)
            .HasColumnName("password")
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.BirthDate)
            .HasColumnName("birth_date")
            .IsRequired();

        builder.Property(c => c.CategoryId)
            .HasColumnName("category_id");

        builder.Property(c => c.SubcategoryId)
            .HasColumnName("subcategory_id");

        // Relationships
        builder.HasOne(c => c.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Contacts)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Subcategory)
            .WithMany(s => s.Contacts)
            .HasForeignKey(c => c.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
