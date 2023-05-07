namespace Simulation.DirectoryBaseHelper.Common.Helper;

public static class DirectoryHelper
{
    private static IWebHostEnvironment? _env;

    public static void DirectoryHelperConfigure(this IServiceProvider serviceProvider)
        => _env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

    public static string? ContentRootPath => _env?.ContentRootPath;
    public static string? WebRootPath => _env?.WebRootPath;

    public static string? ConfigRootPath =>
        ContentRootPath != null ? Path.Combine(ContentRootPath, "Common", "Config") : null;

    public static string? UploadRootPath =>
        WebRootPath != null ? Path.Combine(WebRootPath, "Uploads") : null;

    public static List<string> GetFileNameList(string targetDirectory, string fileFormat)
        => new DirectoryInfo(targetDirectory)
            .GetFiles($"*.{fileFormat}")
            .Select(fileInfo => fileInfo.Name)
            .ToList();
}