// Deep Module Violation — Retry Policy
//
// InventoryApiClient duplicates the same retry logic as OrderApiClient:
// the same attempt count, the same base delay constant, and the same
// exponential backoff formula. Any change to the retry strategy requires
// touching every API client separately — a direct consequence of missing
// a dedicated RetryPolicy module.

class InventoryApiClientShallow
{
    private readonly HttpClient _http;
    public InventoryApiClientShallow(HttpClient http) => _http = http;

    public async Task<int> GetStockAsync(string sku)
    {
        const int maxAttempts = 3;     // same constant duplicated
        const int baseDelayMs = 200;   // same constant duplicated

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var response = await _http.GetAsync($"/inventory/{sku}");
                response.EnsureSuccessStatusCode();
                return int.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException) when (attempt < maxAttempts)
            {
                // same backoff formula duplicated — any change requires touching every caller
                int delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delay);
            }
        }
        throw new InvalidOperationException("unreachable");
    }
}
