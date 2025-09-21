using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class Client : BaseEntity, IAggregateRoot
{
    public string Code { get; private set; } = string.Empty; // Siglas
    public string ShortName { get; private set; } = string.Empty; // Siglas o nombre corto
    public string FullName { get; private set; } = string.Empty; // Nombre completo
    public Guid SegmentId { get; private set; }
    public bool IsGovernment { get; private set; } = true;
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Segment Segment { get; set; } = null!;
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructors
    private Client() { } // EF Core constructor

    public Client(string code, string shortName, string fullName, Guid segmentId, bool isGovernment = true)
    {
        Code = code?.Trim().ToUpperInvariant() ?? throw new ArgumentNullException(nameof(code));
        ShortName = shortName?.Trim() ?? throw new ArgumentNullException(nameof(shortName));
        FullName = fullName?.Trim() ?? throw new ArgumentNullException(nameof(fullName));
        SegmentId = segmentId;
        IsGovernment = isGovernment;
        IsActive = true;
    }

    // Domain methods
    public void UpdateInfo(string shortName, string fullName, Guid? segmentId = null, bool? isGovernment = null)
    {
        ShortName = shortName?.Trim() ?? throw new ArgumentNullException(nameof(shortName));
        FullName = fullName?.Trim() ?? throw new ArgumentNullException(nameof(fullName));
        if (segmentId.HasValue) SegmentId = segmentId.Value;
        if (isGovernment.HasValue) IsGovernment = isGovernment.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Static helper for normalization
    public static string NormalizeClientName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return name.Trim()
            .Replace("  ", " ")
            .Replace("\n", " ")
            .Replace("\r", "");
    }

    public static string GenerateCode(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "UNKNOWN";

        // Try to extract acronym from words
        var words = name.Split(new[] { ' ', '.', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 1)
        {
            var acronym = string.Join("", words.Select(w => w.FirstOrDefault())).ToUpperInvariant();
            if (acronym.Length >= 2 && acronym.Length <= 10)
                return acronym;
        }

        // If single word or acronym too long, use first 10 characters
        var singleWord = words.FirstOrDefault() ?? name;
        return singleWord.Substring(0, Math.Min(10, singleWord.Length)).ToUpperInvariant();
    }
}