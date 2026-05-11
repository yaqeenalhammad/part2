using Microsoft.EntityFrameworkCore;
using PetCareJordan.Api.Models;
using PetCareJordan.Api.Services;

namespace PetCareJordan.Api.Data;

public static class SeedData
{
    private static readonly IReadOnlyDictionary<string, string> SeedPetPhotoUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["PCJ-1001"] = "https://loremflickr.com/900/650/persian,cat?lock=1001",
        ["PCJ-1002"] = "https://loremflickr.com/900/650/goldenretriever,dog?lock=1002",
        ["PCJ-1003"] = "https://loremflickr.com/900/650/cockatiel,bird?lock=1003",
        ["PCJ-1004"] = "https://loremflickr.com/900/650/lop,rabbit?lock=1004",
        ["PCJ-1005"] = "https://loremflickr.com/900/650/tabby,cat?lock=1005",
        ["PCJ-1006"] = "https://loremflickr.com/900/650/germanshepherd,dog?lock=1006",
        ["PCJ-1007"] = "https://loremflickr.com/900/650/siamese,cat?lock=1007",
        ["PCJ-1008"] = "https://loremflickr.com/900/650/lovebird,bird?lock=1008",
        ["PCJ-1009"] = "https://loremflickr.com/900/650/husky,dog?lock=1009",
        ["PCJ-1010"] = "https://loremflickr.com/900/650/brown,rabbit?lock=1010",
        ["PCJ-1011"] = "https://loremflickr.com/900/650/parrot,bird?lock=1011",
        ["PCJ-1012"] = "https://loremflickr.com/900/650/scottishfold,cat?lock=1012",
        ["PCJ-1013"] = "https://loremflickr.com/900/650/rescue,dog?lock=1013",
        ["PCJ-1014"] = "https://loremflickr.com/900/650/hamster?lock=1014",
        ["PCJ-1015"] = "https://loremflickr.com/900/650/orange,cat?lock=1015",
        ["PCJ-1016"] = "https://loremflickr.com/900/650/canary,bird?lock=1016",
        ["PCJ-1017"] = "https://loremflickr.com/900/650/boxer,dog?lock=1017",
        ["PCJ-1018"] = "https://loremflickr.com/900/650/lionhead,rabbit?lock=1018",
        ["PCJ-1019"] = "https://loremflickr.com/900/650/shorthair,cat?lock=1019",
        ["PCJ-1020"] = "https://loremflickr.com/900/650/labrador,dog?lock=1020",
        ["PCJ-1021"] = "https://loremflickr.com/900/650/white,persian,cat?lock=1021",
        ["PCJ-1022"] = "https://loremflickr.com/900/650/budgie,bird?lock=1022",
        ["PCJ-1023"] = "https://loremflickr.com/900/650/dutch,rabbit?lock=1023",
        ["PCJ-1024"] = "https://loremflickr.com/900/650/turtle?lock=1024",
        ["PCJ-1025"] = "https://loremflickr.com/900/650/hamster?lock=1025",
        ["PCJ-1026"] = "https://loremflickr.com/900/650/turtle?lock=1026",
        ["PCJ-1027"] = "https://loremflickr.com/900/650/white,rabbit?lock=1027",
        ["PCJ-1028"] = "https://loremflickr.com/900/650/parakeet,bird?lock=1028",
        ["PCJ-1029"] = "https://loremflickr.com/900/650/poodle,dog?lock=1029",
        ["PCJ-1030"] = "https://loremflickr.com/900/650/calico,cat?lock=1030",
        ["PCJ-1031"] = "https://loremflickr.com/900/650/turtle?lock=1031",
        ["PCJ-1032"] = "https://loremflickr.com/900/650/dalmatian,dog?lock=1032",
        ["PCJ-1033"] = "https://loremflickr.com/900/650/parrot,bird?lock=1033",
        ["PCJ-1034"] = "https://loremflickr.com/900/650/black,rabbit?lock=1034",
        ["PCJ-1035"] = "https://loremflickr.com/900/650/hamster?lock=1035",
        ["PCJ-1036"] = "https://loremflickr.com/900/650/labrador,puppy?lock=1036",
        ["PCJ-1037"] = "https://loremflickr.com/900/650/ginger,cat?lock=1037",
        ["PCJ-1038"] = "https://loremflickr.com/900/650/beagle,dog?lock=1038"
    };

    private static readonly IReadOnlyDictionary<string, string> SeedLostReportPhotoUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Shadow"] = "https://loremflickr.com/900/650/black,cat?lock=2001",
        ["Biscuit"] = "https://loremflickr.com/900/650/small,brown,dog?lock=2002",
        ["Sunny"] = "https://loremflickr.com/900/650/yellow,canary,bird?lock=2003"
    };

    private static readonly IReadOnlyDictionary<string, string> SeedFoundReportPhotoUrls = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Grey cat found with no visible injury."] = "https://loremflickr.com/900/650/grey,cat?lock=3001",
        ["White mixed-breed dog found near market."] = "https://loremflickr.com/900/650/white,dog?lock=3002"
    };

    private static readonly IReadOnlyDictionary<string, string> SeedPetLocationDetails = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["PCJ-1001"] = "Amman, Jabal Al-Weibdeh, College Street",
        ["PCJ-1002"] = "Amman, Abdoun, Cairo Street",
        ["PCJ-1003"] = "Irbid, Al-Husun, Main Street",
        ["PCJ-1004"] = "Zarqa, New Zarqa, 36th Street",
        ["PCJ-1005"] = "Mafraq, King Talal Street",
        ["PCJ-1006"] = "Aqaba, Al-Rabieh, Beach Road",
        ["PCJ-1007"] = "Madaba, City Center, Talal Street",
        ["PCJ-1008"] = "Salt, Al-Sarou, Salt Ring Road",
        ["PCJ-1009"] = "Jerash, Souf, Roman Road",
        ["PCJ-1010"] = "Amman, Khalda, Wasfi Al-Tal Street",
        ["PCJ-1011"] = "Amman, Sweifieh, Ali Nasouh Al-Taher Street",
        ["PCJ-1012"] = "Irbid, University District, University Street",
        ["PCJ-1013"] = "Mafraq, Al-Hussein District",
        ["PCJ-1014"] = "Salt, Downtown Salt, Hammam Street",
        ["PCJ-1015"] = "Amman, Jubeiha, Queen Rania Street",
        ["PCJ-1016"] = "Mafraq, Al-Badiya Road",
        ["PCJ-1017"] = "Karak, Al-Mazar, Castle Street",
        ["PCJ-1018"] = "Jerash, Al-Mastaba Road",
        ["PCJ-1019"] = "Aqaba, Third Area, King Hussein Street",
        ["PCJ-1020"] = "Amman, Dabouq, Al-Hijaz Street",
        ["PCJ-1021"] = "Salt, Al-Balqa, Prince Hasan Street",
        ["PCJ-1022"] = "Zarqa, Russeifa, Yajouz Road",
        ["PCJ-1023"] = "Amman, Marj Al-Hamam, Airport Road",
        ["PCJ-1024"] = "Mafraq, Um Al-Jimal Road",
        ["PCJ-1025"] = "Amman, Tabarbour, Al-Shahid Street",
        ["PCJ-1026"] = "Aqaba, South Beach Road",
        ["PCJ-1027"] = "Zarqa, Al-Dulayl Road",
        ["PCJ-1028"] = "Salt, Fuheis Road",
        ["PCJ-1029"] = "Madaba, Mount Nebo Road",
        ["PCJ-1030"] = "Jerash, Al-Hadada Street",
        ["PCJ-1031"] = "Aqaba, Tala Bay Road",
        ["PCJ-1032"] = "Amman, Al-Rabiah, Mecca Street",
        ["PCJ-1033"] = "Irbid, Al-Husun, Petra Street",
        ["PCJ-1034"] = "Karak, Mutah, University Street",
        ["PCJ-1035"] = "Amman, Bayader Wadi Al-Seer",
        ["PCJ-1036"] = "Zarqa, Jabal Tareq",
        ["PCJ-1037"] = "Amman, Jabal Amman, Rainbow Street",
        ["PCJ-1038"] = "Irbid, Al-Husun, Yarmouk Road"
    };

    public static async Task InitializeAsync(PetCareJordanContext context)
    {
        await EnsurePetLocationDetailsColumnAsync(context);
        await EnsureCommunityReportReporterColumnsAsync(context);
        await EnsureChatTablesAsync(context);
        var passwordService = new PasswordService();

        if (context.Users.Any())
        {
            await NormalizeExistingUserEmailsAsync(context);
            await NormalizeExistingEnglishTextAsync(context);
            await EnsureRequiredDemoAccountsAsync(context, passwordService);
            await EnsureRealisticSeedPhotosAsync(context);
            await EnsureExpandedDemoPetsAsync(context);
            await RemoveDemoChatArtifactsAsync(context);
            await RemoveDemoPostArtifactsAsync(context);
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
            new() { Name = "Milo", Type = PetType.Cat, Breed = "Persian", AgeInMonths = 18, Gender = PetGender.Male, CollarId = "PCJ-1001", Color = "White", City = "Amman", WeightKg = 4.2m, IsNeutered = true, Description = "Calm indoor cat who loves quiet homes.", PhotoUrl = SeedPetPhotoUrls["PCJ-1001"], OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Bella", Type = PetType.Dog, Breed = "Golden Retriever", AgeInMonths = 30, Gender = PetGender.Female, CollarId = "PCJ-1002", Color = "Golden", City = "Amman", WeightKg = 22.4m, IsNeutered = false, Description = "Friendly and perfect with children.", PhotoUrl = SeedPetPhotoUrls["PCJ-1002"], OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Kiwi", Type = PetType.Bird, Breed = "Cockatiel", AgeInMonths = 10, Gender = PetGender.Female, CollarId = "PCJ-1003", Color = "Grey and yellow", City = "Irbid", WeightKg = 0.1m, IsNeutered = false, Description = "Social bird that whistles a lot.", PhotoUrl = SeedPetPhotoUrls["PCJ-1003"], OwnerId = userByEmail["sara@petcare.com"].Id },
            new() { Name = "Snow", Type = PetType.Rabbit, Breed = "Holland Lop", AgeInMonths = 12, Gender = PetGender.Female, CollarId = "PCJ-1004", Color = "Cream", City = "Zarqa", WeightKg = 1.8m, IsNeutered = true, Description = "Gentle rabbit used to apartment life.", PhotoUrl = SeedPetPhotoUrls["PCJ-1004"], OwnerId = userByEmail["dina@petcare.com"].Id },
            new() { Name = "Simba", Type = PetType.Cat, Breed = "Tabby", AgeInMonths = 24, Gender = PetGender.Male, CollarId = "PCJ-1005", Color = "Brown", City = "Amman", WeightKg = 5.1m, IsNeutered = true, Description = "Playful cat with lots of energy.", PhotoUrl = SeedPetPhotoUrls["PCJ-1005"], OwnerId = userByEmail["ahmad@petcare.com"].Id },
            new() { Name = "Rocky", Type = PetType.Dog, Breed = "German Shepherd", AgeInMonths = 36, Gender = PetGender.Male, CollarId = "PCJ-1006", Color = "Black and tan", City = "Aqaba", WeightKg = 29.7m, IsNeutered = false, Description = "Loyal dog that needs an active owner.", PhotoUrl = SeedPetPhotoUrls["PCJ-1006"], OwnerId = userByEmail["rama@petcare.com"].Id },
            new() { Name = "Lulu", Type = PetType.Cat, Breed = "Siamese", AgeInMonths = 16, Gender = PetGender.Female, CollarId = "PCJ-1007", Color = "Cream and brown", City = "Madaba", WeightKg = 3.9m, IsNeutered = false, Description = "Talkative and affectionate.", PhotoUrl = SeedPetPhotoUrls["PCJ-1007"], OwnerId = userByEmail["tareq@petcare.com"].Id },
            new() { Name = "Coco", Type = PetType.Bird, Breed = "Lovebird", AgeInMonths = 14, Gender = PetGender.Male, CollarId = "PCJ-1008", Color = "Green", City = "Salt", WeightKg = 0.09m, IsNeutered = false, Description = "Bright and cheerful companion.", PhotoUrl = SeedPetPhotoUrls["PCJ-1008"], OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Nala", Type = PetType.Dog, Breed = "Husky", AgeInMonths = 28, Gender = PetGender.Female, CollarId = "PCJ-1009", Color = "Grey and white", City = "Jerash", WeightKg = 20.2m, IsNeutered = true, Description = "Energetic dog that enjoys long walks.", PhotoUrl = SeedPetPhotoUrls["PCJ-1009"], OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Hazel", Type = PetType.Rabbit, Breed = "Mini Rex", AgeInMonths = 9, Gender = PetGender.Female, CollarId = "PCJ-1010", Color = "Brown", City = "Amman", WeightKg = 1.4m, IsNeutered = false, Description = "Curious rabbit with a calm personality.", PhotoUrl = SeedPetPhotoUrls["PCJ-1010"], OwnerId = userByEmail["sara@petcare.com"].Id },
            new() { Name = "Zazu", Type = PetType.Bird, Breed = "African Grey", AgeInMonths = 40, Gender = PetGender.Male, CollarId = "PCJ-1011", Color = "Grey", City = "Amman", WeightKg = 0.4m, IsNeutered = false, Description = "Smart parrot with a growing vocabulary.", PhotoUrl = SeedPetPhotoUrls["PCJ-1011"], OwnerId = userByEmail["dina@petcare.com"].Id },
            new() { Name = "Poppy", Type = PetType.Cat, Breed = "Scottish Fold", AgeInMonths = 20, Gender = PetGender.Female, CollarId = "PCJ-1012", Color = "Silver", City = "Irbid", WeightKg = 4.0m, IsNeutered = true, Description = "Quiet cat that likes window naps.", PhotoUrl = SeedPetPhotoUrls["PCJ-1012"], OwnerId = userByEmail["ahmad@petcare.com"].Id },
            new() { Name = "Max", Type = PetType.Dog, Breed = "Mixed Breed", AgeInMonths = 14, Gender = PetGender.Male, CollarId = "PCJ-1013", Color = "Brown and white", City = "Zarqa", WeightKg = 11.5m, IsNeutered = true, Description = "Rescued dog ready for a second chance.", PhotoUrl = SeedPetPhotoUrls["PCJ-1013"], OwnerId = userByEmail["rama@petcare.com"].Id },
            new() { Name = "Ruby", Type = PetType.Other, Breed = "Hamster", AgeInMonths = 6, Gender = PetGender.Female, CollarId = "PCJ-1014", Color = "Golden", City = "Salt", WeightKg = 0.05m, IsNeutered = false, Description = "Small and easy to care for.", PhotoUrl = SeedPetPhotoUrls["PCJ-1014"], OwnerId = userByEmail["tareq@petcare.com"].Id },
            new() { Name = "Leo", Type = PetType.Cat, Breed = "Orange Tabby", AgeInMonths = 15, Gender = PetGender.Male, CollarId = "PCJ-1015", Color = "Orange", City = "Amman", WeightKg = 4.6m, IsNeutered = true, Description = "Curious and social cat.", PhotoUrl = SeedPetPhotoUrls["PCJ-1015"], OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Lemon", Type = PetType.Bird, Breed = "Canary", AgeInMonths = 8, Gender = PetGender.Female, CollarId = "PCJ-1016", Color = "Yellow", City = "Madaba", WeightKg = 0.03m, IsNeutered = false, Description = "Beautiful singer for a calm home.", PhotoUrl = SeedPetPhotoUrls["PCJ-1016"], OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Bruno", Type = PetType.Dog, Breed = "Boxer", AgeInMonths = 26, Gender = PetGender.Male, CollarId = "PCJ-1017", Color = "Brown", City = "Irbid", WeightKg = 24.2m, IsNeutered = false, Description = "Protective, smart, and playful.", PhotoUrl = SeedPetPhotoUrls["PCJ-1017"], OwnerId = userByEmail["sara@petcare.com"].Id },
            new() { Name = "Mochi", Type = PetType.Rabbit, Breed = "Lionhead", AgeInMonths = 13, Gender = PetGender.Male, CollarId = "PCJ-1018", Color = "White and brown", City = "Jerash", WeightKg = 1.6m, IsNeutered = true, Description = "Fluffy rabbit that enjoys gentle handling.", PhotoUrl = SeedPetPhotoUrls["PCJ-1018"], OwnerId = userByEmail["dina@petcare.com"].Id },
            new() { Name = "Sandy", Type = PetType.Cat, Breed = "Domestic Shorthair", AgeInMonths = 22, Gender = PetGender.Female, CollarId = "PCJ-1019", Color = "Sand", City = "Aqaba", WeightKg = 4.3m, IsNeutered = true, Description = "Relaxed cat suited for first-time owners.", PhotoUrl = SeedPetPhotoUrls["PCJ-1019"], OwnerId = userByEmail["ahmad@petcare.com"].Id },
            new() { Name = "Thor", Type = PetType.Dog, Breed = "Labrador", AgeInMonths = 32, Gender = PetGender.Male, CollarId = "PCJ-1020", Color = "Black", City = "Amman", WeightKg = 27.4m, IsNeutered = true, Description = "Very trainable dog with a calm temperament.", PhotoUrl = SeedPetPhotoUrls["PCJ-1020"], OwnerId = userByEmail["rama@petcare.com"].Id },
            new() { Name = "Pearl", Type = PetType.Cat, Breed = "Persian", AgeInMonths = 27, Gender = PetGender.Female, CollarId = "PCJ-1021", Color = "White", City = "Salt", WeightKg = 4.7m, IsNeutered = false, Description = "Elegant and low-energy companion.", PhotoUrl = SeedPetPhotoUrls["PCJ-1021"], OwnerId = userByEmail["tareq@petcare.com"].Id },
            new() { Name = "Pico", Type = PetType.Bird, Breed = "Budgie", AgeInMonths = 11, Gender = PetGender.Male, CollarId = "PCJ-1022", Color = "Blue", City = "Zarqa", WeightKg = 0.04m, IsNeutered = false, Description = "A small bird that enjoys interaction.", PhotoUrl = SeedPetPhotoUrls["PCJ-1022"], OwnerId = userByEmail["lina@petcare.com"].Id },
            new() { Name = "Daisy", Type = PetType.Rabbit, Breed = "Dutch Rabbit", AgeInMonths = 10, Gender = PetGender.Female, CollarId = "PCJ-1023", Color = "Black and white", City = "Amman", WeightKg = 1.7m, IsNeutered = false, Description = "Compact rabbit suited for indoor life.", PhotoUrl = SeedPetPhotoUrls["PCJ-1023"], OwnerId = userByEmail["yaqeen.alhammad@petcare.com"].Id },
            new() { Name = "Scout", Type = PetType.Other, Breed = "Turtle", AgeInMonths = 48, Gender = PetGender.Male, CollarId = "PCJ-1024", Color = "Green", City = "Madaba", WeightKg = 2.3m, IsNeutered = false, Description = "Healthy turtle with a full habitat setup.", PhotoUrl = SeedPetPhotoUrls["PCJ-1024"], OwnerId = userByEmail["sara@petcare.com"].Id }
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
            new() { PetName = "Shadow", PetType = PetType.Cat, Description = "Black cat with green collar.", ApproximateAgeInMonths = 20, LastSeenPlace = "Jabal Amman near Rainbow Street", LastSeenDateUtc = DateTime.UtcNow.AddDays(-2), RewardAmount = 25, PhotoUrl = SeedLostReportPhotoUrls["Shadow"], ContactName = "Lina Khalil", ContactPhone = "0799001002", Status = ReportStatus.Active },
            new() { PetName = "Biscuit", PetType = PetType.Dog, Description = "Small brown dog, very friendly.", ApproximateAgeInMonths = 14, LastSeenPlace = "Irbid University Street", LastSeenDateUtc = DateTime.UtcNow.AddDays(-1), RewardAmount = null, PhotoUrl = SeedLostReportPhotoUrls["Biscuit"], ContactName = "Ahmad Shannaq", ContactPhone = "0799001008", Status = ReportStatus.Active },
            new() { PetName = "Sunny", PetType = PetType.Bird, Description = "Yellow canary escaped from balcony.", ApproximateAgeInMonths = 9, LastSeenPlace = "Madaba downtown", LastSeenDateUtc = DateTime.UtcNow.AddDays(-3), RewardAmount = 15, PhotoUrl = SeedLostReportPhotoUrls["Sunny"], ContactName = "Rama Azar", ContactPhone = "0799001009", Status = ReportStatus.Active }
        };

        var foundReports = new List<FoundPetReport>
        {
            new() { PetType = PetType.Cat, Description = "Grey cat found with no visible injury.", FoundPlace = "Abdoun, Amman", FoundDateUtc = DateTime.UtcNow.AddDays(-1), PhotoUrl = SeedFoundReportPhotoUrls["Grey cat found with no visible injury."], ContactName = "Dina Majali", ContactPhone = "0799001007", Status = ReportStatus.Active },
            new() { PetType = PetType.Dog, Description = "White mixed-breed dog found near market.", FoundPlace = "Salt city center", FoundDateUtc = DateTime.UtcNow.AddDays(-2), PhotoUrl = SeedFoundReportPhotoUrls["White mixed-breed dog found near market."], ContactName = "Tareq Fares", ContactPhone = "0799001010", Status = ReportStatus.Active }
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
        await EnsureRealisticSeedPhotosAsync(context);
        await EnsureExpandedDemoPetsAsync(context);
        await RemoveDemoChatArtifactsAsync(context);
        await RemoveDemoPostArtifactsAsync(context);
    }

    private static async Task EnsureRealisticSeedPhotosAsync(PetCareJordanContext context)
    {
        var hasChanges = false;

        var pets = await context.Pets.ToListAsync();
        foreach (var pet in pets)
        {
            if (SeedPetPhotoUrls.TryGetValue(pet.CollarId, out var photoUrl) && pet.PhotoUrl != photoUrl)
            {
                pet.PhotoUrl = photoUrl;
                hasChanges = true;
            }

            if (SeedPetLocationDetails.TryGetValue(pet.CollarId, out var locationDetails) && pet.LocationDetails != locationDetails)
            {
                pet.LocationDetails = locationDetails;
                hasChanges = true;
            }
        }

        var lostReports = await context.LostPetReports.ToListAsync();
        foreach (var report in lostReports)
        {
            if (SeedLostReportPhotoUrls.TryGetValue(report.PetName, out var photoUrl) && report.PhotoUrl != photoUrl)
            {
                report.PhotoUrl = photoUrl;
                hasChanges = true;
            }
        }

        var foundReports = await context.FoundPetReports.ToListAsync();
        foreach (var report in foundReports)
        {
            if (SeedFoundReportPhotoUrls.TryGetValue(report.Description, out var photoUrl) && report.PhotoUrl != photoUrl)
            {
                report.PhotoUrl = photoUrl;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureExpandedDemoPetsAsync(PetCareJordanContext context)
    {
        var existingCollarIds = (await context.Pets.Select(pet => pet.CollarId).ToListAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var users = await context.Users.ToListAsync();
        var fallbackOwner = users.FirstOrDefault(user => user.Role == UserRole.User);
        if (fallbackOwner is null)
        {
            return;
        }

        int OwnerId(string email) =>
            users.FirstOrDefault(user => string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))?.Id ?? fallbackOwner.Id;

        var additionalPets = new List<Pet>
        {
            new() { Name = "Mango", Type = PetType.Cat, Breed = "Ginger Domestic Shorthair", AgeInMonths = 11, Gender = PetGender.Male, CollarId = "PCJ-1037", Color = "Orange", City = "Amman", LocationDetails = SeedPetLocationDetails["PCJ-1037"], WeightKg = 3.8m, IsNeutered = true, Description = "Young orange cat who enjoys people and playtime.", PhotoUrl = SeedPetPhotoUrls["PCJ-1037"], OwnerId = OwnerId("lina@petcare.com") },
            new() { Name = "Oscar", Type = PetType.Dog, Breed = "Beagle", AgeInMonths = 22, Gender = PetGender.Male, CollarId = "PCJ-1038", Color = "Tri-color", City = "Irbid", LocationDetails = SeedPetLocationDetails["PCJ-1038"], WeightKg = 12.6m, IsNeutered = false, Description = "Curious beagle with a friendly personality.", PhotoUrl = SeedPetPhotoUrls["PCJ-1038"], OwnerId = OwnerId("sara@petcare.com") },
            new() { Name = "Cotton", Type = PetType.Rabbit, Breed = "White Rabbit", AgeInMonths = 7, Gender = PetGender.Female, CollarId = "PCJ-1027", Color = "White", City = "Zarqa", LocationDetails = SeedPetLocationDetails["PCJ-1027"], WeightKg = 1.2m, IsNeutered = false, Description = "Small rabbit used to indoor handling.", PhotoUrl = SeedPetPhotoUrls["PCJ-1027"], OwnerId = OwnerId("dina@petcare.com") },
            new() { Name = "Sky", Type = PetType.Bird, Breed = "Parakeet", AgeInMonths = 15, Gender = PetGender.Female, CollarId = "PCJ-1028", Color = "Blue", City = "Salt", LocationDetails = SeedPetLocationDetails["PCJ-1028"], WeightKg = 0.04m, IsNeutered = false, Description = "Bright parakeet that responds well to gentle care.", PhotoUrl = SeedPetPhotoUrls["PCJ-1028"], OwnerId = OwnerId("tareq@petcare.com") },
            new() { Name = "Misty", Type = PetType.Dog, Breed = "Poodle", AgeInMonths = 19, Gender = PetGender.Female, CollarId = "PCJ-1029", Color = "Cream", City = "Madaba", LocationDetails = SeedPetLocationDetails["PCJ-1029"], WeightKg = 8.3m, IsNeutered = true, Description = "Smart poodle who is comfortable around children.", PhotoUrl = SeedPetPhotoUrls["PCJ-1029"], OwnerId = OwnerId("rama@petcare.com") },
            new() { Name = "Cleo", Type = PetType.Cat, Breed = "Calico", AgeInMonths = 21, Gender = PetGender.Female, CollarId = "PCJ-1030", Color = "Calico", City = "Jerash", LocationDetails = SeedPetLocationDetails["PCJ-1030"], WeightKg = 4.1m, IsNeutered = true, Description = "Gentle calico cat with a quiet temperament.", PhotoUrl = SeedPetPhotoUrls["PCJ-1030"], OwnerId = OwnerId("ahmad@petcare.com") },
            new() { Name = "Atlas", Type = PetType.Other, Breed = "Turtle", AgeInMonths = 60, Gender = PetGender.Male, CollarId = "PCJ-1031", Color = "Green and brown", City = "Aqaba", LocationDetails = SeedPetLocationDetails["PCJ-1031"], WeightKg = 2.8m, IsNeutered = false, Description = "Healthy turtle with a stable feeding routine.", PhotoUrl = SeedPetPhotoUrls["PCJ-1031"], OwnerId = OwnerId("yaqeen.alhammad@petcare.com") },
            new() { Name = "Dot", Type = PetType.Dog, Breed = "Dalmatian", AgeInMonths = 18, Gender = PetGender.Female, CollarId = "PCJ-1032", Color = "White and black", City = "Amman", LocationDetails = SeedPetLocationDetails["PCJ-1032"], WeightKg = 18.5m, IsNeutered = false, Description = "Active dalmatian who needs daily walks.", PhotoUrl = SeedPetPhotoUrls["PCJ-1032"], OwnerId = OwnerId("lina@petcare.com") },
            new() { Name = "Rio", Type = PetType.Bird, Breed = "Parrot", AgeInMonths = 30, Gender = PetGender.Male, CollarId = "PCJ-1033", Color = "Green", City = "Irbid", LocationDetails = SeedPetLocationDetails["PCJ-1033"], WeightKg = 0.35m, IsNeutered = false, Description = "Social parrot that enjoys supervised interaction.", PhotoUrl = SeedPetPhotoUrls["PCJ-1033"], OwnerId = OwnerId("sara@petcare.com") },
            new() { Name = "Onyx", Type = PetType.Rabbit, Breed = "Black Rabbit", AgeInMonths = 16, Gender = PetGender.Male, CollarId = "PCJ-1034", Color = "Black", City = "Karak", LocationDetails = SeedPetLocationDetails["PCJ-1034"], WeightKg = 1.5m, IsNeutered = true, Description = "Calm rabbit best suited for a peaceful home.", PhotoUrl = SeedPetPhotoUrls["PCJ-1034"], OwnerId = OwnerId("dina@petcare.com") },
            new() { Name = "Peanut", Type = PetType.Other, Breed = "Hamster", AgeInMonths = 5, Gender = PetGender.Male, CollarId = "PCJ-1035", Color = "Brown and white", City = "Amman", LocationDetails = SeedPetLocationDetails["PCJ-1035"], WeightKg = 0.06m, IsNeutered = false, Description = "Tiny hamster with a complete cage setup.", PhotoUrl = SeedPetPhotoUrls["PCJ-1035"], OwnerId = OwnerId("tareq@petcare.com") },
            new() { Name = "Duke", Type = PetType.Dog, Breed = "Labrador Puppy", AgeInMonths = 8, Gender = PetGender.Male, CollarId = "PCJ-1036", Color = "Yellow", City = "Zarqa", LocationDetails = SeedPetLocationDetails["PCJ-1036"], WeightKg = 9.4m, IsNeutered = false, Description = "Friendly labrador puppy ready for training.", PhotoUrl = SeedPetPhotoUrls["PCJ-1036"], OwnerId = OwnerId("rama@petcare.com") }
        };

        var missingPets = additionalPets
            .Where(pet => !existingCollarIds.Contains(pet.CollarId))
            .ToList();

        if (missingPets.Count == 0)
        {
            return;
        }

        await context.Pets.AddRangeAsync(missingPets);
        await context.SaveChangesAsync();
    }

    private static async Task RemoveDemoPostArtifactsAsync(PetCareJordanContext context)
    {
        var testLostReports = await context.LostPetReports
            .Where(report => report.PetName == "Image Test Pet" && report.Description == "Temporary image display test post.")
            .ToListAsync();

        if (testLostReports.Count == 0)
        {
            return;
        }

        context.LostPetReports.RemoveRange(testLostReports);
        await context.SaveChangesAsync();
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

    private static async Task EnsurePetLocationDetailsColumnAsync(PetCareJordanContext context)
    {
        const string sql = """
            IF COL_LENGTH(N'[Pets]', N'LocationDetails') IS NULL
            BEGIN
                ALTER TABLE [Pets] ADD [LocationDetails] NVARCHAR(MAX) NOT NULL CONSTRAINT [DF_Pets_LocationDetails] DEFAULT(N'');
            END
            """;

        await context.Database.ExecuteSqlRawAsync(sql);
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
            if (!Enum.IsDefined(typeof(PetType), pet.Type))
            {
                pet.Type = PetType.Other;
                hasChanges = true;
            }

            hasChanges |= NormalizeTextProperty(value => pet.Name = value, pet.Name);
            hasChanges |= NormalizeTextProperty(value => pet.Breed = value, pet.Breed);
            hasChanges |= NormalizeTextProperty(value => pet.Color = value, pet.Color);
            hasChanges |= NormalizeTextProperty(value => pet.City = value, pet.City);
            hasChanges |= NormalizeTextProperty(value => pet.LocationDetails = value, pet.LocationDetails);
            if (string.IsNullOrWhiteSpace(pet.LocationDetails))
            {
                pet.LocationDetails = pet.City;
                hasChanges = true;
            }

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
            "amman" => "Amman",
            "aqaba" => "Aqaba",
            "irbid" => "Irbid",
            "zarqa" => "Zarqa",
            "mafraq" => "Mafraq",
            "madaba" => "Madaba",
            "salt" => "Salt",
            "jerash" => "Jerash",
            "karak" => "Karak",
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
