namespace PetCareJordan.Api.Models;

public class AdoptionListing
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
    public string Story { get; set; } = string.Empty;
    public string ContactMethod { get; set; } = string.Empty;
    public string ContactDetails { get; set; } = string.Empty;
    public AdoptionStatus Status { get; set; }
    public DateTime PostedAtUtc { get; set; }
}
