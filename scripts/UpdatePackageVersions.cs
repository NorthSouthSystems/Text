#!/usr/bin/env dotnet
#:package NuGet.Protocol

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
string toolsFilepath = Path.Combine(solutionDirectory, "dotnet-tools.json");

Console.WriteLine($"Loading {propsFilepath} ...");

var propsDoc = XDocument.Load(propsFilepath, LoadOptions.PreserveWhitespace);

var propsVersionVariables = propsDoc
    .Descendants("PropertyGroup")
    .SelectMany(g => g.Elements())
    .Select(e => new PropsVersionVariable(e))
    .ToImmutableArray();

var propsPackageVersions = propsDoc
    .Descendants("PackageVersion")
    .Select(e => new PropsPackageVersion(e))
    .ToImmutableArray();

Console.WriteLine($"Loading {toolsFilepath} ...");

var toolsDoc = JsonNode.Parse(await File.ReadAllTextAsync(toolsFilepath))
               ?? throw new InvalidDataException("Invalid dotnet-tools.json.");

var toolsPackageVersions = (toolsDoc["tools"]?.AsObject()
        ?? throw new InvalidDataException("Missing 'tools' object."))
    .Select(t => new ToolsPackageVersion(t))
    .ToImmutableArray();

var packageIds = propsPackageVersions
    .Select(ppv => ppv.Id)
    .Concat(toolsPackageVersions.Select(t => t.Id))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToImmutableArray();

Console.WriteLine($"Checking {packageIds.Length} packages against nuget.org ...");

var repoPackageMetadataById = new Dictionary<string, RepositoryPackageMetadata>(StringComparer.OrdinalIgnoreCase);

var now = DateTimeOffset.UtcNow;
const int cooldownDays = 7;
var cooldownCutoff = now.AddDays(-cooldownDays);
ImmutableArray<string> cooldownExemptPrefixes = ["Aspire.", "Azure.", "Microsoft.", "NorthSouthSystems.", "System."];

var repo = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
var metadata = await repo.GetResourceAsync<PackageMetadataResource>();
var search = await repo.GetResourceAsync<PackageSearchResource>();

using SourceCacheContext cache = new();

foreach (string packageId in packageIds)
{
    bool isCooldownExempt = false;
    
    if (cooldownExemptPrefixes.Any(cep => packageId.StartsWith(cep, StringComparison.OrdinalIgnoreCase)))
    {
        var searches = await search!.SearchAsync(
            $"packageid:{packageId}",
            new(includePrerelease: true),
            0,
            1,
            NullLogger.Instance,
            default);

        isCooldownExempt = searches.Any(p => 
            string.Equals(p.Identity.Id, packageId, StringComparison.OrdinalIgnoreCase)
            && p.PrefixReserved);
    }
    
    var metadatas = (await metadata!.GetMetadataAsync(
            packageId,
            includePrerelease: true,
            includeUnlisted: false,
            cache,
            NullLogger.Instance,
            default))
        .Where(md => isCooldownExempt || md.Published.GetValueOrDefault(now) <= cooldownCutoff)
        .Select(md => md.Identity.Version)
        .OrderByDescending(v => v)
        .ToImmutableArray();

    if (metadatas.Length == 0)
        await Console.Error.WriteLineAsync($"No listed metadata (after cooldown) found for {packageId}.");

    var repoPackageMetadata = new RepositoryPackageMetadata(
        packageId,
        metadatas.FirstOrDefault(v => !v.IsPrerelease),
        metadatas.FirstOrDefault(v => v.IsPrerelease));

    repoPackageMetadataById.Add(packageId, repoPackageMetadata);
}

int propsChanges = 0;

