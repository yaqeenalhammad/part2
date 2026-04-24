using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Data;
using PetCareJordan.Api.Dtos;
using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunityController(PetCareJordanContext context) : ControllerBase
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;

    [HttpGet("lost")]
    public async Task<ActionResult<IEnumerable<LostPetReportDto>>> GetLostPets()
    {
        var reports = await context.LostPetReports
            .Where(report => report.Status == ReportStatus.Active)
            .OrderByDescending(report => report.LastSeenDateUtc)
            .Select(report => new LostPetReportDto(
                report.Id,
                report.PetName,
                report.PetType,
                report.Description,
                report.ApproximateAgeInMonths,
                report.LastSeenPlace,
                report.LastSeenDateUtc,
                report.RewardAmount,
                report.PhotoUrl,
                report.ContactName,
                report.ContactPhone,
                report.Status))
            .ToListAsync();

        return Ok(reports);
    }

    [Authorize(Roles = "User")]
    [HttpPost("lost")]
    public async Task<ActionResult<LostPetReportDto>> CreateLostPetReport(CreateLostPetReportRequest request)
    {
        var report = new LostPetReport
        {
            PetName = request.PetName,
            PetType = request.PetType,
            Description = request.Description,
            ApproximateAgeInMonths = request.ApproximateAgeInMonths,
            LastSeenPlace = request.LastSeenPlace,
            LastSeenDateUtc = request.LastSeenDateUtc,
            RewardAmount = request.RewardAmount,
            PhotoUrl = request.PhotoUrl,
            ContactName = request.ContactName,
            ContactPhone = request.ContactPhone,
            Status = ReportStatus.Pending
        };

        context.LostPetReports.Add(report);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLostPets), new LostPetReportDto(report.Id, report.PetName, report.PetType, report.Description, report.ApproximateAgeInMonths, report.LastSeenPlace, report.LastSeenDateUtc, report.RewardAmount, report.PhotoUrl, report.ContactName, report.ContactPhone, report.Status));
    }

    [HttpGet("found")]
    public async Task<ActionResult<IEnumerable<FoundPetReportDto>>> GetFoundPets()
    {
        var reports = await context.FoundPetReports
            .Where(report => report.Status == ReportStatus.Active)
            .OrderByDescending(report => report.FoundDateUtc)
            .Select(report => new FoundPetReportDto(
                report.Id,
                report.PetType,
                report.Description,
                report.FoundPlace,
                report.FoundDateUtc,
                report.PhotoUrl,
                report.ContactName,
                report.ContactPhone,
                report.Status))
            .ToListAsync();

        return Ok(reports);
    }

    [Authorize(Roles = "User")]
    [HttpPost("found")]
    public async Task<ActionResult<FoundPetReportDto>> CreateFoundPetReport(CreateFoundPetReportRequest request)
    {
        var report = new FoundPetReport
        {
            PetType = request.PetType,
            Description = request.Description,
            FoundPlace = request.FoundPlace,
            FoundDateUtc = request.FoundDateUtc,
            PhotoUrl = request.PhotoUrl,
            ContactName = request.ContactName,
            ContactPhone = request.ContactPhone,
            Status = ReportStatus.Pending
        };

        context.FoundPetReports.Add(report);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFoundPets), new FoundPetReportDto(report.Id, report.PetType, report.Description, report.FoundPlace, report.FoundDateUtc, report.PhotoUrl, report.ContactName, report.ContactPhone, report.Status));
    }

    [Authorize(Roles = "User")]
    [HttpPost("upload-image")]
    public async Task<ActionResult<UploadedImageDto>> UploadImage([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Please choose an image file.");
        }

        if (file.Length > MaxImageSizeBytes)
        {
            return BadRequest("Image size must be 5 MB or less.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedImageExtensions.Contains(extension))
        {
            return BadRequest("Only .jpg, .jpeg, .png, and .webp images are allowed.");
        }

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var physicalPath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var publicUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(new UploadedImageDto(publicUrl, fileName));
    }

    [HttpGet("notifications/{userId:int}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(int userId)
    {
        var notifications = await context.Notifications
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.TriggerDateUtc)
            .Select(notification => new NotificationDto(notification.Id, notification.Title, notification.Message, notification.TriggerDateUtc, notification.IsRead))
            .ToListAsync();

        return Ok(notifications);
    }
}
