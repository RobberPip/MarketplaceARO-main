using DB.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace DB;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> {
    private readonly SeedIdentity _seedIdentity;
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<AttributeMeta> AttributeMeta { get; set; }
    public DbSet<ProductData> ProductsData { get; set; }
    public DbSet<Invitations> Invitations { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Entities.File> Files { get; set; }
    public DbSet<ProfileIndustry> CompanyIndustries { get; set; }
    public DbSet<PublicList> PublicList { get; set; }
    public DbSet<MainActivity> MainActivity { get; set; }
    public DbSet<ProfileRole> ProfileRoles { get; set; }
    public DbSet<ProfileCompany> UserCompanies { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Company> Companies { get; set; }


    public AppDbContext(SeedIdentity seed, IWebHostEnvironment environment, IConfiguration configuration,
        DbContextOptions<AppDbContext> options) : base(options) {
        _seedIdentity = seed;
        this.environment = environment;
        this.configuration = configuration;

        if (!Database.CanConnect()) //TODO don't use if database doesn't exist
            throw new Exception("Couldn't connect to the database");

        if (this.environment.IsEnvironment("Testing")) {
            Database.EnsureCreated();
        } else if (this.environment.IsDevelopment()) {
            if (base.Database.GetPendingMigrations().Any())
                base.Database.Migrate();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        var schema = configuration.GetValue<string>("DbSchema");
        schema = string.IsNullOrWhiteSpace(schema) ? "public" : schema;

        modelBuilder.HasDefaultSchema(schema);
        _seedIdentity.Seed(modelBuilder);
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

