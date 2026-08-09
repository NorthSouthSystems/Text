using BenchmarkDotNet.Engines;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

[MemoryDiagnoser]
public class B_SplitQuotedRows
{
    [GlobalSetup]
    public void GlobalSetup()
    {
        bool _ = B_RealEstateSalesCsvs.IsInitialized;
    }

    private readonly Consumer _consumer = new();

    public enum NewRowType
    {
        Linux,
        Windows
    }

    public enum MappingType
    {
        Skip,
        Array,
        Raw,
        AutoRaw,
        AutoTyped
    }

    [Params(MappingType.Array, MappingType.Raw, Priority = 1)]
    public MappingType Mapping { get; set; }

    [ParamsAllValues(Priority = 2)]
    public NewRowType NewRow { get; set; }

    [ParamsAllValues(Priority = 3)]
    public bool ForcedQuotes { get; set; }

    [ParamsAllValues(Priority = 4)]
    public bool IsNewRowTolerant { get; set; }

    private string GetCsv() => NewRow switch
    {
        NewRowType.Linux => ForcedQuotes ? B_RealEstateSalesCsvs.LinuxNewLinesForcedQuotes : B_RealEstateSalesCsvs.LinuxNewLines,
        NewRowType.Windows => ForcedQuotes ? B_RealEstateSalesCsvs.WindowsNewLinesForcedQuotes : B_RealEstateSalesCsvs.WindowsNewLines,

        _ => throw new NotImplementedException(NewRow.ToString())
    };

    [Benchmark]
    public void SplitQuotedRows()
    {
        string csv = GetCsv();

        var signals = NewRow switch
        {
            NewRowType.Linux => IsNewRowTolerant ? StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180 : StringQuotedSignals.CsvNewRowLinux,
            NewRowType.Windows => IsNewRowTolerant ? StringQuotedSignals.CsvNewRowTolerantWindowsPrimaryRFC4180 : StringQuotedSignals.CsvNewRowWindows,

            _ => throw new NotImplementedException(NewRow.ToString())
        };

        Func<string[], object> mapping = Mapping switch
        {
            MappingType.Array => row => row,
            MappingType.Raw => row => new RealEstateSalesRecordRaw(row),

            _ => throw new NotImplementedException(Mapping.ToString())
        };

        csv.SplitQuotedRows(signals).Select(mapping).Consume(_consumer);
    }

    [Benchmark]
    public void CsvHelper()
    {
        string csv = GetCsv();

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture);
        configuration.PrepareHeaderForMatch = args => args.Header.WhereIsInAnyCategory(CharCategories.Letter).ToUpperInvariant();

        if (!IsNewRowTolerant)
        {
            configuration.NewLine = NewRow switch
            {
                NewRowType.Linux => "\n",
                NewRowType.Windows => "\r\n",

                _ => throw new NotImplementedException(NewRow.ToString())
            };
        }

        using var reader = new StringReader(csv);
        using var csvReader = new CsvReader(reader, configuration);

        var rows = Mapping switch
        {
            MappingType.Skip => CsvHelperMappingSkip(csvReader),
            MappingType.Array => CsvHelperMappingArray(csvReader),
            MappingType.Raw => CsvHelperMappingRaw(csvReader),
            MappingType.AutoRaw => csvReader.GetRecords<RealEstateSalesRecordRaw>().Cast<object>(),
            MappingType.AutoTyped => csvReader.GetRecords<RealEstateSalesRecordTyped>().Cast<object>(),

            _ => throw new NotImplementedException(Mapping.ToString())
        };

        rows.Consume(_consumer);
    }

    private IEnumerable<object> CsvHelperMappingSkip(CsvReader csvReader)
    {
        csvReader.Read();
        csvReader.ReadHeader();

        while (csvReader.Read())
            yield return null;
    }

    private IEnumerable<object> CsvHelperMappingArray(CsvReader csvReader)
    {
        csvReader.Read();
        csvReader.ReadHeader();

        while (csvReader.Read())
        {
            yield return new[]
            {
                csvReader.GetField(0),
                csvReader.GetField(1),
                csvReader.GetField(2),
                csvReader.GetField(3),
                csvReader.GetField(4),
                csvReader.GetField(5),
                csvReader.GetField(6),
                csvReader.GetField(7),
                csvReader.GetField(8),
                csvReader.GetField(9),
                csvReader.GetField(10),
                csvReader.GetField(11),
                csvReader.GetField(12),
                csvReader.GetField(13)
            };
        }
    }

    private IEnumerable<object> CsvHelperMappingRaw(CsvReader csvReader)
    {
        csvReader.Read();
        csvReader.ReadHeader();

        while (csvReader.Read())
            yield return new RealEstateSalesRecordRaw(csvReader);
    }

    // Serial Number,List Year,Date Recorded,Town,Address,Assessed Value,Sale Amount,Sales Ratio,Property Type,Residential Type,Non Use Code,Assessor Remarks,OPM remarks,Location
    private class RealEstateSalesRecordRaw
    {
        public RealEstateSalesRecordRaw(string[] row)
        {
            SerialNumber = row[0];
            ListYear = row[1];
            DateRecorded = row[2];
            Town = row[3];
            Address = row[4];
            AssessedValue = row[5];
            SaleAmount = row[6];
            SalesRatio = row[7];
            PropertyType = row[8];
            ResidentialType = row[9];
            NonUseCode = row[10];
            AssessorRemarks = row[11];
            OPMRemarks = row[12];
            Location = row[13];
        }

        public RealEstateSalesRecordRaw(CsvReader csvReader)
        {
            SerialNumber = csvReader.GetField(0);
            ListYear = csvReader.GetField(1);
            DateRecorded = csvReader.GetField(2);
            Town = csvReader.GetField(3);
            Address = csvReader.GetField(4);
            AssessedValue = csvReader.GetField(5);
            SaleAmount = csvReader.GetField(6);
            SalesRatio = csvReader.GetField(7);
            PropertyType = csvReader.GetField(8);
            ResidentialType = csvReader.GetField(9);
            NonUseCode = csvReader.GetField(10);
            AssessorRemarks = csvReader.GetField(11);
            OPMRemarks = csvReader.GetField(12);
            Location = csvReader.GetField(13);
        }

        public string SerialNumber { get; set; }
        public string ListYear { get; set; }
        public string DateRecorded { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
        public string AssessedValue { get; set; }
        public string SaleAmount { get; set; }
        public string SalesRatio { get; set; }
        public string PropertyType { get; set; }
        public string ResidentialType { get; set; }
        public string NonUseCode { get; set; }
        public string AssessorRemarks { get; set; }
        public string OPMRemarks { get; set; }
        public string Location { get; set; }
    }

    private class RealEstateSalesRecordTyped
    {
        public int SerialNumber { get; set; }
        public int ListYear { get; set; }
        public DateTime? DateRecorded { get; set; }
        public string Town { get; set; }
        public string Address { get; set; }
        public decimal AssessedValue { get; set; }
        public decimal SaleAmount { get; set; }
        public string SalesRatio { get; set; }
        public string PropertyType { get; set; }
        public string ResidentialType { get; set; }
        public string NonUseCode { get; set; }
        public string AssessorRemarks { get; set; }
        public string OPMRemarks { get; set; }
        public string Location { get; set; }
    }
}
