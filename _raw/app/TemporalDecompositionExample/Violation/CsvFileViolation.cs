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
