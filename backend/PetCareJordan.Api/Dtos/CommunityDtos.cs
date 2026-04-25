using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Dtos;

public record AdoptionListingDto(
    int Id,
    int PetId,
    string PetName,
    PetType PetType,
    string Breed,
    decimal WeightKg,
    string PhotoUrl,
    string City,
    string Story,
    string ContactMethod,
    string ContactDetails,
    AdoptionStatus Status,
    DateTime PostedAtUtc);

public record CreateAdoptionListingRequest(int PetId, string Story, string ContactMethod, string ContactDetails);

public record CreateAdoptionPostRequest(
    string PetName,
    PetType PetType,
    decimal WeightKg,
    string City,
    string PhotoUrl,
    string Description,
    string ContactPhone);

public record LostPetReportDto(
    int Id,
    string PetName,
    PetType PetType,
    string Description,
    int ApproximateAgeInMonths,
    string LastSeenPlace,
    DateTime LastSeenDateUtc,
    decimal? RewardAmount,
    string PhotoUrl,
    string ContactName,
    string ContactPhone,
    ReportStatus Status,
    int? ReporterId,
    string? ReporterName);

public record CreateLostPetReportRequest(
    string PetName,
    PetType PetType,
    string Description,
    int ApproximateAgeInMonths,
    string LastSeenPlace,
    DateTime LastSeenDateUtc,
    decimal? RewardAmount,
    string PhotoUrl,
    string ContactName,
    string ContactPhone);

public record FoundPetReportDto(
    int Id,
    PetType PetType,
    string Description,
    string FoundPlace,
    DateTime FoundDateUtc,
    string PhotoUrl,
    string ContactName,
    string ContactPhone,
    ReportStatus Status,
    int? ReporterId,
    string? ReporterName);

public record CreateFoundPetReportRequest(
    PetType PetType,
    string Description,
    string FoundPlace,
    DateTime FoundDateUtc,
    string PhotoUrl,
    string ContactName,
    string ContactPhone);

public record PendingCommunityReportsDto(
    IEnumerable<LostPetReportDto> LostReports,
    IEnumerable<FoundPetReportDto> FoundReports);

public record MyCommunityReportsDto(
    IEnumerable<LostPetReportDto> LostReports,
    IEnumerable<FoundPetReportDto> FoundReports);

public record NotificationDto(int Id, string Title, string Message, DateTime TriggerDateUtc, bool IsRead);

public record UploadedImageDto(string Url, string FileName);
