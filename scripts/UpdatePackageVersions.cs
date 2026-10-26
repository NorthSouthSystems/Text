#!/usr/bin/env dotnet
#:package NuGet.Protocol

using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

#pragma warning disable CA1303 // Passes a literal string as parameter 'value' of a call to 'void Console.WriteLine'

string solutionDirectory = Helpers.CrawlToRepositoryRoot();
string propsFilepath = Path.Combine(solutionDirectory, "Directory.Packages.props");

Console.WriteLine($"Loading {propsFilepath} ...");

var propsDoc = XDocument.Load(propsFilepath, LoadOptions.PreserveWhitespace);

var versionVariables = propsDoc
    .Descendants("PropertyGroup")
    .SelectMany(g => g.Elements())
    .Select(e => new VersionVariable(e))
    .ToImmutableArray();

var propsPackageVersions = propsDoc
    .Descendants("PackageVersion")
    .Select(e => new PropsPackageVersion(e))
    .ToImmutableArray();

Console.WriteLine($"Checking {propsPackageVersions.Length} packages against nuget.org ...");

var repoPackageVersionsById = new Dictionary<string, RepositoryPackageVersion>(StringComparer.OrdinalIgnoreCase);

var now = DateTimeOffset.UtcNow;
const int cooldownDays = 7;
var cooldownCutoff = now.AddDays(-cooldownDays);
ImmutableArray<string> cooldownExemptPrefixes = ["Aspire.", "Azure.", "Microsoft.", "NorthSouthSystems."];

var repo = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
var resource = await repo.GetResourceAsync<PackageMetadataResource>();

using SourceCacheContext cache = new();

foreach (var propsPackageVersion in propsPackageVersions)
{
    bool isCooldownExempt = cooldownExemptPrefixes.Any(cep =>
        propsPackageVersion.Id.StartsWith(cep, StringComparison.OrdinalIgnoreCase));
    
    var versions = (await resource!.GetMetadataAsync(
            propsPackageVersion.Id,
            includePrerelease: true,
            includeUnlisted: false,
            cache,
            NullLogger.Instance,
            default))
        .Where(md => isCooldownExempt || md.Published.GetValueOrDefault(now) <= cooldownCutoff)
        .Select(md => md.Identity.Version)
        .OrderByDescending(v => v)
        .ToImmutableArray();

    if (versions.Length == 0)
        await Console.Error.WriteLineAsync($"No listed versions found for {propsPackageVersion.Id}.");

    var repoPackageVersion = new RepositoryPackageVersion(
        propsPackageVersion.Id,
        versions.FirstOrDefault(v => !v.IsPrerelease),
        versions.FirstOrDefault(v => v.IsPrerelease));

    repoPackageVersionsById.Add(propsPackageVersion.Id, repoPackageVersion);
}

int changes = 0;

foreach (var versionVariable in versionVariables)
{
    var packageIds = propsPackageVersions
        .Where(ppv => string.Equals(ppv.VersionRaw, versionVariable.Reference, StringComparison.Ordinal))
        .Select(ppv => ppv.Id)
        .ToImmutableArray();

    if (packageIds.Length == 0)
        continue;

    // A newer release of ANY referencing package wins.
    var nextVersion = packageIds
        .Select(id => SelectVersion(versionVariable.Version, repoPackageVersionsById[id]))
        .OrderByDescending(v => v)
        .First();

    if (versionVariable.Version.CompareTo(nextVersion) >= 0)
        continue;

    Console.WriteLine($"\t$({versionVariable.Name}): {versionVariable.VersionRaw} -> {nextVersion.ToNormalizedString()}");
    versionVariable.VersionRaw = nextVersion.ToNormalizedString();
    
    if (versionVariable.Name == "AspireVersion")
        UpdateGlobalJsonAspire(nextVersion.ToNormalizedString());

    changes++;
}

foreach (var propsPackageVersion in propsPackageVersions.Where(ppv => ppv.CanParse && !ppv.ReferencesVariable))
{
    var nextVersion = SelectVersion(propsPackageVersion.Version, repoPackageVersionsById[propsPackageVersion.Id]);

    if (propsPackageVersion.Version.CompareTo(nextVersion) >= 0)
        continue;

    Console.WriteLine($"\t{propsPackageVersion.Id}: {propsPackageVersion.VersionRaw} -> {nextVersion.ToNormalizedString()}");
    propsPackageVersion.VersionRaw = nextVersion.ToNormalizedString();

    changes++;
}

if (changes == 0)
{
    Console.WriteLine("Already up to date.");
    return;
}

Console.WriteLine($"{changes} elements to update ...");

var xmlSettings = new XmlWriterSettings()
{
    Async = true,
    Encoding = new UTF8Encoding(false),
    Indent = false,
    OmitXmlDeclaration = propsDoc.Declaration is null,
    NewLineHandling = NewLineHandling.None
};

await using (var writer = XmlWriter.Create(propsFilepath, xmlSettings))
    await propsDoc.SaveAsync(writer, CancellationToken.None);

Console.WriteLine($"Updated {propsFilepath}!");
return;

#pragma warning restore

static NuGetVersion SelectVersion(NuGetVersion current, RepositoryPackageVersion repositoryPackageVersion)
{
    // Existing stable stays stable.
    // Existing prereleases may graduate to a newer stable release or otherwise advance to a newer prerelease.

    if (repositoryPackageVersion.Stable is not null
        && repositoryPackageVersion.Stable.CompareTo(current) >= 0)
        return repositoryPackageVersion.Stable;

    if (current.IsPrerelease
        && repositoryPackageVersion.Prerelease is not null
        && repositoryPackageVersion.Prerelease.CompareTo(current) >= 0)
        return repositoryPackageVersion.Prerelease;

    return current;
}

void UpdateGlobalJsonAspire(string version)
{
    string filepath = Path.Combine(solutionDirectory, "global.json");
    string json = File.ReadAllText(filepath);

    var regex = new Regex(@"(""Aspire\.AppHost\.Sdk""\s*:\s*"")[^""]*("")");

    if (!regex.IsMatch(json))
        throw new InvalidOperationException("Aspire.AppHost.Sdk not found.");

    json = regex.Replace(json, m => $"{m.Groups[1].Value}{version}{m.Groups[2].Value}");

    File.WriteAllText(filepath, json);
}

internal sealed record VersionVariable(XElement Element)
{
    internal string Name => Element.Name.LocalName;
    internal string Reference => "$(" + Name + ")";

    internal string VersionRaw
    {
        get => Element.Value;
        set => Element.Value = value;
    }

    internal NuGetVersion Version => NuGetVersion.Parse(VersionRaw);
}

internal sealed record PropsPackageVersion(XElement Element)
{
    internal string Id => (string?)Element.Attribute("Include")
        ?? throw new InvalidDataException("Missing Include attribute.");

    internal string VersionRaw
    {
        get => (string?)Element.Attribute("Version") ?? throw new InvalidDataException("Missing Version attribute.");
        set => Element.SetAttributeValue("Version", value);
    }

    internal NuGetVersion Version => NuGetVersion.Parse(VersionRaw);

    // We need to filter out ranges, etc.
    internal bool CanParse => NuGetVersion.TryParse(VersionRaw, out _);
    internal bool ReferencesVariable => VersionRaw.Contains('$', StringComparison.OrdinalIgnoreCase);
}

internal sealed record RepositoryPackageVersion(string Id, NuGetVersion? Stable, NuGetVersion? Prerelease);

// See comment in Helpers.cs.
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