foreach (var versionVariable in propsVersionVariables)
{
    var referencingPackageIds = propsPackageVersions
        .Where(ppv => string.Equals(ppv.VersionRaw, versionVariable.Reference, StringComparison.Ordinal))
        .Select(ppv => ppv.Id)
        .ToImmutableArray();

    if (referencingPackageIds.Length == 0)
        continue;

    // A newer release of ANY referencing package wins.
    var nextVersion = referencingPackageIds
        .Select(id => SelectVersion(versionVariable.Version, repoPackageMetadataById[id]))
        .OrderByDescending(v => v)
        .First();

    if (versionVariable.Version.CompareTo(nextVersion) >= 0)
        continue;

    Console.WriteLine($"\t$({versionVariable.Name}): {versionVariable.VersionRaw} -> {nextVersion.ToNormalizedString()}");
    versionVariable.VersionRaw = nextVersion.ToNormalizedString();
    
    if (versionVariable.Name == "AspireVersion")
        UpdateGlobalJsonAspire(nextVersion.ToNormalizedString());

    propsChanges++;
}

foreach (var packageVersion in propsPackageVersions.Where(ppv => ppv.CanParse && !ppv.ReferencesVariable))
{
    var nextVersion = SelectVersion(packageVersion.Version, repoPackageMetadataById[packageVersion.Id]);

    if (packageVersion.Version.CompareTo(nextVersion) >= 0)
        continue;

    Console.WriteLine($"\t{packageVersion.Id}: {packageVersion.VersionRaw} -> {nextVersion.ToNormalizedString()}");
    packageVersion.VersionRaw = nextVersion.ToNormalizedString();

    propsChanges++;
}

int toolsChanges = 0;

foreach (var packageVersion in toolsPackageVersions)
{
    var nextVersion = SelectVersion(packageVersion.Version, repoPackageMetadataById[packageVersion.Id]);

    if (packageVersion.Version.CompareTo(nextVersion) >= 0)
        continue;

    Console.WriteLine($"\t{packageVersion.Id}: {packageVersion.VersionRaw} -> {nextVersion.ToNormalizedString()}");
    packageVersion.VersionRaw = nextVersion.ToNormalizedString();

    toolsChanges++;
}

if (propsChanges == 0 && toolsChanges == 0)
{
    Console.WriteLine("Already up to date.");
    return;
}

if (propsChanges > 0)
{
    Console.WriteLine($"{propsChanges} Directory.Package.props elements to update ...");

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
}

if (toolsChanges > 0)
{
    Console.WriteLine($"{toolsChanges} dotnet-tools.json nodes to update ...");

    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        IndentSize = 4
    };
    
    await File.WriteAllTextAsync(toolsFilepath, toolsDoc.ToJsonString(jsonOptions));

    Console.WriteLine($"Updated {toolsFilepath}!");
}

return;

#pragma warning restore

static NuGetVersion SelectVersion(NuGetVersion current, RepositoryPackageMetadata repositoryPackageMetadata)
{
    // Existing stable stays stable.
    // Existing prereleases may graduate to a newer stable release or otherwise advance to a newer prerelease.

    if (repositoryPackageMetadata.Stable is not null
        && repositoryPackageMetadata.Stable.CompareTo(current) >= 0)
        return repositoryPackageMetadata.Stable;

    if (current.IsPrerelease
        && repositoryPackageMetadata.Prerelease is not null
        && repositoryPackageMetadata.Prerelease.CompareTo(current) >= 0)
        return repositoryPackageMetadata.Prerelease;

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

internal sealed record PropsVersionVariable(XElement Element)
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
        get => (string?)Element.Attribute("Version") ?? throw new InvalidDataException("Missing 'Version' attribute.");
        set => Element.SetAttributeValue("Version", value);
    }

    internal NuGetVersion Version => NuGetVersion.Parse(VersionRaw);

    // We need to filter out ranges, etc.
    internal bool CanParse => NuGetVersion.TryParse(VersionRaw, out _);
    internal bool ReferencesVariable => VersionRaw.Contains('$', StringComparison.OrdinalIgnoreCase);
}

internal sealed record ToolsPackageVersion(KeyValuePair<string, JsonNode?> KeyAndNode)
{
    internal string Id => KeyAndNode.Key;

    internal string VersionRaw
    {
        get => (string?)KeyAndNode.Value?["version"] ?? throw new InvalidDataException($"Missing 'version' for {Id}.");
        set => KeyAndNode.Value!["version"] = value;
    }

    internal NuGetVersion Version => NuGetVersion.Parse(VersionRaw);
}

internal sealed record RepositoryPackageMetadata(string Id, NuGetVersion? Stable, NuGetVersion? Prerelease);

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
