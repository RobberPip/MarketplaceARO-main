using System.Text.Json.Serialization;
using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

public class AccountConfig : IEntityTypeConfiguration<Account> {
  public void Configure(EntityTypeBuilder<Account> builder) {
    builder.Property(account => account.Status)
      .IsRequired()
      .HasDefaultValue(AccountStatus.Pending)
      .HasConversion<string>();
    builder
      .Property(account => account.Type)
      .IsRequired()
      .HasDefaultValue(AccountType.Regular)
      .HasConversion<string>();
    builder.Property(account => account.IsDeveloper)
          .HasDefaultValue(false)
          .IsRequired();
    builder.Property(account => account.INN).HasMaxLength(12);
    builder.Property(account => account.FullName).HasMaxLength(255);
    builder.Property(account => account.Address).HasMaxLength(500);
    builder.Property(account => account.KPP).HasMaxLength(9);
    builder.Property(account => account.OGRN).HasMaxLength(15);

    builder
      .HasMany(a => a.Invitations)
      .WithOne(ap => ap.Account)
      .HasForeignKey(ap => ap.AccountId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);
  }
}

[EntityTypeConfiguration(typeof(AccountConfig))]

public class Account : IEntity {
  public Guid Id { get; set; }

  public List<Invitations> Invitations { get; set; } = new();
  public Guid? BrandId { get; set; }
  public Brand? Brand { get; set; }
  /// <summary>
  /// Является ли аккаунт разработчиком
  /// </summary>
  public required bool IsDeveloper { get; set; } = false;

  public required AccountStatus Status { get; set; }
  public required AccountType Type { get; set; }
  /// <summary>
  /// Короткое имя
  /// </summary>
  public string? ShortName { get; set; }
  /// <summary>
  /// Полное имя
  /// </summary>
  public string? FullName { get; set; }
  /// <summary>
  /// ИНН
  /// </summary>
  public string? INN { get; set; }
  /// <summary>
  /// Адрес
  /// </summary>
  public string? Address { get; set; }
  /// <summary>
  /// КПП
  /// </summary>
  public string? KPP { get; set; }
  /// <summary>
  /// ОГРН
  /// </summary>
  public string? OGRN { get; set; }
  public List<Product> OwnedProducts { get; set; } = new();
  public List<Product> RightsProducts { get; set; } = new();
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountStatus {
  Approved,
  Pending,
  Blocked
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountType {
  /// <summary>
  /// Обычный пользователь
  /// </summary>
  Regular,
  /// <summary>
  /// Самозанятый
  /// </summary>
  SelfEmployed,
  /// <summary>
  /// Индивидуальный предприниматель (ИП)
  /// </summary>
  IndividualEntrepreneur,
  /// <summary>
  /// Юридическое лицо
  /// </summary>
  LegalEntity,
  /// <summary>
  /// Общество с ограниченной ответственностью (ООО)
  /// </summary>
  LimitedLiabilityCompany,

  /// <summary>
  /// Акционерное общество (АО)
  /// </summary>
  JointStockCompany,

  /// <summary>
  /// Некоммерческая организация (НКО)
  /// </summary>
  NonProfitOrganization,

  /// <summary>
  /// Физическое лицо
  /// </summary>
  Individual,

  /// <summary>
  /// Государственное учреждение
  /// </summary>
  GovernmentInstitution,

  /// <summary>
  /// Частное предприятие
  /// </summary>
  PrivateEnterprise,

  /// <summary>
  /// Иностранная организация
  /// </summary>
  ForeignOrganization,

  /// <summary>
  /// Иностранное физическое лицо
  /// </summary>
  ForeignIndividual
}
public class InvitationsConfig : IEntityTypeConfiguration<Invitations> {
  public void Configure(EntityTypeBuilder<Invitations> builder) {
    builder.HasKey(ap => ap.Id);

    builder.Property(ap => ap.Email).HasMaxLength(255);

    builder.Property(ap => ap.Role)
      .IsRequired()
      .HasConversion<string>();

    builder.Property(ap => ap.Status)
      .IsRequired()
      .HasConversion<string>();

    builder.HasOne(ap => ap.Account)
      .WithMany(a => a.Invitations)
      .HasForeignKey(ap => ap.AccountId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(ap => ap.Profile)
      .WithMany(p => p.Invitations)
      .HasForeignKey(ap => ap.ProfileId)
      .OnDelete(DeleteBehavior.SetNull);
  }
}
[EntityTypeConfiguration(typeof(InvitationsConfig))]
public class Invitations {
  public Guid Id { get; set; }

  public Guid AccountId { get; set; }
  public Account Account { get; set; } = null!;

  public Guid? ProfileId { get; set; }
  public Profile? Profile { get; set; }

  public string? Email { get; set; }

  public AccountRole Role { get; set; }
  public InvitationStatus Status { get; set; } = InvitationStatus.Invited;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? AcceptedAt { get; set; }
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InvitationStatus {
  Invited,
  Accepted,
  Revoked,
  Rejected
}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountRole {
  Owner,
  Manager,
  Developer,
  Member
}
