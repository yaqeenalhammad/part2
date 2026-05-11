using PetCareJordan.Api.Models;

namespace PetCareJordan.Api.Services;

public static class PhotoUrlResolver
{
    private static string PexelsPhoto(int id) =>
        $"https://images.pexels.com/photos/{id}/pexels-photo-{id}.jpeg?auto=compress&cs=tinysrgb&w=900&h=650&fit=crop";

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
            PetType.Cat => PexelsPhoto(15116820),
            PetType.Dog => PexelsPhoto(458799),
            PetType.Bird => PexelsPhoto(11961251),
            PetType.Rabbit => PexelsPhoto(3730206),
            PetType.Other when Contains(hint, "turtle") || Contains(hint, "slider") => PexelsPhoto(18497947),
            PetType.Other when Contains(hint, "hamster") || Contains(hint, "syrian") => PexelsPhoto(4588050),
            _ => PexelsPhoto(1108099)
        };

    private static bool Contains(string? value, string text) =>
        value?.Contains(text, StringComparison.OrdinalIgnoreCase) == true;
}
