// Deep Module Fix — Retry Policy
//
// InventoryApiClientDeep delegates all retry logic to RetryPolicy.
// It supplies only the operation; attempt count, backoff formula,
// jitter, and retriable-exception decisions are invisible to it.

class InventoryApiClientDeep
{
    private readonly HttpClient  _http;
    private readonly RetryPolicy _retry;

    public InventoryApiClientDeep(HttpClient http, RetryPolicy retry)
    {
        _http  = http;
        _retry = retry;
    }

    public Task<int> GetStockAsync(string sku) =>
        _retry.ExecuteAsync(async () =>
        {
            var r = await _http.GetAsync($"/inventory/{sku}");
            r.EnsureSuccessStatusCode();
            return int.Parse(await r.Content.ReadAsStringAsync());
        });
}
