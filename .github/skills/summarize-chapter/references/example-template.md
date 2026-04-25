# Example: <Title>

Source: *A Philosophy of Software Design*, Chapter X.Y–X.Z

## The Context

<One paragraph: the scenario, the system being built, and what design decision was being made.>

---

## Violation

<What the violation looks like — code first, then analysis.>

```csharp
// Special-purpose / leaky design
```

**Problems:**
- <Problem 1 — one line>
- <Problem 2 — one line>
- <Problem 3 — one line>

---

## Fix

<What the better design looks like — code first, then explanation.>

```csharp
// General-purpose / hidden design
```

<One sentence explaining the key change in the API shape.>

**Using the fix to implement the original operations:**

```csharp
// Show how the violation's use cases are now expressed through the fix API
```

<One sentence: why the behavior is now obvious at the call site.>

---

## Key Decision

> <One sentence that captures the single most important design choice made.>

This single decision:
- <Consequence 1>
- <Consequence 2>
- <Consequence 3>
