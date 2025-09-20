using Microsoft.AspNetCore.Identity;

namespace TeamTime.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ApplicationRole() : base()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ApplicationRole(string roleName, string description = "") : base(roleName)
    {
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    // Predefined roles
    public static class Names
    {
        public const string ADMINISTRADOR = "ADMINISTRADOR";
        public const string COORDINADOR = "COORDINADOR";
        public const string COLABORADOR = "COLABORADOR";
    }
}