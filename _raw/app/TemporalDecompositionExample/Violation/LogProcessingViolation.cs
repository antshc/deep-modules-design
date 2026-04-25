// Temporal Decomposition — Log Processing
//
// The log line format ("timestamp | level | message") is a single design
// decision.  It leaks independently into LogLineParser (which splits it),
// LogLevelFilter (which also splits it to inspect the level field), and
// LogLineFormatter (which reassembles it).  A format change forces edits in
// all three classes.

// ── Bad design: three classes split by time order (parse → filter → format) ──

class LogLineParser
{
    public (string Timestamp, string Level, string Message) Parse(string line)
    {
        var parts = line.Split(" | ");
        return (parts[0], parts[1], parts[2]);
    }
}

class LogLevelFilter
{
    private readonly string[] _allowed = ["WARN", "ERROR"];

    public List<(string Timestamp, string Level, string Message)> Filter(
        IEnumerable<string> lines)
    {
        var result = new List<(string, string, string)>();
        foreach (var line in lines)
        {
            var parts = line.Split(" | ");
            if (_allowed.Contains(parts[1]))
                result.Add((parts[0], parts[1], parts[2]));
        }
        return result;
    }
}

class LogLineFormatter
{
    public IEnumerable<string> Format(
        IEnumerable<(string Timestamp, string Level, string Message)> entries)
    {
        return entries.Select(e => $"{e.Timestamp} | {e.Level} | {e.Message}");
    }
}
