namespace PetCareJordan.Api.Models;

public class FoundPetReport
{
    public int Id { get; set; }
    public PetType PetType { get; set; }
    public string Description { get; set; } = string.Empty;
    public string FoundPlace { get; set; } = string.Empty;
    public DateTime FoundDateUtc { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public int? ReporterId { get; set; }
    public AppUser? Reporter { get; set; }
}
