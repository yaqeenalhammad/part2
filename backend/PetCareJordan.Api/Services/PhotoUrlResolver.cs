using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Services;

public static class PhotoUrlResolver
{
    private const string WikimediaFilePathBase = "https://commons.wikimedia.org/wiki/Special:FilePath/";

    private static readonly IReadOnlyDictionary<int, string> SeedPhotosByLockId = new Dictionary<int, string>
    {
        [1001] = "A cat's direct gaze.jpg",
        [1002] = "Golden Retriever dog at MAV-USP.jpg",
        [1003] = "Cockatiel bird in the cage.jpg",
        [1004] = "Grey Holland Lop rabbit.jpg",
        [1005] = "Cat November 2010-1a.jpg",
        [1006] = "20110425 German Shepherd Dog 8505.jpg",
        [1007] = "Siamese cat, female.jpg",
        [1008] = "Rosy-faced lovebird (Agapornis roseicollis roseicollis).jpg",
        [1009] = "Siberian Husky (6163026231).jpg",
        [1010] = "Daisy the Mini Rex Rabbit.jpg",
        [1011] = "African Grey Parrot, peeking out from under its wing - edit.jpg",
        [1012] = "Scottish fold cat.jpg",
        [1013] = "Mixed-breed dogs in Serles.jpg",
        [1014] = "Baby Hamster - 2 Weeks Old.jpg",
        [1015] = "Orange Tabby Cat 2.jpg",
        [1016] = "YellowCanary.jpg",
        [1017] = "Deutscher Boxer Elmo vom Freudenreich.jpg",
        [1018] = "Lionhead Rabbit.jpg",
        [1019] = "Domestic shorthair cat portrait in grass.jpg",
        [1020] = "Black labrador retriever.jpg",
        [1021] = "Close-up of a cat.JPG",
        [1022] = "Melopsittacus undulatus - Vogelpark Steinen 02.jpg",
        [1023] = "Dutch rabbit.jpg",
        [1024] = "Red-eared sliders and Mallard in Golden Gate Park 1.jpg",
        [1025] = "Syrian Hamster.JPG",
        [1026] = "Maine Coon kittens NO Sigdalskauen.jpg",
        [1027] = "Pet rabbit white 2009.JPG",
        [2001] = "A Black Cat.jpg",
        [2002] = "A small brown dog stands on a sidewalk.jpg",
        [2003] = "YellowCanary.jpg",
        [3001] = "Grey cat.jpg",
        [3002] = "White dog lying on the floor.jpg",
        [4001] = "Agnes the Golden Retriever.jpg",
        [4002] = "A cat's direct gaze.jpg",
        [4003] = "Grey Holland Lop rabbit.jpg",
        [4004] = "20110425 German Shepherd Dog 8505.jpg",
        [4005] = "Cat November 2010-1a.jpg",
        [4006] = "Dutch rabbit.jpg",
        [4007] = "Siberian Husky (6163026231).jpg",
        [4008] = "Scottish fold cat.jpg",
        [4009] = "Lionhead Rabbit.jpg",
        [4010] = "Black labrador retriever.jpg",
        [4011] = "Siamese cat, female.jpg",
        [4012] = "Pet rabbit white 2009.JPG",
        [4013] = "Mixed-breed dogs in Serles.jpg",
        [4014] = "Orange Tabby Cat 2.jpg",
        [4015] = "Daisy the Mini Rex Rabbit.jpg",
        [4016] = "Red-eared sliders and Mallard in Golden Gate Park 1.jpg",
        [4017] = "Cute dog.jpg",
        [4018] = "Close-up of a cat.JPG",
        [4019] = "Cute rabbit.JPG",
        [4020] = "Deutscher Boxer Elmo vom Freudenreich.jpg",
        [4021] = "Domestic shorthair cat portrait in grass.jpg",
        [4022] = "A small brown dog stands on a sidewalk.jpg",
        [4023] = "Grey cat.jpg",
        [4024] = "White dog lying on the floor.jpg",
        [4025] = "A Black Cat.jpg",
        [4026] = "Yellow Labrador Retriever Puppy.jpg",
        [4027] = "Calico cat, - Assisi, Italy.jpg",
        [4028] = "A black rabbit.jpg",
        [4029] = "Ginger Cat (15421086046).jpg",
        [4030] = "Beagle Dog.jpg",
        [9001] = "A cat's direct gaze.jpg",
        [9002] = "Cute dog.jpg",
        [9003] = "Ara ararauna Luc Viatour.jpg",
        [9004] = "Cute rabbit.JPG",
        [9005] = "Turtle.JPG",
        [9006] = "Syrian Hamster.JPG",
        [9007] = "A cat's direct gaze.jpg"
    };

