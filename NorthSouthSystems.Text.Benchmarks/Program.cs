using BenchmarkDotNet.Running;

namespace NorthSouthSystems.Text;

internal class Program
{
    internal const string CatalogDataGovDirectory = @"C:\CatalogDataGov";

    private static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
