// Deep Module Fix — Retry Policy
//
// OrderApiClientDeep delegates all retry logic to RetryPolicy.
// It supplies only the operation; attempt count, backoff formula,
// jitter, and retriable-exception decisions are invisible to it.

class OrderApiClientDeep
{
    private readonly HttpClient  _http;
    private readonly RetryPolicy _retry;

    public OrderApiClientDeep(HttpClient http, RetryPolicy retry)
    {
        _http  = http;
        _retry = retry;
    }

    public Task<string> GetOrderAsync(int orderId) =>
        _retry.ExecuteAsync(async () =>
        {
            var r = await _http.GetAsync($"/orders/{orderId}");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadAsStringAsync();
        });
}
