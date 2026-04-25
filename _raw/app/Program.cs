// ── Usage ─────────────────────────────────────────────────────────────────────
// Classes defined in TemporalDecompositionExample.cs
var reader   = new CsvFileReader();
var modifier = new RecordModifier();
var writer   = new CsvFileWriter();

var records = reader.Read("products.csv");
records     = modifier.DoublePrice(records);
writer.Write("products.csv", records);
