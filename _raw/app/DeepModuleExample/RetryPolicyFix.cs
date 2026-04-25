// ── Deep design: RetryPolicy hides all resilience complexity ──────────────────
//
// Interface surface: one method — ExecuteAsync<T>(Func<Task<T>>).
// Hidden inside: attempt counter, base delay, exponential backoff formula,
// random jitter to spread retries, and the decision that HttpRequestException
// is retriable while all other exceptions are not.
// None of those details exist in any caller. Changing the backoff strategy
// (e.g., adding a cap, switching to linear, adjusting jitter range) only
// touches RetryPolicy.

class RetryPolicy
{
    private readonly int      _maxAttempts;
    private readonly TimeSpan _baseDelay;
    private readonly Random   _jitter = new();

    public RetryPolicy(int maxAttempts = 3, TimeSpan? baseDelay = null)
    {
        _maxAttempts = maxAttempts;
        _baseDelay   = baseDelay ?? TimeSpan.FromMilliseconds(200);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException) when (attempt < _maxAttempts)
            {
                // exponential backoff + jitter — invisible to every caller
                double backoffMs = _baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
                int    jitterMs  = _jitter.Next(0, (int)(backoffMs * 0.25));
                await Task.Delay((int)backoffMs + jitterMs);
            }
        }
    }
}


