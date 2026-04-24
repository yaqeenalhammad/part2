namespace PetCareJordan.Api.Dtos;

public record DashboardSummaryDto(
    int TotalUsers,
    int TotalVets,
    int TotalPets,
    int PetsForAdoption,
    int LostReports,
    int FoundReports,
    int UpcomingVaccines,
    IDictionary<string, int> PetsByType,
    IDictionary<string, int> PetsByCity);
