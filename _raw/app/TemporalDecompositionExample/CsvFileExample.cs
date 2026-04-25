// Temporal Decomposition — Information Leakage Example
//
// The file format (CSV with header line + comma-separated fields) is a design
// decision that should be hidden in ONE place.  Instead, it leaks into all
// three classes: CsvFileReader, RecordModifier, and CsvFileWriter each know
// the layout independently.  A change to the format (e.g., switching delimiter
// from ',' to '\t') forces edits in every class.

// ── Bad design: three classes split by time order (read → modify → write) ────

/// <summary>Knows the CSV format to parse it.</summary>
class CsvFileReader
{
    public List<string[]> Read(string path)
    {
        var records = new List<string[]>();
        foreach (var line in File.ReadAllLines(path).Skip(1)) // skip CSV header
            records.Add(line.Split(','));                       // knows delimiter
        return records;
    }
}

/// <summary>Knows the field positions inside the CSV row to modify them.</summary>
class RecordModifier
{
    public List<string[]> DoublePrice(List<string[]> records)
    {
        foreach (var r in records)
            r[2] = (double.Parse(r[2]) * 2).ToString(); // knows column index
        return records;
    }
}

/// <summary>Knows the CSV format to write it back.</summary>
class CsvFileWriter
{
    public void Write(string path, List<string[]> records)
    {
        var lines = new List<string> { "id,name,price" };        // duplicates header knowledge
        lines.AddRange(records.Select(r => string.Join(',', r))); // knows delimiter
        File.WriteAllLines(path, lines);
    }
}

// ── Fix: one class owns all CSV format knowledge ─────────────────────────────
//
// CsvFile encapsulates reading and writing.  RecordModifier receives a CsvFile
// in both phases, but never needs to know the delimiter, header, or column
// layout.  A delimiter change only touches CsvFile.

/// <summary>
/// Owns the CSV format (delimiter, header, column layout) in one place.
/// Callers work with plain string arrays and never see the format details.
/// </summary>
class CsvFile
{
    private const string Header = "id,name,price";
    private const char   Delimiter = ',';

    public List<string[]> Read(string path)
    {
        var records = new List<string[]>();
        foreach (var line in File.ReadAllLines(path).Skip(1)) // skip header
            records.Add(line.Split(Delimiter));
        return records;
    }

    public void Write(string path, List<string[]> records)
    {
        var lines = new List<string> { Header };
        lines.AddRange(records.Select(r => string.Join(Delimiter, r)));
        File.WriteAllLines(path, lines);
    }
}

/// <summary>
/// Works with CsvFile for both reading and writing.
/// Contains zero knowledge of the file format.
/// </summary>
class RecordModifierFixed
{
    private readonly CsvFile _csv = new();

    public void DoublePrice(string path)
    {
        var records = _csv.Read(path);          // read phase
        foreach (var r in records)
            r[2] = (double.Parse(r[2]) * 2).ToString();
        _csv.Write(path, records);              // write phase — same class, same format knowledge
    }
}
