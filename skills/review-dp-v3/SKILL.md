---
name: review-dp-v3
description: Review a code diff for deep-module design quality, information hiding, abstraction quality, facade design, and subdivision correctness.
argument-hint: "<diff file/path or pasted diff>"
---

# Deep Module Diff Review Skill

Use this skill when the user provides a code diff and asks for a design-principles review based on deep modules, information hiding, abstraction quality, facade modules, and subdivision design.

The review must focus on how the diff changes module depth and caller complexity. Do not review only syntax or style unless it affects the design contract.

## Core Review Goal

Determine whether the diff makes the system easier to understand, change, and use correctly by validating:

- deep modules: small interface with significant hidden implementation
- information hiding: important design decisions are hidden inside the owning module
- information leakage: design knowledge is not duplicated across unrelated modules
- abstraction quality: names, contracts, parameters, and behavior match the real concept
- facade quality: facade modules simplify callers instead of forwarding calls
- subdivision quality: helper/subdivision classes move complexity downward without becoming new public concepts
- ease of correct use: the API guides callers toward the correct behavior
- resistance to misuse: the API does not require hidden call order, mode flags, or internal knowledge

## Input

The input is a code diff.

The diff may be pasted directly or loaded from a file. Review changed code first, then inspect surrounding code only when needed to understand the design boundary, responsibility, callers, or hidden assumptions.

## Required Change Classification

For every changed class, interface, method, constructor, or public contract, classify the change as one of the following cases:

| Change Case | Review Focus |
|---|---|
| New method added to existing class | Does it deepen the module or bloat the interface? |
| New facade module/class added | Does it hide workflow complexity or only forward calls? |
| New subdivision/helper class added | Does it move complexity downward without leaking knowledge? |
| New method added to subdivision/helper class | Does it support hidden implementation or expose helper internals upward? |
| Existing method changed | Did the contract, behavior, or hidden assumptions change? |
| New public type/interface added | Is it a real abstraction or unnecessary classitis? |
| New dependency added | Does it belong below the abstraction boundary? |
| New configuration/flag added | Does it expose policy or implementation mode to callers? |

## Review Procedure

### Step 1: Map the Diff

Build a short map of changed symbols:

- symbol name
- file path
- change case
- visibility: public, internal, protected, private
- owning module/class
- new dependencies
- callers affected, if visible from the diff
- new concepts exposed to callers

Do not skip this step. The changed-symbol map is the basis for the design review.

### Step 2: Review New Methods Added to Existing Classes

When a method is added to an existing class, validate:

- Does the method belong to the existing class abstraction?
- Does it make the class deeper, or only wider?
- Is the new method needed by many callers or only by one special case?
- Could the behavior be expressed through an existing method without making the interface ambiguous?
- Does the method expose internal lifecycle, storage, cache, protocol, retry, ordering, or fallback decisions?
- Does it force callers to know when or in what order to call it?
- Does the name describe caller intent instead of implementation mechanics?
- Are parameters simple and stable from the caller perspective?
- Does the return type expose implementation state?
- Is the formal contract clear from types?
- Is the informal contract documented when behavior is non-obvious?

Flag these problems:

- public method added for one special caller
- method exposes internal state or resource lifecycle
- method forces callers to coordinate low-level steps
- method introduces temporal coupling
- method adds boolean flags or modes that change behavior significantly
- method makes the class wider without hiding more complexity

### Step 3: Review New Facade Module Classes

A facade is good only when it reduces caller complexity.

Validate:

- Does the facade provide a simpler abstraction than the wrapped services?
- Does it coordinate multiple lower-level modules so callers do not need to?
- Does it hide sequencing, retries, mapping, validation, error handling, resource ownership, or policy decisions?
- Does it reduce the number of concepts visible to callers?
- Is the facade interface smaller and more intention-revealing than the combined lower-level interfaces?
- Does it protect callers from implementation churn in lower-level modules?
- Does the facade own a meaningful use case or domain operation?

Flag these problems:

- facade only forwards calls one-to-one
- facade exposes the same parameters as the lower-level services
- facade leaks implementation names from wrapped classes
- facade requires callers to understand the internal workflow
- facade adds another public interface without reducing caller complexity
- facade mixes unrelated use cases and becomes a generic service locator

### Step 4: Review New Subdivision or Helper Classes

A subdivision/helper class is good when it moves complexity downward and remains below the parent abstraction boundary.

Validate:

- Is the subdivision owned by a parent module?
- Does it hide a coherent internal design decision?
- Is it private or internal unless there is a strong reason to expose it?
- Does the parent module remain the simple entry point?
- Does the subdivision reduce complexity inside the parent without creating a new public concept?
- Can callers use the parent module without knowing the subdivision exists?
- Does the subdivision have one clear responsibility?
- Does it avoid duplicating rules already known by the parent or sibling classes?

Flag these problems:

- helper has a broad public interface
- parent and helper both understand the same low-level protocol or data structure
- constants, ordering rules, lifecycle rules, or fallback rules are duplicated
- helper exists only because of temporal decomposition
- subdivision creates more interfaces than complexity it hides
- helper name describes an implementation step instead of a hidden concept

### Step 5: Review Methods Added to Subdivision or Helper Classes

When a method is added to a subdivision/helper class, validate:

- Does the method support the parent module's hidden implementation?
- Is the method still below the abstraction boundary?
- Does it avoid leaking helper concepts upward?
- Does it avoid requiring the parent to micromanage low-level details?
- Does it keep policy decisions in one place?
- Is it cohesive with the helper's purpose?

