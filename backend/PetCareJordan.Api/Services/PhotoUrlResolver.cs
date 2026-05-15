using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Services;

public static class PhotoUrlResolver
{
    private static string TaggedPhoto(string tag, int lockId) =>
        $"https://loremflickr.com/900/650/{tag}?lock={lockId}";

    public static string Resolve(string? photoUrl, PetType petType, string? hint, IWebHostEnvironment environment)
    {
        if (string.IsNullOrWhiteSpace(photoUrl) || IsMissingUploadedFile(photoUrl, environment))
        {
            return FallbackFor(petType, hint);
        }

        return photoUrl;
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
            PetType.Cat => TaggedPhoto("cat", 9001),
            PetType.Dog => TaggedPhoto("dog", 9002),
            PetType.Bird => TaggedPhoto("bird", 9003),
            PetType.Rabbit => TaggedPhoto("rabbit", 9004),
            PetType.Other when Contains(hint, "turtle") || Contains(hint, "slider") => TaggedPhoto("turtle", 9005),
            PetType.Other when Contains(hint, "hamster") || Contains(hint, "syrian") => TaggedPhoto("hamster", 9006),
            _ => TaggedPhoto("pet", 9007)
        };

    private static bool Contains(string? value, string text) =>
        value?.Contains(text, StringComparison.OrdinalIgnoreCase) == true;
}
