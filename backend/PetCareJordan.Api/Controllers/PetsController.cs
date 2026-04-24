using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Data;
using PetCareJordan.Api.Dtos;
using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(PetCareJordanContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PetSummaryDto>>> GetPets([FromQuery] string? search, [FromQuery] PetType? type)
    {
        var query = context.Pets
            .Include(pet => pet.Owner)
            .Include(pet => pet.AdoptionListing)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(pet =>
                pet.Name.Contains(search) ||
                pet.Breed.Contains(search) ||
                pet.City.Contains(search) ||
                pet.CollarId.Contains(search));
        }

        if (type.HasValue)
        {
            query = query.Where(pet => pet.Type == type.Value);
        }

        var pets = await query
            .OrderBy(pet => pet.Name)
            .Select(pet => new PetSummaryDto(
                pet.Id,
                pet.Name,
                pet.Type,
                pet.Breed,
                pet.City,
                pet.CollarId,
                pet.PhotoUrl,
                pet.Owner!.FullName,
                pet.AdoptionListing != null ? pet.AdoptionListing.Status : null))
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PetDetailsDto>> GetPet(int id)
    {
        var pet = await context.Pets
            .Include(item => item.Owner)
            .Include(item => item.MedicalRecords)
                .ThenInclude(item => item.Vet)
            .Include(item => item.Vaccinations)
                .ThenInclude(item => item.Vet)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (pet is null || pet.Owner is null)
        {
            return NotFound();
        }

        return Ok(new PetDetailsDto(
            pet.Id,
            pet.Name,
            pet.Type,
            pet.Breed,
            pet.AgeInMonths,
            pet.Gender,
            pet.CollarId,
            pet.Color,
            pet.City,
            pet.WeightKg,
            pet.IsNeutered,
            pet.Description,
            pet.PhotoUrl,
            pet.Owner.FullName,
            pet.Owner.PhoneNumber,
            pet.MedicalRecords.OrderByDescending(item => item.VisitDateUtc).Select(item => new MedicalRecordDto(item.Id, item.Vet!.FullName, item.VisitReason, item.Diagnosis, item.Treatment, item.VisitDateUtc)),
            pet.Vaccinations.OrderBy(item => item.DueDateUtc).Select(item => new VaccinationDto(item.Id, item.Vet!.FullName, item.VaccineName, item.GivenOnUtc, item.DueDateUtc, item.IsCompleted))));
    }

    [HttpGet("collar/{collarId}")]
    public async Task<ActionResult<PetSummaryDto>> GetByCollarId(string collarId)
    {
        var pet = await context.Pets
            .Include(item => item.Owner)
            .Include(item => item.AdoptionListing)
            .FirstOrDefaultAsync(item => item.CollarId == collarId);

        if (pet is null || pet.Owner is null)
        {
            return NotFound();
        }

        return Ok(new PetSummaryDto(pet.Id, pet.Name, pet.Type, pet.Breed, pet.City, pet.CollarId, pet.PhotoUrl, pet.Owner.FullName, pet.AdoptionListing?.Status));
    }

    [HttpPost]
    public async Task<ActionResult<PetDetailsDto>> CreatePet(CreatePetRequest request)
    {
        var owner = await context.Users.FirstOrDefaultAsync(user => user.Id == request.OwnerId);
        if (owner is null)
        {
            return BadRequest("Owner not found.");
        }

        var pet = new Pet
        {
            Name = request.Name,
            Type = request.Type,
            Breed = request.Breed,
            AgeInMonths = request.AgeInMonths,
            Gender = request.Gender,
            CollarId = request.CollarId,
            Color = request.Color,
            City = request.City,
            WeightKg = request.WeightKg,
            IsNeutered = request.IsNeutered,
            Description = request.Description,
            PhotoUrl = request.PhotoUrl,
            OwnerId = request.OwnerId
        };

        context.Pets.Add(pet);
        await context.SaveChangesAsync();

        if (request.PublishForAdoption)
        {
            context.AdoptionListings.Add(new AdoptionListing
            {
                PetId = pet.Id,
                Story = request.AdoptionStory ?? "Available for adoption",
                ContactMethod = request.ContactMethod ?? "Phone",
                ContactDetails = request.ContactDetails ?? owner.PhoneNumber,
                Status = AdoptionStatus.Available,
                PostedAtUtc = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        return await GetPet(pet.Id);
    }
}
