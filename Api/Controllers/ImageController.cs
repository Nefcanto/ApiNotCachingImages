namespace Api;

public class ImageController : ControllerBase
{
    private static HttpClient http = new HttpClient();

    private readonly IMemoryCache memoryCache;

    public ImageController(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    [Route("image/resize/{container}/{guid}/{width:int?}/{height:int?}")]
    [HttpGet]
    [ResponseCache(Duration = 30)]
    public IActionResult Resize(string container, Guid guid, int? width, int? height)
    {
        container.Ensure().IsSomething();
        var cacheKey = $"{container}-{guid.ToString()}-{(width.HasValue ? width.Value.ToString() : "no-width")}-{(height.HasValue ? height.Value.ToString() : "no-height")}";
        byte[] imageBytes;
        bool isCached = memoryCache.TryGetValue<byte[]>(cacheKey, out imageBytes);
        if (isCached)
        {
            return File(imageBytes, "image/webp");
        }
        imageBytes = System.IO.File.ReadAllBytes("Image.webp");

        var dimensions = $@"{(width.HasValue ? width.Value.ToString() : "")}x{(height.HasValue ? height.Value.ToString() : "")}\>";
        dimensions = dimensions.Trim('x');

        var path = Path.Combine("/Temp", "ImageResizingAndConversion", guid.ToString());
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        System.IO.File.WriteAllBytes(path, imageBytes);

        var command = $"convert -resize {dimensions} {path} {path}-Resized";
        var result = Terminal.Run(command);
        if (result.ExitCode != 0)
        {
            throw new ServerException($"Termianl error: {result.ExitCode} - {result.Log}");
        }
        var resizedImageBytes = System.IO.File.ReadAllBytes($"{path}-Resized");

        memoryCache.Set(cacheKey, resizedImageBytes);
        return File(resizedImageBytes, "image/webp");
    }
}
