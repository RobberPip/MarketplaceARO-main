using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

[EntityTypeConfiguration(typeof(CompanyConfig))]
public class Company {
    public required Guid Id { get; set; }
    public required string? Name { get; set; }
    public string? Description { get; set; }
    public required CompanyStatus Status { get; set; }
    /// <summary>
    /// Наличие в реестре ARO
    /// </summary>

    // TODO
    public ICollection<Profile> ChildProfiles { get; set; } = [];

    public ICollection<Profile> Profiles { get; set; } = [];
    public ICollection<ProfileCompany> ProfileCompanies { get; set; } = [];

    public required bool IsRegistry { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}

public class CompanyConfig : IEntityTypeConfiguration<Company> {
    public void Configure(EntityTypeBuilder<Company> builder) {
        builder.Property(prop => prop.Status)
            .IsRequired()
            .HasDefaultValue(CompanyStatus.Init)
            .HasConversion<string>();
        builder.Property(prop => prop.Name).HasMaxLength(150);
        builder.Property(prop => prop.Description).HasMaxLength(255);
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(prop => prop.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasMany(c => c.Profiles).WithMany(p => p.Companies).UsingEntity<ProfileCompany>(
            l => l.HasOne(p => p.Profile).WithMany(e => e.ProfileCompanies).HasForeignKey(pc => pc.ProfileId),
            r => r.HasOne(p => p.Company).WithMany(e => e.ProfileCompanies).HasForeignKey(pc => pc.CompanyId),
            pc => {
                pc.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                pc.Property(prop => prop.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                pc.Property(prop => prop.Type)
                    .IsRequired()
                    .HasConversion<string>();
            });

        builder
          .HasMany(c => c.ChildProfiles)
          .WithOne(p => p.ProfileCompany)
          .HasForeignKey(e => e.CurrentCompanyId);

        // TODO: temp
        var seedCompanies = new List<Company>() {
            new() {
                Id = Guid.CreateVersion7(),
                Name = "Компания 1",
                Status = CompanyStatus.Approved,
                IsRegistry = true
            },
            new() {
                Id = Guid.CreateVersion7(),
                Name = "Компания 2",
                Status = CompanyStatus.Approved,
                IsRegistry = true
            },
            new() {
                Id = Guid.CreateVersion7(),
                Name = "Компания 3",
                Status = CompanyStatus.Approved,
                IsRegistry = true
            },
            new() {
                Id = Guid.CreateVersion7(),
                Name = "Компания 4",
                Status = CompanyStatus.Init,
                IsRegistry = true
            },

        };
        builder.HasData(seedCompanies);
    }

}

public class ProfileCompany {
    public required Guid ProfileId { get; set; }
    public Profile Profile { get; set; } = null!;
    public required Guid CompanyId { get; set; }
    public required Company Company { get; set; }

    public required ProfileCompanyType Type { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProfileCompanyType {
    /// <summary>
    /// Админ
    /// </summary>
    Admin,
    /// <summary>
    /// Сотрудник
    /// </summary>
    Employee,
    /// <summary>
    /// Владелец
    /// </summary>
    Owner,

    // это примеры возможных связей
    Client,
    Expert,
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CompanyStatus {
    /// <summary>
    /// Не подтвержден
    /// </summary>
    Init,
    /// <summary>
    /// Подтвержден
    /// </summary>
    Approved,
    /// <summary>
    /// На модерации
    /// </summary>
    Pending
}
