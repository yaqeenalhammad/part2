using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Models;
using PetCareJordan.Api.Services;

namespace PetCareJordan.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(PetCareJordanContext context)
    {
        await EnsureCommunityReportReporterColumnsAsync(context);
        await EnsureChatTablesAsync(context);
        var passwordService = new PasswordService();

        if (context.Users.Any())
        {
            await NormalizeExistingUserEmailsAsync(context);
            await NormalizeExistingEnglishTextAsync(context);
            await EnsureRequiredDemoAccountsAsync(context, passwordService);
            await RemoveDemoChatArtifactsAsync(context);
            return;
        }

        var users = new List<AppUser>
        {
            new() { FullName = "Safaa Alquraan", Email = "safaa.alquraan@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001001", City = "Amman", Role = UserRole.Admin },
            new() { FullName = "Yaqeen Alhammad", Email = "yaqeen.alhammad@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001002", City = "Amman", Role = UserRole.User },
            new() { FullName = "Lina Khalil", Email = "lina@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001003", City = "Irbid", Role = UserRole.User },
            new() { FullName = "Sara Odeh", Email = "sara@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001004", City = "Zarqa", Role = UserRole.User },
            new() { FullName = "Dr. Malak Alquraan", Email = "malak.alquraan@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001005", City = "Amman", Role = UserRole.Vet },
            new() { FullName = "Dr. Omar Qudah", Email = "omar.vet@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001006", City = "Irbid", Role = UserRole.Vet },
            new() { FullName = "Dina Majali", Email = "dina@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001007", City = "Salt", Role = UserRole.User },
            new() { FullName = "Ahmad Shannaq", Email = "ahmad@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001008", City = "Aqaba", Role = UserRole.User },
            new() { FullName = "Rama Azar", Email = "rama@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001009", City = "Madaba", Role = UserRole.User },
            new() { FullName = "Tareq Fares", Email = "tareq@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001010", City = "Jerash", Role = UserRole.User }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var userByEmail = users.ToDictionary(user => user.Email, StringComparer.OrdinalIgnoreCase);

        var pets = new List<Pet>
        {
            new() { Name = "Milo", Type = PetType.Cat, Breed = "Persian", AgeInMonths = 18, Gender = PetGender.Male, CollarId = "PCJ-1001", Color = "White", City = "Amman", WeightKg = 4.2m, IsNeutered = true, Description = "Calm indoor cat who loves quiet homes.", PhotoUrl = "https://images.unsplash.com/photo-1511044568932-338cba0ad803", OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Bella", Type = PetType.Dog, Breed = "Golden Retriever", AgeInMonths = 30, Gender = PetGender.Female, CollarId = "PCJ-1002", Color = "Golden", City = "Amman", WeightKg = 22.4m, IsNeutered = false, Description = "Friendly and perfect with children.", PhotoUrl = "https://images.unsplash.com/photo-1517849845537-4d257902454a", OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Kiwi", Type = PetType.Bird, Breed = "Cockatiel", AgeInMonths = 10, Gender = PetGender.Female, CollarId = "PCJ-1003", Color = "Grey and yellow", City = "Irbid", WeightKg = 0.1m, IsNeutered = false, Description = "Social bird that whistles a lot.", PhotoUrl = "https://images.unsplash.com/photo-1444464666168-49d633b86797", OwnerId = userByEmail["sara@petcare.com"].Id },
            new() { Name = "Snow", Type = PetType.Rabbit, Breed = "Holland Lop", AgeInMonths = 12, Gender = PetGender.Female, CollarId = "PCJ-1004", Color = "Cream", City = "Zarqa", WeightKg = 1.8m, IsNeutered = true, Description = "Gentle rabbit used to apartment life.", PhotoUrl = "https://images.unsplash.com/photo-1585110396000-c9ffd4e4b308", OwnerId = userByEmail["dina@petcare.com"].Id },
            new() { Name = "Simba", Type = PetType.Cat, Breed = "Tabby", AgeInMonths = 24, Gender = PetGender.Male, CollarId = "PCJ-1005", Color = "Brown", City = "Amman", WeightKg = 5.1m, IsNeutered = true, Description = "Playful cat with lots of energy.", PhotoUrl = "https://images.unsplash.com/photo-1574158622682-e40e69881006", OwnerId = userByEmail["ahmad@petcare.com"].Id },
            new() { Name = "Rocky", Type = PetType.Dog, Breed = "German Shepherd", AgeInMonths = 36, Gender = PetGender.Male, CollarId = "PCJ-1006", Color = "Black and tan", City = "Aqaba", WeightKg = 29.7m, IsNeutered = false, Description = "Loyal dog that needs an active owner.", PhotoUrl = "https://images.unsplash.com/photo-1518717758536-85ae29035b6d", OwnerId = userByEmail["rama@petcare.com"].Id },
            new() { Name = "Lulu", Type = PetType.Cat, Breed = "Siamese", AgeInMonths = 16, Gender = PetGender.Female, CollarId = "PCJ-1007", Color = "Cream and brown", City = "Madaba", WeightKg = 3.9m, IsNeutered = false, Description = "Talkative and affectionate.", PhotoUrl = "https://images.unsplash.com/photo-1494256997604-768d1f608cac", OwnerId = userByEmail["tareq@petcare.com"].Id },
            new() { Name = "Coco", Type = PetType.Bird, Breed = "Lovebird", AgeInMonths = 14, Gender = PetGender.Male, CollarId = "PCJ-1008", Color = "Green", City = "Salt", WeightKg = 0.09m, IsNeutered = false, Description = "Bright and cheerful companion.", PhotoUrl = "https://images.unsplash.com/photo-1522926193341-e9ffd686c60f", OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Nala", Type = PetType.Dog, Breed = "Husky", AgeInMonths = 28, Gender = PetGender.Female, CollarId = "PCJ-1009", Color = "Grey and white", City = "Jerash", WeightKg = 20.2m, IsNeutered = true, Description = "Energetic dog that enjoys long walks.", PhotoUrl = "https://images.unsplash.com/photo-1548199973-03cce0bbc87b", OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Hazel", Type = PetType.Rabbit, Breed = "Mini Rex", AgeInMonths = 9, Gender = PetGender.Female, CollarId = "PCJ-1010", Color = "Brown", City = "Amman", WeightKg = 1.4m, IsNeutered = false, Description = "Curious rabbit with a calm personality.", PhotoUrl = "https://images.unsplash.com/photo-1585110396000-c9ffd4e4b308", OwnerId = userByEmail["sara@petcare.com"].Id },
            new() { Name = "Zazu", Type = PetType.Bird, Breed = "African Grey", AgeInMonths = 40, Gender = PetGender.Male, CollarId = "PCJ-1011", Color = "Grey", City = "Amman", WeightKg = 0.4m, IsNeutered = false, Description = "Smart parrot with a growing vocabulary.", PhotoUrl = "https://images.unsplash.com/photo-1552728089-57bdde30beb3", OwnerId = userByEmail["dina@petcare.com"].Id },
            new() { Name = "Poppy", Type = PetType.Cat, Breed = "Scottish Fold", AgeInMonths = 20, Gender = PetGender.Female, CollarId = "PCJ-1012", Color = "Silver", City = "Irbid", WeightKg = 4.0m, IsNeutered = true, Description = "Quiet cat that likes window naps.", PhotoUrl = "https://images.unsplash.com/photo-1519052537078-e6302a4968d4", OwnerId = userByEmail["ahmad@petcare.com"].Id },
            new() { Name = "Max", Type = PetType.Dog, Breed = "Mixed Breed", AgeInMonths = 14, Gender = PetGender.Male, CollarId = "PCJ-1013", Color = "Brown and white", City = "Zarqa", WeightKg = 11.5m, IsNeutered = true, Description = "Rescued dog ready for a second chance.", PhotoUrl = "https://images.unsplash.com/photo-1507146426996-ef05306b995a", OwnerId = userByEmail["rama@petcare.com"].Id },
            new() { Name = "Ruby", Type = PetType.Other, Breed = "Hamster", AgeInMonths = 6, Gender = PetGender.Female, CollarId = "PCJ-1014", Color = "Golden", City = "Salt", WeightKg = 0.05m, IsNeutered = false, Description = "Small and easy to care for.", PhotoUrl = "https://images.unsplash.com/photo-1425082661705-1834bfd09dca", OwnerId = userByEmail["tareq@petcare.com"].Id },
            new() { Name = "Leo", Type = PetType.Cat, Breed = "Orange Tabby", AgeInMonths = 15, Gender = PetGender.Male, CollarId = "PCJ-1015", Color = "Orange", City = "Amman", WeightKg = 4.6m, IsNeutered = true, Description = "Curious and social cat.", PhotoUrl = "https://images.unsplash.com/photo-1543852786-1cf6624b9987", OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Lemon", Type = PetType.Bird, Breed = "Canary", AgeInMonths = 8, Gender = PetGender.Female, CollarId = "PCJ-1016", Color = "Yellow", City = "Madaba", WeightKg = 0.03m, IsNeutered = false, Description = "Beautiful singer for a calm home.", PhotoUrl = "https://images.unsplash.com/photo-1520808663317-647b476a81b9", OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Bruno", Type = PetType.Dog, Breed = "Boxer", AgeInMonths = 26, Gender = PetGender.Male, CollarId = "PCJ-1017", Color = "Brown", City = "Irbid", WeightKg = 24.2m, IsNeutered = false, Description = "Protective, smart, and playful.", PhotoUrl = "https://images.unsplash.com/photo-1517423440428-a5a00ad493e8", OwnerId = userByEmail["sara@petcare.com"].Id },
            new() { Name = "Mochi", Type = PetType.Rabbit, Breed = "Lionhead", AgeInMonths = 13, Gender = PetGender.Male, CollarId = "PCJ-1018", Color = "White and brown", City = "Jerash", WeightKg = 1.6m, IsNeutered = true, Description = "Fluffy rabbit that enjoys gentle handling.", PhotoUrl = "https://images.unsplash.com/photo-1585110396000-c9ffd4e4b308", OwnerId = userByEmail["dina@petcare.com"].Id },
            new() { Name = "Sandy", Type = PetType.Cat, Breed = "Domestic Shorthair", AgeInMonths = 22, Gender = PetGender.Female, CollarId = "PCJ-1019", Color = "Sand", City = "Aqaba", WeightKg = 4.3m, IsNeutered = true, Description = "Relaxed cat suited for first-time owners.", PhotoUrl = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131", OwnerId = userByEmail["ahmad@petcare.com"].Id },
            new() { Name = "Thor", Type = PetType.Dog, Breed = "Labrador", AgeInMonths = 32, Gender = PetGender.Male, CollarId = "PCJ-1020", Color = "Black", City = "Amman", WeightKg = 27.4m, IsNeutered = true, Description = "Very trainable dog with a calm temperament.", PhotoUrl = "https://images.unsplash.com/photo-1561037404-61cd46aa615b", OwnerId = userByEmail["rama@petcare.com"].Id },
            new() { Name = "Pearl", Type = PetType.Cat, Breed = "Persian", AgeInMonths = 27, Gender = PetGender.Female, CollarId = "PCJ-1021", Color = "White", City = "Salt", WeightKg = 4.7m, IsNeutered = false, Description = "Elegant and low-energy companion.", PhotoUrl = "https://images.unsplash.com/photo-1533738363-b7f9aef128ce", OwnerId = userByEmail["tareq@petcare.com"].Id },
            new() { Name = "Pico", Type = PetType.Bird, Breed = "Budgie", AgeInMonths = 11, Gender = PetGender.Male, CollarId = "PCJ-1022", Color = "Blue", City = "Zarqa", WeightKg = 0.04m, IsNeutered = false, Description = "A small bird that enjoys interaction.", PhotoUrl = "https://images.unsplash.com/photo-1452570053594-1b985d6ea890", OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Daisy", Type = PetType.Rabbit, Breed = "Dutch Rabbit", AgeInMonths = 10, Gender = PetGender.Female, CollarId = "PCJ-1023", Color = "Black and white", City = "Amman", WeightKg = 1.7m, IsNeutered = false, Description = "Compact rabbit suited for indoor life.", PhotoUrl = "https://images.unsplash.com/photo-1585110396000-c9ffd4e4b308", OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Scout", Type = PetType.Other, Breed = "Turtle", AgeInMonths = 48, Gender = PetGender.Male, CollarId = "PCJ-1024", Color = "Green", City = "Madaba", WeightKg = 2.3m, IsNeutered = false, Description = "Healthy turtle with a full habitat setup.", PhotoUrl = "https://images.unsplash.com/photo-1437623889155-075d40e2e59f", OwnerId = userByEmail["sara@petcare.com"].Id }
        };

        await context.Pets.AddRangeAsync(pets);
        await context.SaveChangesAsync();

        var petByCollarId = pets.ToDictionary(pet => pet.CollarId, StringComparer.OrdinalIgnoreCase);

        var adoptionListings = new List<AdoptionListing>
        {
            new() { PetId = petByCollarId["PCJ-1001"].Id, Story = "Owner is relocating and wants a safe home.", ContactMethod = "Phone", ContactDetails = "0799001002", Status = AdoptionStatus.Available, PostedAtUtc = DateTime.UtcNow.AddDays(-10) },
            new() { PetId = petByCollarId["PCJ-1004"].Id, Story = "Looking for a family experienced with rabbits.", ContactMethod = "Phone", ContactDetails = "0799001007", Status = AdoptionStatus.Available, PostedAtUtc = DateTime.UtcNow.AddDays(-7) },
            new() { PetId = petByCollarId["PCJ-1006"].Id, Story = "Needs an active adopter with a yard.", ContactMethod = "Phone", ContactDetails = "0799001009", Status = AdoptionStatus.Pending, PostedAtUtc = DateTime.UtcNow.AddDays(-4) },
            new() { PetId = petByCollarId["PCJ-1010"].Id, Story = "Perfect for a calm apartment home.", ContactMethod = "Email", ContactDetails = "sara@petcare.com", Status = AdoptionStatus.Available, PostedAtUtc = DateTime.UtcNow.AddDays(-3) },
            new() { PetId = petByCollarId["PCJ-1013"].Id, Story = "Rescue dog that deserves a loving family.", ContactMethod = "Phone", ContactDetails = "0799001009", Status = AdoptionStatus.Available, PostedAtUtc = DateTime.UtcNow.AddDays(-5) },
            new() { PetId = petByCollarId["PCJ-1018"].Id, Story = "Friendly rabbit available because owner is moving.", ContactMethod = "Phone", ContactDetails = "0799001007", Status = AdoptionStatus.Available, PostedAtUtc = DateTime.UtcNow.AddDays(-6) },
            new() { PetId = petByCollarId["PCJ-1021"].Id, Story = "Quiet Persian cat available for adoption.", ContactMethod = "Phone", ContactDetails = "0799001010", Status = AdoptionStatus.Pending, PostedAtUtc = DateTime.UtcNow.AddDays(-2) },
            new() { PetId = petByCollarId["PCJ-1023"].Id, Story = "Young rabbit looking for a first home.", ContactMethod = "Phone", ContactDetails = "0799001003", Status = AdoptionStatus.Available, PostedAtUtc = DateTime.UtcNow.AddDays(-1) }
        };

        var lostReports = new List<LostPetReport>
        {
            new() { PetName = "Shadow", PetType = PetType.Cat, Description = "Black cat with green collar.", ApproximateAgeInMonths = 20, LastSeenPlace = "Jabal Amman near Rainbow Street", LastSeenDateUtc = DateTime.UtcNow.AddDays(-2), RewardAmount = 25, PhotoUrl = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131", ContactName = "Lina Khalil", ContactPhone = "0799001002", Status = ReportStatus.Active },
            new() { PetName = "Biscuit", PetType = PetType.Dog, Description = "Small brown dog, very friendly.", ApproximateAgeInMonths = 14, LastSeenPlace = "Irbid University Street", LastSeenDateUtc = DateTime.UtcNow.AddDays(-1), RewardAmount = null, PhotoUrl = "https://images.unsplash.com/photo-1507146426996-ef05306b995a", ContactName = "Ahmad Shannaq", ContactPhone = "0799001008", Status = ReportStatus.Active },
            new() { PetName = "Sunny", PetType = PetType.Bird, Description = "Yellow canary escaped from balcony.", ApproximateAgeInMonths = 9, LastSeenPlace = "Madaba downtown", LastSeenDateUtc = DateTime.UtcNow.AddDays(-3), RewardAmount = 15, PhotoUrl = "https://images.unsplash.com/photo-1520808663317-647b476a81b9", ContactName = "Rama Azar", ContactPhone = "0799001009", Status = ReportStatus.Active }
        };

        var foundReports = new List<FoundPetReport>
        {
            new() { PetType = PetType.Cat, Description = "Grey cat found with no visible injury.", FoundPlace = "Abdoun, Amman", FoundDateUtc = DateTime.UtcNow.AddDays(-1), PhotoUrl = "https://images.unsplash.com/photo-1519052537078-e6302a4968d4", ContactName = "Dina Majali", ContactPhone = "0799001007", Status = ReportStatus.Active },
            new() { PetType = PetType.Dog, Description = "White mixed-breed dog found near market.", FoundPlace = "Salt city center", FoundDateUtc = DateTime.UtcNow.AddDays(-2), PhotoUrl = "https://images.unsplash.com/photo-1561037404-61cd46aa615b", ContactName = "Tareq Fares", ContactPhone = "0799001010", Status = ReportStatus.Active }
        };

        await context.AdoptionListings.AddRangeAsync(adoptionListings);
        await context.LostPetReports.AddRangeAsync(lostReports);
        await context.FoundPetReports.AddRangeAsync(foundReports);
        await context.SaveChangesAsync();

        var medicalRecords = new List<MedicalRecord>
        {
            new() { PetId = petByCollarId["PCJ-1001"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VisitReason = "General check-up", Diagnosis = "Healthy", Treatment = "No treatment needed", VisitDateUtc = DateTime.UtcNow.AddMonths(-2) },
            new() { PetId = petByCollarId["PCJ-1002"].Id, VetId = userByEmail["omar.vet@petcare.com"].Id, VisitReason = "Skin irritation", Diagnosis = "Mild allergy", Treatment = "Antihistamine for 5 days", VisitDateUtc = DateTime.UtcNow.AddMonths(-1) },
            new() { PetId = petByCollarId["PCJ-1006"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VisitReason = "Vaccination follow-up", Diagnosis = "Healthy", Treatment = "Routine monitoring", VisitDateUtc = DateTime.UtcNow.AddMonths(-3) },
            new() { PetId = petByCollarId["PCJ-1011"].Id, VetId = userByEmail["omar.vet@petcare.com"].Id, VisitReason = "Feather check", Diagnosis = "Vitamin deficiency", Treatment = "Diet adjustment and supplements", VisitDateUtc = DateTime.UtcNow.AddDays(-40) },
            new() { PetId = petByCollarId["PCJ-1020"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VisitReason = "Dental cleaning", Diagnosis = "Healthy gums", Treatment = "Annual follow-up", VisitDateUtc = DateTime.UtcNow.AddDays(-18) }
        };

        var vaccinations = new List<VaccinationRecord>
        {
            new() { PetId = petByCollarId["PCJ-1001"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VaccineName = "Rabies", GivenOnUtc = DateTime.UtcNow.AddMonths(-11), DueDateUtc = DateTime.UtcNow.AddDays(15), IsCompleted = false },
            new() { PetId = petByCollarId["PCJ-1002"].Id, VetId = userByEmail["omar.vet@petcare.com"].Id, VaccineName = "DHPP", GivenOnUtc = DateTime.UtcNow.AddMonths(-10), DueDateUtc = DateTime.UtcNow.AddDays(7), IsCompleted = false },
            new() { PetId = petByCollarId["PCJ-1006"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VaccineName = "Rabies", GivenOnUtc = DateTime.UtcNow.AddMonths(-4), DueDateUtc = DateTime.UtcNow.AddMonths(8), IsCompleted = true },
            new() { PetId = petByCollarId["PCJ-1009"].Id, VetId = userByEmail["omar.vet@petcare.com"].Id, VaccineName = "Bordetella", GivenOnUtc = DateTime.UtcNow.AddMonths(-8), DueDateUtc = DateTime.UtcNow.AddDays(9), IsCompleted = false },
            new() { PetId = petByCollarId["PCJ-1012"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VaccineName = "FVRCP", GivenOnUtc = DateTime.UtcNow.AddMonths(-11), DueDateUtc = DateTime.UtcNow.AddDays(20), IsCompleted = false },
            new() { PetId = petByCollarId["PCJ-1015"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VaccineName = "Rabies", GivenOnUtc = DateTime.UtcNow.AddMonths(-9), DueDateUtc = DateTime.UtcNow.AddDays(5), IsCompleted = false },
            new() { PetId = petByCollarId["PCJ-1020"].Id, VetId = userByEmail["malak.alquraan@petcare.com"].Id, VaccineName = "DHPP", GivenOnUtc = DateTime.UtcNow.AddMonths(-12), DueDateUtc = DateTime.UtcNow.AddDays(3), IsCompleted = false },
            new() { PetId = petByCollarId["PCJ-1023"].Id, VetId = userByEmail["omar.vet@petcare.com"].Id, VaccineName = "Rabbit Hemorrhagic Disease", GivenOnUtc = DateTime.UtcNow.AddMonths(-6), DueDateUtc = DateTime.UtcNow.AddDays(12), IsCompleted = false }
        };

        var notifications = new List<Notification>
        {
            new() { UserId = userByEmail["lina@petcare.com"].Id, Type = NotificationType.VaccineReminder, Title = "Vaccine reminder for Milo", Message = "Rabies vaccine is due in 15 days.", TriggerDateUtc = DateTime.UtcNow, IsRead = false },
            new() { UserId = userByEmail["yaqeen.alhammad@petcare.com"].Id, Type = NotificationType.VaccineReminder, Title = "Vaccine reminder for Bella", Message = "DHPP vaccine is due in 7 days.", TriggerDateUtc = DateTime.UtcNow, IsRead = false },
            new() { UserId = userByEmail["rama@petcare.com"].Id, Type = NotificationType.VaccineReminder, Title = "Vaccine reminder for Thor", Message = "DHPP vaccine is due in 3 days.", TriggerDateUtc = DateTime.UtcNow, IsRead = false }
        };

        await context.MedicalRecords.AddRangeAsync(medicalRecords);
        await context.VaccinationRecords.AddRangeAsync(vaccinations);
        await context.Notifications.AddRangeAsync(notifications);
        await context.SaveChangesAsync();
        await NormalizeExistingEnglishTextAsync(context);
        await RemoveDemoChatArtifactsAsync(context);
    }

    private static async Task EnsureChatTablesAsync(PetCareJordanContext context)
    {
        const string removeMalformedChatTables = """
            IF OBJECT_ID(N'[ChatMessages]', N'U') IS NOT NULL
               AND (
                    COL_LENGTH(N'[ChatMessages]', N'ConversationId') IS NULL
                    OR COL_LENGTH(N'[ChatMessages]', N'SenderId') IS NULL
                    OR COL_LENGTH(N'[ChatMessages]', N'Message') IS NULL
                    OR COL_LENGTH(N'[ChatMessages]', N'SentAtUtc') IS NULL
               )
            BEGIN
                DROP TABLE [ChatMessages];
            END

            IF OBJECT_ID(N'[ChatConversations]', N'U') IS NOT NULL
               AND (
                    COL_LENGTH(N'[ChatConversations]', N'UserId') IS NULL
                    OR COL_LENGTH(N'[ChatConversations]', N'VetId') IS NULL
                    OR COL_LENGTH(N'[ChatConversations]', N'CreatedAtUtc') IS NULL
               )
            BEGIN
                IF OBJECT_ID(N'[ChatMessages]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [ChatMessages];
                END

                DROP TABLE [ChatConversations];
            END
            """;

        const string createConversations = """
            IF OBJECT_ID(N'[ChatConversations]', N'U') IS NULL
            BEGIN
                CREATE TABLE [ChatConversations] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [UserId] INT NOT NULL,
                    [VetId] INT NOT NULL,
                    [CreatedAtUtc] DATETIME2 NOT NULL,
                    [UpdatedAtUtc] DATETIME2 NULL,
                    CONSTRAINT [FK_ChatConversations_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_ChatConversations_Users_VetId] FOREIGN KEY ([VetId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
                );
                CREATE UNIQUE INDEX [IX_ChatConversations_UserId_VetId] ON [ChatConversations] ([UserId], [VetId]);
                CREATE INDEX [IX_ChatConversations_UserId] ON [ChatConversations] ([UserId]);
                CREATE INDEX [IX_ChatConversations_VetId] ON [ChatConversations] ([VetId]);
            END
            """;

        const string createMessages = """
            IF OBJECT_ID(N'[ChatMessages]', N'U') IS NULL
            BEGIN
                CREATE TABLE [ChatMessages] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [ConversationId] INT NOT NULL,
                    [SenderId] INT NOT NULL,
                    [Message] NVARCHAR(MAX) NOT NULL,
                    [SentAtUtc] DATETIME2 NOT NULL,
                    [IsReadByRecipient] BIT NOT NULL CONSTRAINT [DF_ChatMessages_IsReadByRecipient] DEFAULT(0),
                    [ReadAtUtc] DATETIME2 NULL,
                    CONSTRAINT [FK_ChatMessages_ChatConversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [ChatConversations] ([Id]) ON DELETE CASCADE,
                    CONSTRAINT [FK_ChatMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
                );
                CREATE INDEX [IX_ChatMessages_ConversationId] ON [ChatMessages] ([ConversationId]);
                CREATE INDEX [IX_ChatMessages_SenderId] ON [ChatMessages] ([SenderId]);
            END
            """;

        const string ensureMessageReadByRecipientColumn = """
            IF COL_LENGTH(N'[ChatMessages]', N'IsReadByRecipient') IS NULL
            BEGIN
                ALTER TABLE [ChatMessages]
                ADD [IsReadByRecipient] BIT NOT NULL CONSTRAINT [DF_ChatMessages_IsReadByRecipient] DEFAULT(1);
            END
            """;

        const string ensureMessageReadValues = """
            UPDATE [ChatMessages]
            SET [IsReadByRecipient] = 1
            WHERE [IsReadByRecipient] = 0;
            """;

        const string ensureMessageReadAtColumn = """
            IF COL_LENGTH(N'[ChatMessages]', N'ReadAtUtc') IS NULL
            BEGIN
                ALTER TABLE [ChatMessages]
                ADD [ReadAtUtc] DATETIME2 NULL;
            END
            """;

        await context.Database.ExecuteSqlRawAsync(removeMalformedChatTables);
        await context.Database.ExecuteSqlRawAsync(createConversations);
        await context.Database.ExecuteSqlRawAsync(createMessages);
        await context.Database.ExecuteSqlRawAsync(ensureMessageReadByRecipientColumn);
        await context.Database.ExecuteSqlRawAsync(ensureMessageReadValues);
        await context.Database.ExecuteSqlRawAsync(ensureMessageReadAtColumn);
    }

    private static async Task EnsureCommunityReportReporterColumnsAsync(PetCareJordanContext context)
    {
        const string sql = """
            IF COL_LENGTH(N'[LostPetReports]', N'ReporterId') IS NULL
            BEGIN
                ALTER TABLE [LostPetReports] ADD [ReporterId] INT NULL;
                CREATE INDEX [IX_LostPetReports_ReporterId] ON [LostPetReports] ([ReporterId]);
                ALTER TABLE [LostPetReports]
                ADD CONSTRAINT [FK_LostPetReports_Users_ReporterId]
                FOREIGN KEY ([ReporterId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL;
            END

            IF COL_LENGTH(N'[FoundPetReports]', N'ReporterId') IS NULL
            BEGIN
                ALTER TABLE [FoundPetReports] ADD [ReporterId] INT NULL;
                CREATE INDEX [IX_FoundPetReports_ReporterId] ON [FoundPetReports] ([ReporterId]);
                ALTER TABLE [FoundPetReports]
                ADD CONSTRAINT [FK_FoundPetReports_Users_ReporterId]
                FOREIGN KEY ([ReporterId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL;
            END
            """;

        await context.Database.ExecuteSqlRawAsync(sql);
    }

    private static async Task RemoveDemoChatArtifactsAsync(PetCareJordanContext context)
    {
        var messagesToDelete = await context.ChatMessages
            .Where(item =>
                item.Message == "Hello, I can reply as vet." ||
                item.Message == "Hello doctor from user test" ||
                item.Message == "Vet notification check 131843" ||
                item.Message == "User notification check 131843" ||
                item.Message.StartsWith("Hello from user ") ||
                item.Message.StartsWith("Hello user, this is the vet ") ||
                item.Message.StartsWith("Validation ping ") ||
                item.Message == "Hi doctor, my dog is not eating well today. What should I do?" ||
                item.Message == "Please check temperature and water intake first. If appetite stays low by tomorrow, book a check-up.")
            .ToListAsync();

        if (messagesToDelete.Count == 0)
        {
            return;
        }

        context.ChatMessages.RemoveRange(messagesToDelete);
        await context.SaveChangesAsync();
    }

    private static async Task NormalizeExistingEnglishTextAsync(PetCareJordanContext context)
    {
        var hasChanges = false;

        var users = await context.Users.ToListAsync();
        foreach (var user in users)
        {
            hasChanges |= NormalizeTextProperty(value => user.FullName = value, user.FullName);
            hasChanges |= NormalizeTextProperty(value => user.City = value, user.City);
        }

        var pets = await context.Pets.ToListAsync();
        foreach (var pet in pets)
        {
            hasChanges |= NormalizeTextProperty(value => pet.Name = value, pet.Name);
            hasChanges |= NormalizeTextProperty(value => pet.Breed = value, pet.Breed);
            hasChanges |= NormalizeTextProperty(value => pet.Color = value, pet.Color);
            hasChanges |= NormalizeTextProperty(value => pet.City = value, pet.City);
            hasChanges |= NormalizeTextProperty(value => pet.Description = value, pet.Description);
        }

        var adoptions = await context.AdoptionListings.ToListAsync();
        foreach (var adoption in adoptions)
        {
            hasChanges |= NormalizeTextProperty(value => adoption.Story = value, adoption.Story);
            hasChanges |= NormalizeTextProperty(value => adoption.ContactMethod = value, adoption.ContactMethod);
        }

        var lostReports = await context.LostPetReports.ToListAsync();
        foreach (var report in lostReports)
        {
            hasChanges |= NormalizeTextProperty(value => report.PetName = value, report.PetName);
            hasChanges |= NormalizeTextProperty(value => report.Description = value, report.Description);
            hasChanges |= NormalizeTextProperty(value => report.LastSeenPlace = value, report.LastSeenPlace);
            hasChanges |= NormalizeTextProperty(value => report.ContactName = value, report.ContactName);
        }

        var foundReports = await context.FoundPetReports.ToListAsync();
        foreach (var report in foundReports)
        {
            hasChanges |= NormalizeTextProperty(value => report.Description = value, report.Description);
            hasChanges |= NormalizeTextProperty(value => report.FoundPlace = value, report.FoundPlace);
            hasChanges |= NormalizeTextProperty(value => report.ContactName = value, report.ContactName);
        }

        var medicalRecords = await context.MedicalRecords.ToListAsync();
        foreach (var record in medicalRecords)
        {
            hasChanges |= NormalizeTextProperty(value => record.VisitReason = value, record.VisitReason);
            hasChanges |= NormalizeTextProperty(value => record.Diagnosis = value, record.Diagnosis);
            hasChanges |= NormalizeTextProperty(value => record.Treatment = value, record.Treatment);
        }

        var vaccinations = await context.VaccinationRecords.ToListAsync();
        foreach (var vaccination in vaccinations)
        {
            hasChanges |= NormalizeTextProperty(value => vaccination.VaccineName = value, vaccination.VaccineName);
        }

        var notifications = await context.Notifications.ToListAsync();
        foreach (var notification in notifications)
        {
            hasChanges |= NormalizeTextProperty(value => notification.Title = value, notification.Title);
            hasChanges |= NormalizeTextProperty(value => notification.Message = value, notification.Message);
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }
    }

    private static bool NormalizeTextProperty(Action<string> setValue, string currentValue)
    {
        var normalized = ToEnglishOnly(currentValue);
        if (normalized == currentValue)
        {
            return false;
        }

        setValue(normalized);
        return true;
    }

    private static string ToEnglishOnly(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var pipeIndex = value.LastIndexOf('|');
        if (pipeIndex >= 0 && pipeIndex < value.Length - 1)
        {
            return value[(pipeIndex + 1)..].Trim();
        }

        return value.Trim() switch
        {
            "عمان" => "Amman",
            "العقبة" => "Aqaba",
            "إربد" => "Irbid",
            "اربد" => "Irbid",
            "الزرقاء" => "Zarqa",
            "المفرق" => "Mafraq",
            "مأدبا" => "Madaba",
            "مادبا" => "Madaba",
            "السلط" => "Salt",
            "جرش" => "Jerash",
            "الكرك" => "Karak",
            "تذكير تطعيم" => "Vaccine Reminder",
            _ => value
        };
    }

    private static async Task NormalizeExistingUserEmailsAsync(PetCareJordanContext context)
    {
        var users = await context.Users.ToListAsync();
        var hasChanges = false;

        foreach (var user in users)
        {
            var email = (user.Email ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                continue;
            }

            var atIndex = email.IndexOf('@');
            var localPart = atIndex > 0 ? email[..atIndex] : email;
            var normalized = $"{localPart}@petcare.com".ToLowerInvariant();

            if (string.Equals(email, normalized, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var duplicate = users.Any(item =>
                item.Id != user.Id &&
                string.Equals(item.Email, normalized, StringComparison.OrdinalIgnoreCase));

            if (!duplicate)
            {
                user.Email = normalized;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureRequiredDemoAccountsAsync(PetCareJordanContext context, PasswordService passwordService)
    {
        var required = new List<AppUser>
        {
            new() { FullName = "Safaa Alquraan", Email = "safaa.alquraan@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001001", City = "Amman", Role = UserRole.Admin },
            new() { FullName = "Yaqeen Alhammad", Email = "yaqeen.alhammad@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001002", City = "Amman", Role = UserRole.User },
            new() { FullName = "Dr. Malak Alquraan", Email = "malak.alquraan@petcare.com", PasswordHash = passwordService.HashPassword("Pass123!"), PhoneNumber = "0799001005", City = "Amman", Role = UserRole.Vet }
        };

        var hasChanges = false;

        foreach (var requiredUser in required)
        {
            var existing = await context.Users.FirstOrDefaultAsync(item => item.Email == requiredUser.Email);
            if (existing is null)
            {
                context.Users.Add(requiredUser);
                hasChanges = true;
                continue;
            }

            if (existing.FullName != requiredUser.FullName ||
                existing.Role != requiredUser.Role ||
                existing.PhoneNumber != requiredUser.PhoneNumber ||
                existing.City != requiredUser.City)
            {
                existing.FullName = requiredUser.FullName;
                existing.Role = requiredUser.Role;
                existing.PhoneNumber = requiredUser.PhoneNumber;
                existing.City = requiredUser.City;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }
    }
}
