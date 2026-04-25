// Deep Module Violation — Retry Policy
//
// OrderApiClient implements retry logic inline: it knows the attempt count,
// owns the backoff formula (exponential), and decides which exceptions are
// retriable. All resilience decisions are embedded in the caller instead of
// being hidden behind a dedicated policy module.

class OrderApiClientShallow
{
    private readonly HttpClient _http;
    public OrderApiClientShallow(HttpClient http) => _http = http;

    public async Task<string> GetOrderAsync(int orderId)
    {
        const int maxAttempts = 3;
        const int baseDelayMs = 200;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var response = await _http.GetAsync($"/orders/{orderId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException) when (attempt < maxAttempts)
            {
                // caller must know the backoff formula and own the delay calculation
                int delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                await Task.Delay(delay);
            }
        }
        throw new InvalidOperationException("unreachable");
    }
}
