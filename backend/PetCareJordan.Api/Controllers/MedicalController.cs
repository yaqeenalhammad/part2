using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Data;
using PetCareJordan.Api.Dtos;
using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalController(PetCareJordanContext context) : ControllerBase
{
    [HttpGet("upcoming-vaccines")]
    public async Task<ActionResult<IEnumerable<object>>> GetUpcomingVaccines()
    {
        var cutoff = DateTime.UtcNow.AddDays(30);
        var vaccines = await context.VaccinationRecords
            .Include(vaccine => vaccine.Pet)
                .ThenInclude(pet => pet!.Owner)
            .Where(vaccine => !vaccine.IsCompleted && vaccine.DueDateUtc <= cutoff)
            .OrderBy(vaccine => vaccine.DueDateUtc)
            .Select(vaccine => new
            {
                vaccine.Id,
                vaccine.VaccineName,
                vaccine.DueDateUtc,
                PetName = vaccine.Pet!.Name,
                OwnerName = vaccine.Pet.Owner!.FullName,
                OwnerPhone = vaccine.Pet.Owner.PhoneNumber
            })
            .ToListAsync();

        return Ok(vaccines);
    }

    [HttpPost("records")]
    public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(CreateMedicalRecordRequest request)
    {
        var vet = await context.Users.FirstOrDefaultAsync(item => item.Id == request.VetId && item.Role == UserRole.Vet);
        var pet = await context.Pets.FindAsync(request.PetId);

        if (vet is null || pet is null)
        {
            return BadRequest("Valid pet and vet are required.");
        }

        var record = new MedicalRecord
        {
            PetId = request.PetId,
            VetId = request.VetId,
            VisitReason = request.VisitReason,
            Diagnosis = request.Diagnosis,
            Treatment = request.Treatment,
            VisitDateUtc = request.VisitDateUtc
        };

        context.MedicalRecords.Add(record);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateMedicalRecord), new MedicalRecordDto(record.Id, vet.FullName, record.VisitReason, record.Diagnosis, record.Treatment, record.VisitDateUtc));
    }

    [HttpPut("records/{recordId:int}")]
    public async Task<ActionResult<MedicalRecordDto>> UpdateMedicalRecord(int recordId, UpdateMedicalRecordRequest request)
    {
        var record = await context.MedicalRecords
            .Include(item => item.Vet)
            .FirstOrDefaultAsync(item => item.Id == recordId);

        if (record is null || record.Vet is null)
        {
            return NotFound();
        }

        record.VisitReason = request.VisitReason;
        record.Diagnosis = request.Diagnosis;
        record.Treatment = request.Treatment;
        record.VisitDateUtc = request.VisitDateUtc;

        await context.SaveChangesAsync();

        return Ok(new MedicalRecordDto(record.Id, record.Vet.FullName, record.VisitReason, record.Diagnosis, record.Treatment, record.VisitDateUtc));
    }
}
