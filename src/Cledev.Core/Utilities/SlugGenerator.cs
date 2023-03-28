using Cledev.Core.Extensions;

namespace Cledev.Core.Utilities;

public interface ISlugGenerator
{
    Task<string> GenerateSlug(string text, Func<string, Task<bool>> getAny, int maxRepeat = 5, Func<Task<string>>? getDefault = null);
}

public class SlugGenerator : ISlugGenerator
{
    public async Task<string> GenerateSlug(string text, Func<string, Task<bool>> getAny, int maxRepeat = 5, Func<Task<string>>? getDefault = null)
    {
        var slug = string.Empty;
        var exists = true;
        var repeat = 0;

        while (exists && repeat < maxRepeat)
        {
            var suffix = repeat > 0 ? $"-{repeat}" : string.Empty;
            slug = $"{text.ToSlug()}{suffix}";
            exists = await getAny(slug);
            repeat++;
        }

        if (exists)
        {
            slug = getDefault is null
                ? Guid.NewGuid().ToString()
                : await getDefault();
        }

        return slug;
    }
}