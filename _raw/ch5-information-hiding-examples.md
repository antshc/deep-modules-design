# Chapter 5 — Information Hiding Examples

## Example: Too Many Classes (HTTP server)

Students split HTTP request handling into two classes: one to read the request from the network, another to parse the string. This is temporal decomposition — and both classes ended up understanding HTTP structure (e.g., `Content-Length` must be parsed during reading). Parsing code was duplicated.

**Fix**: merge into a single class that reads and parses. This isolates all format knowledge in one place and exposes one method instead of two.

**General theme**: making a class slightly larger often improves information hiding by:
1. Bringing together all code for a capability.
2. Raising the interface level (one method for the whole computation vs. three steps).

## Example: HTTP Parameter Handling

**Good decisions:**
- Merged parameters from URL query string and body — callers don't care where a parameter came from.
- Decoded URL encoding internally — callers get `"What a cute baby!"` not `"What+a+cute+baby%21"`.

**Bad (shallow) interface:**
```csharp
public Map<String, String> getParams() { return this.params; }
```
Exposes internal representation; callers must dig into the Map; any storage change breaks all callers.

**Better (deeper) interface:**
```csharp
public String getParameter(String name) { ... }
public int getIntParameter(String name) { ... }
```
Hides storage, handles type conversion, fewer dependencies.

## Example: Defaults in HTTP Responses

**Bad**: requiring callers to specify the HTTP protocol version on every response — they likely don't know the right value, and it leaks protocol details.

**Good**: the HTTP library sets the version automatically (it already has the request object) and provides a `Date` header by default.

**Defaults = partial information hiding**: in the common case, callers don't even know the defaulted item exists. Override via a special method only when needed.

> **Principle**: classes should "do the right thing" without being asked. The best features are the ones you get without knowing they exist.

**Anti-example — Java I/O**: buffering is universally desired but must be explicitly requested (`BufferedInputStream`). It should be the default.