Flag these problems:

- helper method exposes internal state
- helper method requires call-order knowledge
- helper method makes the parent orchestrate low-level details
- helper method should be private implementation detail
- helper method spreads one design decision across multiple classes

### Step 6: Review Deep Module Depth

For each affected module, decide whether the diff changed module depth.

Use these questions:

- Did the public API grow?
- Did the hidden implementation power grow more than the interface complexity?
- Did the common caller path become simpler or harder?
- Did the module hide more design decisions than before?
- Did the module become easier to use correctly?
- Did the module become harder to misuse?
- Can future internal changes happen with fewer caller changes?

Classify the result:

- Deepened: interface stayed small or simpler while hidden power increased
- Neutral: no meaningful impact on depth
- Made shallower: interface grew without hiding enough complexity
- Introduced leakage: callers now need internal design knowledge

### Step 7: Review Information Hiding

Identify design decisions introduced or modified by the diff:

- storage or layout decisions
- cache/resource lifecycle decisions
- ordering or sequencing decisions
- retry/fallback decisions
- protocol/schema/format decisions
- algorithm or optimization decisions
- error-handling policy
- concurrency/threading policy
- configuration precedence
- validation policy

For each design decision, verify:

- Is it hidden inside one owning module?
- Is it exposed only if callers genuinely need it?
- Is it duplicated across classes?
- Can it change without modifying many callers?
- Are tests coupled to the public behavior rather than internal mechanics?

### Step 8: Review Information Leakage

Look for two forms of leakage:

1. Interface leakage: public API exposes implementation knowledge.
2. Back-door leakage: several classes privately depend on the same hidden rule.

Flag evidence such as:

- duplicated constants
- duplicated condition logic
- parallel data structures
- repeated mapping logic
- caller-side branching based on callee internals
- comments that explain the same hidden rule in multiple places
- tests that must know too much about internals
- public parameters named after internal mechanisms
- return values that expose internal state transitions

### Step 9: Review Abstraction Quality

Validate abstraction names, contracts, and parameters:

- Does the name describe what the caller wants, not how it is implemented?
- Is the abstraction simpler than the implementation?
- Are parameters stable concepts from the caller perspective?
- Are primitive values wrapped when a stronger domain type would prevent misuse?
- Are exceptions, side effects, and fallback behavior visible in the contract or documented?
- Does the class expose getters/setters instead of behavior?
- Does the API require reading implementation to use it safely?

Flag:

- over-detailed abstraction
- false abstraction
- vague names
- leaky getters/setters
- primitive obsession that enables misuse
- boolean flags that expose modes
- contract ambiguity

### Step 10: Review Classitis and Temporal Decomposition

Classitis signs:

- new class with little behavior and no meaningful hidden decision
- interface added only because a class was added
- many small classes that make the design harder to understand
- new layer that only delegates

Temporal decomposition signs:

- classes or methods named after execution steps instead of stable concepts
- caller must execute several methods in a specific order
- workflow knowledge is spread across caller, facade, and helper
- each class owns a phase rather than hiding a design decision

Do not suggest splitting classes unless the split hides a coherent design decision.
Do not suggest adding interfaces unless they reduce coupling or clarify abstraction.

## Finding Rules

Report only actionable findings.

A finding is actionable when it shows that the diff:

- increases caller cognitive load
- exposes hidden design decisions
- makes the module shallower
- spreads knowledge across modules
- makes future changes harder
- makes correct usage harder
- introduces a facade that does not simplify callers
- introduces a subdivision that leaks upward

Prefer fewer, higher-quality findings over a long list of weak comments.

## Output Format

Use this exact structure:

```markdown
## Deep Module Diff Review

### Summary
[1-3 sentence verdict. State whether the diff deepens the design, is neutral, or makes the module shallower.]

### Changed Symbols Map

| Symbol | File | Change Case | Visibility | Role | Verdict |
|---|---|---|---|---|---|
| ... | ... | New method / Facade / Subdivision / etc. | public/internal/private | Module/Subdivision/Facade | ✅ / ⚠️ / ❌ |

### What Works Well
- ...

### Design Findings

#### Finding 1: [short title]
**Severity:** High / Medium / Low  
**Changed code:** [class/method/file]  
**Problem:** ...  
**Why it matters:** ...  
**Deep-module principle:** Interface depth / Information hiding / Information leakage / Abstraction quality / Classitis / Temporal decomposition  
**Recommendation:** ...

### Case-Specific Review

#### New Methods on Existing Classes
- ...

#### New Facade Classes
- ...

#### New Subdivision Classes
- ...

#### Methods Added to Subdivision Classes
- ...

### Information Hiding and Leakage
- ...

### Final Recommendation
[Approve / Approve with comments / Request changes]
```

## Tone and Style

- Be concise and direct.
- Explain the design impact, not only the code smell.
- Prefer concrete recommendations over abstract advice.
- When a design is acceptable, say why.
- When a design is risky, explain what future change becomes harder.
- Do not invent code that is not present in the diff.
- Do not require perfect design; focus on meaningful risks.

## Hard Rules

- Do not treat `private` as automatic information hiding.
- Do not treat small classes as automatically good.
- Do not treat a facade as good unless it reduces caller complexity.
- Do not treat a subdivision as good unless it moves complexity downward.
- Do not recommend a new abstraction unless it hides a real design decision.
- Always connect findings to deep module depth, information hiding, or ease of correct use.
