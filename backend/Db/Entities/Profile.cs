using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

[EntityTypeConfiguration(typeof(ProfileConfig))]
public class Profile : IEntity, IAvatarOwner {
    public Guid Id { get; set; }
    public Guid? ActiveAccountId { get; set; }
    public Account? ActiveAccount { get; set; }
    public required string Email { get; set; }
    public required string Login { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // TODO: delete, rename?
    public string? Description { get; set; }

    public Guid? AvatarId { get; set; }
    public File? Avatar { get; set; }

    public List<Company> Companies { get; } = [];
    public List<ProfileCompany> ProfileCompanies { get; } = [];

    #region Links: ProfileCompany, Industry, Roles
    public Guid? CurrentCompanyId { get; set; }
    /// <summary>
    /// Компания, указанная в профиле пользователем
    /// </summary>
    public Company? ProfileCompany { get; set; }


    public Guid? ProfileIndustryId { get; set; }
    public ProfileIndustry? ProfileIndustry { get; set; }

    public Guid? ProfileRoleId { get; set; }
    public ProfileRole? ProfileRole { get; set; }
    #endregion

    #region Legal
    public bool PersonalDataPolitics { get; set; } = false;
    public bool PersonalDataAgreement { get; set; } = false;
    #endregion
    public List<Invitations> Invitations { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class ProfileConfig : IEntityTypeConfiguration<Profile> {
    public void Configure(EntityTypeBuilder<Profile> builder) {
        builder.Property(prop => prop.Email).IsRequired().HasMaxLength(255);
        builder.Property(prop => prop.Login).IsRequired().HasMaxLength(255);
        builder.Property(prop => prop.FirstName).HasMaxLength(150);
        builder.Property(prop => prop.Description).HasMaxLength(255);
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(profile => profile.ProfileIndustry).WithMany(pi => pi.Profiles);
        builder.HasOne(profile => profile.ProfileRole).WithMany(pr => pr.Profiles);

        builder
            .HasOne(u => u.ActiveAccount)
            .WithMany()
            .HasForeignKey(u => u.ActiveAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Avatar)
            .WithMany()
            .HasForeignKey(p => p.AvatarId)
            .IsRequired(false);
    }
}

#region Profile Properties
[EntityTypeConfiguration(typeof(ProfileIndustryConfig))]
public class ProfileIndustry {
    public required Guid Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Profile> Profiles { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class ProfileIndustryConfig : IEntityTypeConfiguration<ProfileIndustry> {
    public void Configure(EntityTypeBuilder<ProfileIndustry> builder) {
        builder.Property(prop => prop.Name).HasMaxLength(150).IsRequired();
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

        var industriesData = new[] {
            "Автомобилестроение и транспорт",
            "Архитектура и строительство",
            "Аэрокосмическая отрасль",
            "Электроника и высокие технологии",
            "Инфраструктура",
            "Энергетика",
            "Добыча полезных ископаемых",
            "Промышленное оборудование",
            "Судостроение и морское строительство"
        };

        var industries = industriesData.Select((name, index) => new ProfileIndustry {
            Id = Guid.NewGuid(),
            Name = name,
        }).ToList();
        builder.HasData(industries);
    }
}
[EntityTypeConfiguration(typeof(ProfileRoleConfig))]
public class ProfileRole {
    public required Guid Id { get; set; }
    public required string Key { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }

    public ICollection<Profile> Profiles { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProfileRoleConfig : IEntityTypeConfiguration<ProfileRole> {
    public void Configure(EntityTypeBuilder<ProfileRole> builder) {
        builder.Property(prop => prop.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(prop => prop.Key).IsUnique();
        builder.Property(prop => prop.Key).IsRequired();
        DateTime startTime = DateTime.UtcNow;

        var rolesData = new[] {
            ("SurveySpecialist", "Специалист по инженерным изысканиям", "Изыскания"),
            ("Topographer", "Топограф", "Изыскания"),

            ("Arch", "Архитектор", "Проектирование"),
            ("LandscapeArch", "Ландшафтный архитектор", "Проектирование"),
            ("Designer", "Дизайнер", "Проектирование"),
            ("ChiefArch", "Главный архитектор проекта", "Проектирование"),
            ("TransportDev", "Специалист транспортного развития территорий", "Проектирование"),
            ("Infrastructure", "Специалист инженерного обеспечения территорий", "Проектирование"),
            ("EngineerDesigner", "Инженер-проектировщик", "Проектирование"),
            ("Estimator", "Инженер-сметчик", "Проектирование"),
            ("StructuralEngineer", "Инженер-конструктор", "Проектирование"),
            ("ChiefEngineer", "Главный инженер проекта", "Проектирование"),
            ("ProcessEngineer", "Инженер-технолог", "Проектирование"),
            ("BIMManager", "Менеджер ТИМ", "Проектирование"),

            ("ProcurementManager", "Менеджер по закупкам", "Строительство"),
            ("Mechanic", "Механик", "Строительство"),
            ("Builder", "Строитель", "Строительство"),
            ("Foreman", "Прораб", "Строительство"),
            ("QualityEngineer", "Инженер по качеству", "Строительство"),
            ("ConstructionControl", "Инженер по строительному контролю", "Строительство"),

            ("MainEngineer", "Главный инженер", "Эксплуатация"),
            ("ChiefMechanic", "Главный механик", "Эксплуатация"),
            ("ChiefPowerEngineer", "Главный энергетик", "Эксплуатация"),
            ("ChiefTechnologist", "Главный технолог", "Эксплуатация"),
            ("OperationsEngineer", "Инженер по эксплуатации", "Эксплуатация"),

            ("GenDirector", "Генеральный директор", "Управление"),
            ("Technical", "Технический директор", "Управление"),
            ("Financial", "Финансовый директор", "Управление"),
            ("ProjectManager", "Руководитель проекта", "Управление"),

            ("Other", "Другое", "Управление")
        };

        var roles = rolesData.Select((role, index) => {
            return new ProfileRole {
                Id = Guid.CreateVersion7(startTime.AddSeconds(index)),
                Key = role.Item1,
                Name = role.Item2,
                Category = role.Item3
            };
        }).ToList();

        builder.HasData(roles);
    }
}
#endregion