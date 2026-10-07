using System.IO;

internal static class B_RealEstateSalesCsvs
{
    static B_RealEstateSalesCsvs()
    {
        string filepath = Path.Combine(Program.CatalogDataGovDirectory, "Real_Estate_Sales_2001-2020_GL.csv");

        LinuxNewLines = File.ReadAllText(filepath).TrimEnd();

        if (LinuxNewLines.Contains("\r\n"))
            throw new FormatException();

        LinuxNewLinesForcedQuotes = ReJoin(StringQuotedSignals.CsvNewRowLinux, true);

        WindowsNewLines = ReJoin(StringQuotedSignals.CsvNewRowWindows, false);
        WindowsNewLinesForcedQuotes = ReJoin(StringQuotedSignals.CsvNewRowWindows, true);

        string ReJoin(StringQuotedSignals signals, bool forceQuotes) =>
            string.Join(
                signals.NewRow,
                LinuxNewLines.SplitQuotedRows(StringQuotedSignals.CsvNewRowLinux)
                    .Select(row => row.JoinQuotedRow(signals, forceQuotes)));
    }

    internal static bool IsInitialized =>
        new[] { LinuxNewLines, LinuxNewLinesForcedQuotes, WindowsNewLines, WindowsNewLinesForcedQuotes }
            .All(string.IsNotNullAndNotEmpty);

    internal static string LinuxNewLines { get; }
    internal static string LinuxNewLinesForcedQuotes { get; }
    internal static string WindowsNewLines { get; }
    internal static string WindowsNewLinesForcedQuotes { get; }
}
