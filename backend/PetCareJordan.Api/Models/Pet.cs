namespace PetCareJordan.Api.Models;

public class Pet
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PetType Type { get; set; }
    public string Breed { get; set; } = string.Empty;
    public int AgeInMonths { get; set; }
    public PetGender Gender { get; set; }
    public string CollarId { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public bool IsNeutered { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public AppUser? Owner { get; set; }
    public AdoptionListing? AdoptionListing { get; set; }
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<VaccinationRecord> Vaccinations { get; set; } = new List<VaccinationRecord>();
}
