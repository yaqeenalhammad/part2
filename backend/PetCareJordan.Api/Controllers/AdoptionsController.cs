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
            .OrderByDescending(listing => listing.PostedAtUtc)
            .Select(listing => new AdoptionListingDto(
                listing.Id,
                listing.PetId,
                listing.Pet!.Name,
                listing.Pet.Type,
                listing.Pet.Breed,
                listing.Pet.PhotoUrl,
                listing.Pet.City,
                listing.Story,
                listing.ContactMethod,
                listing.ContactDetails,
                listing.Status,
                listing.PostedAtUtc))
            .ToListAsync();

        return Ok(listings);
    }

    [HttpPost]
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

        return CreatedAtAction(nameof(GetAdoptionListings), new AdoptionListingDto(listing.Id, pet.Id, pet.Name, pet.Type, pet.Breed, pet.PhotoUrl, pet.City, listing.Story, listing.ContactMethod, listing.ContactDetails, listing.Status, listing.PostedAtUtc));
    }
}
