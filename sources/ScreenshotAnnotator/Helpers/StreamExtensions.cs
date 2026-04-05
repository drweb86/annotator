using System.IO;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Helpers;

static class StreamExtensions
{
    public static async Task<byte[]> GetBytes(this Stream stream)
    {
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        return buffer.ToArray();
    }
}
