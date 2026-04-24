using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Dtos;

public record PetSummaryDto(
    int Id,
    string Name,
    PetType Type,
    string Breed,
    string City,
    string CollarId,
    string PhotoUrl,
    string OwnerName,
    AdoptionStatus? AdoptionStatus);

public record PetDetailsDto(
    int Id,
    string Name,
    PetType Type,
    string Breed,
    int AgeInMonths,
    PetGender Gender,
    string CollarId,
    string Color,
    string City,
    decimal WeightKg,
    bool IsNeutered,
    string Description,
    string PhotoUrl,
    string OwnerName,
    string OwnerPhone,
    IEnumerable<MedicalRecordDto> MedicalHistory,
    IEnumerable<VaccinationDto> Vaccines);

public record CreatePetRequest(
    string Name,
    PetType Type,
    string Breed,
    int AgeInMonths,
    PetGender Gender,
    string CollarId,
    string Color,
    string City,
    decimal WeightKg,
    bool IsNeutered,
    string Description,
    string PhotoUrl,
    int OwnerId,
    bool PublishForAdoption,
    string? AdoptionStory,
    string? ContactMethod,
    string? ContactDetails);

public record CreateMedicalRecordRequest(int PetId, int VetId, string VisitReason, string Diagnosis, string Treatment, DateTime VisitDateUtc);

public record UpdateMedicalRecordRequest(string VisitReason, string Diagnosis, string Treatment, DateTime VisitDateUtc);

public record MedicalRecordDto(int Id, string VetName, string VisitReason, string Diagnosis, string Treatment, DateTime VisitDateUtc);

public record VaccinationDto(int Id, string VetName, string VaccineName, DateTime? GivenOnUtc, DateTime DueDateUtc, bool IsCompleted);

public record UserPetVaccinePlanDto(
    int Id,
    string VaccineName,
    DateTime? GivenOnUtc,
    DateTime DueDateUtc,
    bool IsCompleted,
    string Status);

public record UserPetMedicalSnapshotDto(
    int PetId,
    string CollarId,
    string PetName,
    PetType PetType,
    string Breed,
    string PhotoUrl,
    string HealthSummary,
    bool IsVaccinesUpToDate,
    int PendingVaccinesCount,
    IEnumerable<MedicalRecordDto> MedicalHistory,
    IEnumerable<UserPetVaccinePlanDto> VaccinePlan);
