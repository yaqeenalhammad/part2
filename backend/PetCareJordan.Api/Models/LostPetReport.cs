namespace PetCareJordan.Api.Models;

public class LostPetReport
{
    public int Id { get; set; }
    public string PetName { get; set; } = string.Empty;
    public PetType PetType { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ApproximateAgeInMonths { get; set; }
    public string LastSeenPlace { get; set; } = string.Empty;
    public DateTime LastSeenDateUtc { get; set; }
    public decimal? RewardAmount { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public int? ReporterId { get; set; }
    public AppUser? Reporter { get; set; }
}
