internal static class Helpers
{
    internal static string CrawlToRepositoryRoot()
    {
        const string sentinelName = ".git";
        string? directory = Directory.GetCurrentDirectory();

        while (!string.IsNullOrWhiteSpace(directory))
        {
            string sentinelPath = Path.Combine(directory, sentinelName);

            if (Directory.Exists(sentinelPath) || File.Exists(sentinelPath))
                return directory;

            directory = Path.GetDirectoryName(directory);
        }

        throw new DirectoryNotFoundException(sentinelName);
    }
}
