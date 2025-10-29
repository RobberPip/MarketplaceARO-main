using System.Text.Json;
using System.Text.Json.Serialization;
using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

public class ProductConfig : IEntityTypeConfiguration<Product> {
  public void Configure(EntityTypeBuilder<Product> builder) {
    builder.Property(p => p.Name).HasMaxLength(255);
    builder.Property(p => p.Description).HasMaxLength(1000);
    builder.Property(p => p.Website).HasMaxLength(255);
    builder.Property(p => p.CreatedAt).IsRequired();
    builder.Property(p => p.UpdatedAt).IsRequired(false);
    builder.Property(p => p.LikesCount).HasDefaultValue(0);
    builder.Property(p => p.ReviewsCount).HasDefaultValue(0);
    builder.Property(p => p.AverageRating).HasColumnType("numeric(3,2)").HasDefaultValue(0);
    builder.Property(p => p.Status)
         .IsRequired()
         .HasDefaultValue(ProductStatus.Draft)
         .HasConversion<string>();
    builder
    .HasMany(p => p.Files)
    .WithMany()
    .UsingEntity<Dictionary<string, object>>(
        "ProductFiles",
        pf => pf.HasOne<File>()
                .WithMany()
                .HasForeignKey("FileId")
                .HasPrincipalKey(f => f.Id)
                .OnDelete(DeleteBehavior.Cascade),
        pf => pf.HasOne<Product>()
                .WithMany()
                .HasForeignKey("ProductId")
                .HasPrincipalKey(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade),
        pf => {
          pf.HasKey("ProductId", "FileId");
          pf.Property<Guid>("ProductId");
          pf.Property<Guid>("FileId");
          pf.ToTable("ProductFiles");
        }
    );



    builder
    .HasOne(p => p.Type)
    .WithMany(p => p.Products)
    .IsRequired();

    builder
      .HasOne(p => p.Owner)
      .WithMany(a => a.OwnedProducts)
      .IsRequired();

    builder
    .HasOne(p => p.ProductData)
    .WithMany(pd => pd.Product)
    .HasForeignKey(p => p.ProductDataId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired(false);


    builder
  .HasMany(p => p.RightholdersAccounts)
  .WithMany(a => a.RightsProducts)
  .UsingEntity<Dictionary<string, object>>(
    "Rightholders",
    right => right
      .HasOne<Account>()
      .WithMany()
      .HasForeignKey("AccountId")
      .OnDelete(DeleteBehavior.Cascade),
    left => left
      .HasOne<Product>()
      .WithMany()
      .HasForeignKey("ProductId")
      .OnDelete(DeleteBehavior.Cascade),
    join => {
      join.ToTable("Rightholders");
      join.Property<Guid>("ProductId");
      join.Property<Guid>("AccountId");
      join.HasKey("ProductId", "AccountId");
    });

  }
}

[EntityTypeConfiguration(typeof(ProductConfig))]
public class Product : IEntity {
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public ProductStatus Status { get; set; }
  public string? Website { get; set; }
  public required Guid TypeId { get; set; }
  public List<File>? Files { get; set; }
  public ProductType? Type { get; set; }
  public Guid OwnerId { get; set; }
  public Account? Owner { get; set; }
  public List<Account> RightholdersAccounts { get; set; } = new();
  public Guid? ProductDataId { get; set; }
  public ProductData? ProductData { get; set; }

  public int LikesCount { get; set; }
  public int ReviewsCount { get; set; }
  public decimal AverageRating { get; set; }
  public List<Review> Reviews { get; set; } = new();
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductStatus {
  // Черновик
  Draft,
  // На модерации
  Pending,
  // Не прошел модерацию
  Rejected,
  // Требует подтверждения разработчика
  DevApproval,
  // Подтвержден разработчиком
  Approved,
  // Нарушение правил платформы
  Violation,
  // В архиве
  Archived
}

[EntityTypeConfiguration(typeof(ProductTypeConfig))]
public class ProductType {
  public required Guid Id { get; set; }
  public string? FullName { get; set; }
  public string? ShortName { get; set; }
  public string? CombinedName { get; set; }