    public static string SeedPhoto(string tag, int lockId)
    {
        var fileName = SeedPhotosByLockId.TryGetValue(lockId, out var lockedFileName)
            ? lockedFileName
            : FallbackFileNameFor(tag);

        return $"{WikimediaFilePathBase}{Uri.EscapeDataString(fileName)}?width=900";
    }

    public static string Resolve(string? photoUrl, PetType petType, string? hint, IWebHostEnvironment environment)
    {
        if (string.IsNullOrWhiteSpace(photoUrl) ||
            IsMissingUploadedFile(photoUrl, environment) ||
            IsUntrustedRemotePhoto(photoUrl))
        {
            return FallbackFor(petType, hint);
        }

        return photoUrl;
    }

    private static bool IsUntrustedRemotePhoto(string photoUrl)
    {
        if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
            !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (uri.AbsolutePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase) ||
            uri.Host.Equals("loremflickr.com", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !uri.Host.Equals("commons.wikimedia.org", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMissingUploadedFile(string photoUrl, IWebHostEnvironment environment)
    {
        var path = photoUrl;
        if (Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
        {
            path = uri.AbsolutePath;
        }

        if (!path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var fileName = Path.GetFileName(path);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return true;
        }

        var physicalPath = Path.Combine(environment.ContentRootPath, "uploads", fileName);
        return !File.Exists(physicalPath);
    }

    private static string FallbackFor(PetType petType, string? hint) =>
        petType switch
        {
            PetType.Cat => SeedPhoto("cat", 9001),
            PetType.Dog => SeedPhoto("dog", 9002),
            PetType.Bird => SeedPhoto("bird", 9003),
            PetType.Rabbit => SeedPhoto("rabbit", 9004),
            PetType.Other when Contains(hint, "turtle") || Contains(hint, "slider") => SeedPhoto("turtle", 9005),
            PetType.Other when Contains(hint, "hamster") || Contains(hint, "syrian") => SeedPhoto("hamster", 9006),
            _ => SeedPhoto("pet", 9007)
        };

    private static bool Contains(string? value, string text) =>
        value?.Contains(text, StringComparison.OrdinalIgnoreCase) == true;

    private static string NormalizeSeedTag(string tag)
    {
        var normalized = tag.ToLowerInvariant();

        return normalized switch
        {
            var value when value.Contains("dog") || value.Contains("retriever") || value.Contains("husky") || value.Contains("boxer") || value.Contains("poodle") || value.Contains("beagle") || value.Contains("dalmatian") => "dog",
            var value when value.Contains("bird") || value.Contains("parrot") || value.Contains("canary") || value.Contains("budgie") || value.Contains("finch") || value.Contains("cockatiel") || value.Contains("lovebird") => "bird",
            var value when value.Contains("rabbit") || value.Contains("lop") || value.Contains("rex") => "rabbit",
            var value when value.Contains("hamster") => "hamster",
            var value when value.Contains("turtle") || value.Contains("slider") => "turtle",
            _ => "cat"
        };
    }

    private static string FallbackFileNameFor(string tag) =>
        NormalizeSeedTag(tag) switch
        {
            "dog" => "Cute dog.jpg",
            "bird" => "Ara ararauna Luc Viatour.jpg",
            "rabbit" => "Cute rabbit.JPG",
            "hamster" => "Syrian Hamster.JPG",
            "turtle" => "Turtle.JPG",
            _ => "A cat's direct gaze.jpg"
        };
}
