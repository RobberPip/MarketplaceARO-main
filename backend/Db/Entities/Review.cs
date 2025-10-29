using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

public class ReviewConfig : IEntityTypeConfiguration<Review> {
    public void Configure(EntityTypeBuilder<Review> builder) {
        builder.Property(prop => prop.AuthorId).IsRequired();
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(prop => prop.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}

[EntityTypeConfiguration(typeof(ReviewConfig))]
public class Review : IEntity {
    public Guid Id { get; set; }

    public Guid AuthorId { get; set; }
    public Profile? Author { get; set; }

    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
    public int LegacySoftwareId { get; set; }

    // Future
    public string? Text { get; set; }
    public int? Rate { get; set; }
    public bool Like { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}