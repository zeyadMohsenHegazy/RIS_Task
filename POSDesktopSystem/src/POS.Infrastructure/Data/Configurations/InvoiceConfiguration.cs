using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;
using POS.Domain.Enums;

namespace POS.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Date)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Discount)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Tax)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.FinalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.PaymentMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.SyncStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.SyncError)
            .HasMaxLength(500);

        builder.HasIndex(i => i.SyncStatus);
        builder.HasIndex(i => i.Date);
        builder.HasIndex(i => i.UserId);

        builder.HasOne(i => i.User)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Items)
            .WithOne(ii => ii.Invoice)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
