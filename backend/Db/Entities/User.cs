using Db.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB.Entities;

[EntityTypeConfiguration(typeof(UserConfig))]
public class User : IdentityUser<Guid>, IEntity {

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
public class UserConfig : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {


        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}