  public ICollection<Product> Products { get; set; } = [];

  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
public class ProductTypeConfig : IEntityTypeConfiguration<ProductType> {
  public void Configure(EntityTypeBuilder<ProductType> builder) {
    builder.Property(prop => prop.FullName).HasMaxLength(150).IsRequired();
    builder.Property(prop => prop.ShortName).HasMaxLength(50);
    builder.Property(prop => prop.CombinedName).HasMaxLength(150);
    builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

    var data = new[] {
      new { FullName = "Геоинформационные и навигационные системы", ShortName = "GIS" },
      new { FullName = "Серверное и связующее программное обеспечение", ShortName = "MWS" },
      new { FullName = "Системы CAD проектирования", ShortName = "CAD" },
      new { FullName = "Системы виртуальной и дополненной реальности", ShortName = "AR/VR" },
      new { FullName = "Системы диспетчерского управления и сбора данных", ShortName = "SCADA" },
      new { FullName = "Системы для оценки стоимости строительства", ShortName = "CCE" },
      new { FullName = "Системы инженерного анализа и расчетов", ShortName = "CAE" },
      new { FullName = "Системы компьютерного зрения", ShortName = "CVS" },
      new { FullName = "Системы технического обслуживания и ремонта", ShortName = "ТОиР" },
      new { FullName = "Системы управление жизненным циклом продукции", ShortName = "PLM" },
      new { FullName = "Системы управления инженерными данными об изделии", ShortName = "PDM" },
      new { FullName = "Системы управления проектами и календарно-сетевого планирования", ShortName = "PMS" },
      new { FullName = "Системы управления процессами охраны труда, промышленной и пожарной безопасности", ShortName = "ОТиПБ" },
      new { FullName = "Системы управления ресурсами предприятия", ShortName = "ERP" },
      new { FullName = "Системы управления строительством и строительного контроля", ShortName = "CMS" },
      new { FullName = "Системы управления требованиями", ShortName = "RMS" },
      new { FullName = "Системы электронного документооборота", ShortName = "СЭД" },
      new { FullName = "Справочные системы, справочники, библиотеки и базы данных", ShortName = "RDM" },
      new { FullName = "Среда общих данных", ShortName = "CDE" },
      new { FullName = "Средства текстового и графического создания и редактирования", ShortName = "CGT" },
      new { FullName = "Средства технологической подготовки производства", ShortName = "CAPP" },
      new { FullName = "Средства управления производственными процессами", ShortName = "MES" }
    };

    var entities = data.Select(item => new ProductType {
      Id = Guid.NewGuid(),
      FullName = item.FullName,
      ShortName = item.ShortName,
      CombinedName = string.IsNullOrWhiteSpace(item.ShortName) ? item.FullName : $"{item.FullName} ({item.ShortName})",
      CreatedAt = DateTime.UtcNow
    }).ToList();

    builder.HasData(entities);
  }
}

public class ProductDataConfig : IEntityTypeConfiguration<ProductData> {
  public void Configure(EntityTypeBuilder<ProductData> builder) {
    builder.HasKey(d => d.Id);

    builder.Property(d => d.Data)
      .HasColumnType("jsonb")
      .HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<ProductDataEntry>>(v, (JsonSerializerOptions?)null)!
      );
  }
}

[EntityTypeConfiguration(typeof(ProductDataConfig))]
public class ProductData {
  public Guid Id { get; set; } = Guid.NewGuid();
  public List<Product> Product { get; set; } = new();

  public List<ProductDataEntry> Data { get; set; } = new();
}
public class ProductDataEntry {
  public Guid AttributeId { get; set; }
  public object Value { get; set; } = default!;
}


public class AttributeMetaConfig : IEntityTypeConfiguration<AttributeMeta> {
  public void Configure(EntityTypeBuilder<AttributeMeta> builder) {
    builder.HasKey(a => a.Id);

    builder.Property(a => a.Name)
      .IsRequired()
      .HasMaxLength(255);

    builder.Property(a => a.DataType)
      .HasMaxLength(100);

    builder.Property(a => a.AvailableValues)
      .HasColumnType("jsonb")
      .HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!
      );
    builder.HasData(
        new AttributeMeta {
          Id = Guid.Parse("d346573c-be0e-4f8c-91fb-053ce6166564"),
          Name = "VideoUrl",
          DataType = "string",
          Unit = null,
          IsFilterable = false,
          IsSortable = false
        }
    );
  }
}

[EntityTypeConfiguration(typeof(AttributeMetaConfig))]
public class AttributeMeta {
  public Guid Id { get; set; }
  public required string Name { get; set; }
  public string? DataType { get; set; }
  public string? Unit { get; set; }
  public bool IsFilterable { get; set; }
  public bool IsSortable { get; set; }
  public List<string> AvailableValues { get; set; } = new();
}

