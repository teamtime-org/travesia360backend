using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamTime.Infrastructure.Identity;

namespace TeamTime.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Configure additional properties
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("first_name");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("last_name");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnName("role");

        builder.Property(u => u.AreaId)
            .HasColumnName("area_id");

        builder.Property(u => u.JobTitleId)
            .HasColumnName("job_title_id");

        builder.Property(u => u.EmployeeCode)
            .HasMaxLength(50)
            .HasColumnName("employee_code");

        builder.Property(u => u.DateOfJoining)
            .HasColumnName("date_of_joining");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(u => u.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(u => u.LastModifiedBy)
            .HasColumnName("last_modified_by");

        // Configure inherited Identity properties
        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.UserName)
            .HasColumnName("user_name");

        builder.Property(u => u.NormalizedUserName)
            .HasColumnName("normalized_user_name");

        builder.Property(u => u.Email)
            .HasColumnName("email");

        builder.Property(u => u.NormalizedEmail)
            .HasColumnName("normalized_email");

        builder.Property(u => u.EmailConfirmed)
            .HasColumnName("email_confirmed");

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash");

        builder.Property(u => u.SecurityStamp)
            .HasColumnName("security_stamp");

        builder.Property(u => u.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp");

        builder.Property(u => u.PhoneNumber)
            .HasColumnName("phone_number");

        builder.Property(u => u.PhoneNumberConfirmed)
            .HasColumnName("phone_number_confirmed");

        builder.Property(u => u.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled");

        builder.Property(u => u.LockoutEnd)
            .HasColumnName("lockout_end");

        builder.Property(u => u.LockoutEnabled)
            .HasColumnName("lockout_enabled");

        builder.Property(u => u.AccessFailedCount)
            .HasColumnName("access_failed_count");

        // Relationships
        builder.HasOne(u => u.Area)
            .WithMany()
            .HasForeignKey(u => u.AreaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.JobTitle)
            .WithMany()
            .HasForeignKey(u => u.JobTitleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.ProjectAssignments)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.CreatedProjects)
            .WithOne()
            .HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.CreatedTasks)
            .WithOne()
            .HasForeignKey("CreatedBy")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.TimeEntries)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ApprovedTimeEntries)
            .WithOne()
            .HasForeignKey("ApprovedById")
            .OnDelete(DeleteBehavior.SetNull);
    }
}