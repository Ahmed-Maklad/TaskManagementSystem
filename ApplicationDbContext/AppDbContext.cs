using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    public DbSet<UserTask> UserTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole<int> { Id = 2, Name = "User", NormalizedName = "USER" }
        );

        var hasher = new PasswordHasher<ApplicationUser>();

        var adminUser = new ApplicationUser
        {
            Id = 1,
            FullName = "Ahmed Maklad",
            UserName = "ahmedmaklad",
            NormalizedUserName = "AHMEDMAKLAD",
            Email = "ahmedmaklad@gmail.com",
            NormalizedEmail = "AHMEDMAKLAD@GMAIL.COM",
            EmailConfirmed = true,
            PhoneNumber = "01016461212",
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Foni123456");

        modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
             new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
        );

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.ToTable("UserTasks");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Priority)
                .IsRequired();

            entity.Property(e => e.DueDate)
                .IsRequired();

            entity.Property(e => e.AssignedUserId)
                .IsRequired();

            entity.HasOne(e => e.AssignedUser)
                .WithMany()
                .HasForeignKey(e => e.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Priority)
                .HasDatabaseName("UserTask_Priority")
                .IsUnique(false)
                .IsClustered(false);
        });
    }
}

