using System.Text;
using System.Text.RegularExpressions;

namespace Marketplace.Application.Common.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase
        var slug = input.ToLowerInvariant();

        // Remove accents
        slug = RemoveAccents(slug);

        // Replace spaces and special characters with hyphens
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        return slug;
    }

    public static string GenerateUniqueSlug(string baseSlug, Func<string, Task<bool>> slugExistsAsync)
    {
        return GenerateUniqueSlugAsync(baseSlug, slugExistsAsync).GetAwaiter().GetResult();
    }

    public static async Task<string> GenerateUniqueSlugAsync(string baseSlug, Func<string, Task<bool>> slugExistsAsync)
    {
        var slug = GenerateSlug(baseSlug);
        var uniqueSlug = slug;
        var counter = 1;

        while (await slugExistsAsync(uniqueSlug))
        {
            uniqueSlug = $"{slug}-{counter}";
            counter++;
        }

        return uniqueSlug;
    }

    private static string RemoveAccents(string text)
    {
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(text);
        return Encoding.ASCII.GetString(bytes);
    }
}
