class InvoiceLoader
{
    public Dictionary<string, string> Load(string path)
    {
        var fields = new Dictionary<string, string>();
        foreach (var line in File.ReadAllLines(path))
        {
            var kv = line.Split('=');
            fields[kv[0].Trim()] = kv[1].Trim();
        }
        return fields;
    }
}

class InvoiceTaxCalculator
{
    public void Apply(Dictionary<string, string> invoice)
    {
        var amount = decimal.Parse(invoice["amount"]);
        var rate   = decimal.Parse(invoice["tax_rate"]);
        invoice["total"] = (amount + amount * rate).ToString("F2");
    }
}

class InvoiceSaver
{
    public void Save(string path, Dictionary<string, string> invoice)
    {
        var lines = invoice.Select(kv => $"{kv.Key} = {kv.Value}");
        File.WriteAllLines(path, lines);
    }
}
