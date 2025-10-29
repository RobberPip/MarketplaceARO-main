using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

public class FileConfig : IEntityTypeConfiguration<File> {
  public void Configure(EntityTypeBuilder<File> builder) {
    builder.Property(prop => prop.Name).IsRequired();
    builder.Property(prop => prop.ContentType).HasMaxLength(25).IsRequired();
    builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
  }
}

[EntityTypeConfiguration(typeof(FileConfig))]
public class File : IEntity {
  public Guid Id { get; set; }

  public required string Name { get; set; }
  public required string ContentType { get; set; }
  public byte[]? Content { get; set; }
  public string? Path { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  // public Guid? CreatedById { get; set; }
  // public Profile? CreatedBy { get; set; }
  // public Guid? UpdatedById { get; set; }
  // public Profile? UpdatedBy { get; set; }
}

