namespace Infra;

public static class StreamExtensions
{
    public static byte[] GetBytes(this Stream stream)
    {
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public static string GetText(this Stream stream)
    {
        return new StreamReader(stream).ReadToEnd();
    }

    public static async Task<string> GetTextAsync(this Stream stream)
    {
        return await new StreamReader(stream).ReadToEndAsync();
    }

    public static Stream GzipDecompress(this Stream stream)
    {
        return new GZipStream(stream, CompressionMode.Decompress);
    }
}
