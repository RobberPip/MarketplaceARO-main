using System.Text.Json.Serialization;
using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

public class BrandConfig : IEntityTypeConfiguration<Brand> {
    public void Configure(EntityTypeBuilder<Brand> builder) {
        builder.HasIndex(b => b.PublicName).IsUnique();
        builder.Property(brand => brand.PublicName).HasMaxLength(255);
        builder.Property(brand => brand.Description).HasMaxLength(1000);
        builder.Property(brand => brand.Website).HasMaxLength(255);
        builder.Property(brand => brand.Email).HasMaxLength(255);
        builder.Property(brand => brand.PhoneNumber).HasMaxLength(255);
        builder
            .Property(brand => brand.Status)
            .IsRequired()
            .HasDefaultValue(BrandStatus.Pending)
            .HasConversion<string>();

        builder.HasMany(b => b.Accounts)
              .WithOne(a => a.Brand)
              .HasForeignKey(a => a.BrandId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Avatar)
                .WithMany()
                .HasForeignKey(p => p.AvatarId)
                .IsRequired(false);
    }
}

[EntityTypeConfiguration(typeof(BrandConfig))]

public class Brand : IEntity, IAvatarOwner {
    public Guid Id { get; set; }

    public ICollection<Account> Accounts { get; set; } = [];
    public Guid? AvatarId { get; set; }
    public required BrandStatus Status { get; set; }
    public File? Avatar { get; set; }
    /// <summary>
    /// Публичное имя
    /// </summary>
    public string? PublicName { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Основная деятельность
    /// </summary>
    public List<MainActivity>? MainActivities { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<PublicList>? PublicLists { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BrandStatus {
    Approved,
    Pending,
    Blocked
}
[EntityTypeConfiguration(typeof(PublicListConfig))]
public class PublicList {
    public required Guid Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Brand> Brands { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PublicListConfig : IEntityTypeConfiguration<PublicList> {
    public void Configure(EntityTypeBuilder<PublicList> builder) {
        builder.Property(prop => prop.Name).HasMaxLength(150).IsRequired();
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        var publicListsData = new[] {
            "НОТИМ",
            "НОТЕХ",
            "НОПРИЗ",
            "НОСТРОЙ",
        };

        var publicLists = publicListsData.Select((name, index) => new PublicList {
            Id = Guid.NewGuid(),
            Name = name,
        }).ToList();
        builder.HasData(publicLists);
    }
}
[EntityTypeConfiguration(typeof(MainActivityConfig))]
public class MainActivity {
    public required Guid Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Brand> Brands { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class MainActivityConfig : IEntityTypeConfiguration<MainActivity> {
    public void Configure(EntityTypeBuilder<MainActivity> builder) {
        builder.Property(prop => prop.Name).HasMaxLength(150).IsRequired();
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        var mainActivityData = new[] {
        "Проектирование",
        "Строительство",
        "Эксплуатация",
        "Добыча полезных ископаемых",
        "Машиностроение",
        "Судостроение",
        "Автомобилестроение",
        "Разработка программного обеспечения",
        "Другое"
    };


        var mainActivity = mainActivityData.Select((name, index) => new MainActivity {
            Id = Guid.NewGuid(),
            Name = name,
        }).ToList();
        builder.HasData(mainActivity);
    }
}