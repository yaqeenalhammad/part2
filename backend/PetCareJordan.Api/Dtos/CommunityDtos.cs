using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Dtos;

public record AdoptionListingDto(
    int Id,
    int PetId,
    string PetName,
    PetType PetType,
    string Breed,
    string PhotoUrl,
    string City,
    string Story,
    string ContactMethod,
    string ContactDetails,
    AdoptionStatus Status,
    DateTime PostedAtUtc);

public record CreateAdoptionListingRequest(int PetId, string Story, string ContactMethod, string ContactDetails);

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
    ReportStatus Status);

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
    ReportStatus Status);

public record CreateFoundPetReportRequest(
    PetType PetType,
    string Description,
    string FoundPlace,
    DateTime FoundDateUtc,
    string PhotoUrl,
    string ContactName,
    string ContactPhone);

public record NotificationDto(int Id, string Title, string Message, DateTime TriggerDateUtc, bool IsRead);

public record UploadedImageDto(string Url, string FileName);
