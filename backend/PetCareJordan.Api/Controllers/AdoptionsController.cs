using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Data;
using PetCareJordan.Api.Dtos;
using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdoptionsController(PetCareJordanContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdoptionListingDto>>> GetAdoptionListings()
    {
        var listings = await context.AdoptionListings
            .Include(listing => listing.Pet)
            .Where(listing => listing.Status == AdoptionStatus.Available)
            .OrderByDescending(listing => listing.PostedAtUtc)
            .ToListAsync();

        return Ok(listings.Select(listing => ToDto(listing, listing.Pet!)));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<AdoptionListingDto>>> GetAdminAdoptionListings()
    {
        var listings = await context.AdoptionListings
            .Include(listing => listing.Pet)
            .OrderByDescending(listing => listing.PostedAtUtc)
            .ToListAsync();

        return Ok(listings.Select(listing => ToDto(listing, listing.Pet!)));
    }

    [HttpPost]
    [Authorize(Roles = "User,Vet")]
    public async Task<ActionResult<AdoptionListingDto>> CreateAdoptionPost(CreateAdoptionPostRequest request)
    {
        var ownerId = GetCurrentUserId();
        if (ownerId is null)
        {
            return Unauthorized();
        }

        var owner = await context.Users.FirstOrDefaultAsync(user => user.Id == ownerId.Value);
        if (owner is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.PetName) ||
            string.IsNullOrWhiteSpace(request.City) ||
            string.IsNullOrWhiteSpace(request.PhotoUrl) ||
            string.IsNullOrWhiteSpace(request.Description) ||
            string.IsNullOrWhiteSpace(request.ContactPhone))
        {
            return BadRequest("Pet name, city, photo, description, and contact phone are required.");
        }

        if (request.WeightKg <= 0)
        {
            return BadRequest("Pet weight must be greater than zero.");
        }

        var pet = new Pet
        {
            Name = request.PetName.Trim(),
            Type = request.PetType,
            Breed = request.PetType.ToString(),
            AgeInMonths = 0,
            Gender = PetGender.Male,
            CollarId = $"ADOPT-{Guid.NewGuid():N}"[..18].ToUpperInvariant(),
            Color = "Unknown",
            City = request.City.Trim(),
            WeightKg = request.WeightKg,
            IsNeutered = false,
            Description = request.Description.Trim(),
            PhotoUrl = request.PhotoUrl.Trim(),
            OwnerId = owner.Id
        };

        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        var listing = new AdoptionListing
        {
            PetId = pet.Id,
            Story = request.Description.Trim(),
            ContactMethod = "Phone",
            ContactDetails = request.ContactPhone.Trim(),
            Status = AdoptionStatus.Pending,
            PostedAtUtc = DateTime.UtcNow
        };

        context.AdoptionListings.Add(listing);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAdoptionListings), ToDto(listing, pet));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/{id:int}/approve")]
    public async Task<ActionResult<AdoptionListingDto>> ApproveAdoptionListing(int id)
    {
        var listing = await context.AdoptionListings
            .Include(item => item.Pet)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (listing?.Pet is null)
        {
            return NotFound();
        }

        listing.Status = AdoptionStatus.Available;
        await context.SaveChangesAsync();

        return Ok(ToDto(listing, listing.Pet));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("admin/{id:int}/reject")]
    public async Task<ActionResult<AdoptionListingDto>> RejectAdoptionListing(int id)
    {
        var listing = await context.AdoptionListings
            .Include(item => item.Pet)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (listing?.Pet is null)
        {
            return NotFound();
        }

        listing.Status = AdoptionStatus.Rejected;
        await context.SaveChangesAsync();

        return Ok(ToDto(listing, listing.Pet));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id:int}")]
    public async Task<IActionResult> DeleteAdoptionListing(int id)
    {
        var listing = await context.AdoptionListings.FindAsync(id);
        if (listing is null)
        {
            return NotFound();
        }

        context.AdoptionListings.Remove(listing);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("existing")]
    public async Task<ActionResult<AdoptionListingDto>> CreateAdoptionListing(CreateAdoptionListingRequest request)
    {
        var pet = await context.Pets.FindAsync(request.PetId);
        if (pet is null)
        {
            return BadRequest("Pet not found.");
        }

        var listing = new AdoptionListing
        {
            PetId = request.PetId,
            Story = request.Story,
            ContactMethod = request.ContactMethod,
            ContactDetails = request.ContactDetails,
            Status = AdoptionStatus.Available,
            PostedAtUtc = DateTime.UtcNow
        };

        context.AdoptionListings.Add(listing);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAdoptionListings), ToDto(listing, pet));
    }

    private static AdoptionListingDto ToDto(AdoptionListing listing, Pet pet) =>
        new(
            listing.Id,
            pet.Id,
            pet.Name,
            pet.Type,
            pet.Breed,
            pet.WeightKg,
            pet.PhotoUrl,
            pet.City,
            listing.Story,
            listing.ContactMethod,
            listing.ContactDetails,
            listing.Status,
            listing.PostedAtUtc);

    private int? GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var userId) ? userId : null;
    }
}
