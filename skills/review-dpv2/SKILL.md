---
name: review-dpv2
description: "Review C# classes or files for deep-module design quality. Use when: reviewing code for interface depth, information hiding, leakage, shallow modules, classitis, temporal decomposition, abstraction quality, or interface formal/informal contract quality."
argument-hint: One or more file paths or classes to review for deep-module design principles
---

# Deep Module Design Review

Analyze one or more C# source files against deep-module design principles. When multiple files are provided, identify which files/classes are **standalone modules** (parent modules that present an interface and hide implementation) and which are **subdivision / helper modules** that form part of that hidden implementation. Then assess whether each standalone module is deep or shallow. Produce a structured report identifying design strengths, violations, and actionable improvements.

> This skill is C#-oriented. Adapt terminology (e.g., `interface` → protocol/trait, `class` → struct/module) when reviewing other languages.

## Definitions

### Module
A **module** is any unit of code that has an interface and an implementation — a class, subsystem, or service. Each class in an OOP language is a module.

### Interface
A module's interface has two parts:
- **Formal**: signatures, parameter types, return types, exceptions, public members — enforced by the language.
- **Informal**: behavior descriptions, side effects, usage constraints (e.g., call order) — conveyed only through comments.

The informal aspects are typically larger and more complex than the formal ones.

### Abstraction
A simplified view of an entity that omits unimportant details. A module's interface is its abstraction.

Two failure modes:
- **Over-detailed abstraction**: exposes unimportant details → increases cognitive load.
- **False abstraction**: omits important details → looks simple but misleads the caller.

### Standalone Module (Parent Module)
A module that presents its own meaningful interface to external consumers and hides its implementation behind it. A standalone module may delegate to internal subdivision/helper modules that callers never see.

Being a standalone module is a **role**, not a quality judgment. A standalone module can be **deep** (good) or **shallow** (bad) — the depth assessment is separate.

### Deep Module
A standalone module that provides powerful functionality behind a simple interface. Depth = functionality ÷ interface complexity. The best modules maximize hidden functionality while minimizing interface surface.

### Shallow Module
A standalone module whose interface complexity is comparable to its implementation complexity — it hides almost nothing. The cost of learning the interface negates the benefit of hiding internals.

### Information Hiding
Each module should encapsulate design decisions in its implementation, invisible through its interface.

Two complexity wins:
1. **Simpler interface** — callers see an abstract view, reducing cognitive load.
2. **Easier evolution** — changes to hidden information affect only the owning module.

> `private` ≠ information hiding. Getter/setter methods that expose internal state leak information just as much as public fields.

### Information Leakage
A single design decision reflected in multiple modules. Any change to that decision forces changes across all involved modules.

- **Interface leakage**: information visible in the public interface.
- **Back-door leakage**: two modules share knowledge (e.g., both know a file format) without exposing it in interfaces — more pernicious because it's not obvious.

### Classitis
The mistaken belief that "more classes are better." Too many tiny classes create too many interfaces, accumulating high system-level complexity plus verbose boilerplate.

### Temporal Decomposition
Structuring code by the time order of operations (read → modify → write → three classes). This leaks information because the same knowledge is needed at different stages.

Focus on **what knowledge** each module needs, not **when** tasks occur.

## Review Procedure

### Step 1 — Map the file structure

Read all target files. For each class/type found, record:
- **Source file** where the class is defined
- Class name and its role (standalone module vs. helper/subdivision)
- Public interface: methods, properties, events, constructors
- Dependencies: what it imports, injects, or calls

**When multiple files are provided**, build a cross-file dependency graph:
- Which files/classes depend on which others (via constructor injection, direct instantiation, or method calls)?
- Which files define abstractions (interfaces/abstract classes) and which files implement them?
- Which files are consumed by external callers vs. only consumed internally within the reviewed set?

### Step 2 — Classify each class (within and across files)

For each class, determine if it is:

| Classification | Indicator |
|---|---|
| **Standalone module** | A parent module that presents its own interface to external consumers and hides implementation behind it. Can be **deep** or **shallow** — depth is assessed separately in Step 4. |
| **Subdivision / helper** | Exists only to serve a standalone module; has no independent consumers; would not make sense on its own. Part of the standalone module's hidden implementation. |

**When multiple files are provided**, apply classification at the file level too:

| File-level Classification | Indicator |
|---|---|
| **Standalone module file** | Contains a parent module whose public API is the entry point for external consumers. It orchestrates or delegates to subdivision files that callers never see. Can be deep or shallow — depth is assessed separately. |
| **Subdivision / helper file** | Contains classes that are part of a standalone module's hidden implementation. Only consumed internally by the standalone module — not by external callers. Would not be useful on its own. Examples: internal data types, strategy implementations, format-specific logic, accessor layers. |
| **Shared infrastructure file** | Contains cross-cutting utilities consumed by multiple standalone modules (e.g., retry logic, logging helpers). Neither a standalone module nor a subdivision of one specific module. |

To classify files, use the cross-file dependency graph from Step 1:
1. **Identify entry points**: files/classes that external code (outside the reviewed set) depends on → these are standalone module candidates.
2. **Trace inward**: files only reachable from a single standalone module → subdivisions of that module.
3. **Flag shared**: files reachable from multiple standalone modules → shared infrastructure.
4. **Validate depth**: an entry-point file that hides almost nothing (thin wrapper) is still a standalone module by role, but flag it as **shallow** — its subdivisions do the real work while it adds interface cost without hiding complexity.

If subdivision files outnumber standalone module files, flag potential **classitis**.

### Step 3 — Validate interface quality (formal + informal)

For each public interface or class API, evaluate both contract layers:

**Formal contract** (enforced by the compiler):
- Are parameter types specific enough? (`Guid userId` vs `string userId`; `TimeSpan ttl` vs `int seconds`)
- Are return types honest? (`Task<Session?>` — nullable signals "may not exist")
- Are exceptions part of the contract or hidden surprises?

**Informal contract** (conveyed only through comments/docs):
- Are preconditions documented? (e.g., "Create before Get")
- Are side effects and edge-case behaviors stated? (e.g., "no-op if already expired")
- Does the caller know the full contract from the interface alone — without reading the implementation?
- Are there **unknown unknowns** — behaviors the caller must discover by trial and error?

> The informal aspects are typically larger and more complex than the formal ones. If XML doc comments are missing or vague, flag this as a gap.

See: [Interface Design Example](./references/interface-quality-example.md)

### Step 4 — Evaluate interface depth

For each standalone module, assess:

1. **Interface surface**: count public members (methods, properties, events). Fewer = better.
2. **Functionality behind the interface**: estimate how much complexity the module hides (algorithms, state management, I/O, error handling).
3. **Depth ratio**: high hidden functionality with small interface = **deep** (good). Interface ≈ implementation = **shallow** (bad).
4. **Common-case simplicity**: does the interface optimize for the most frequent usage? Or does it force callers to deal with rare cases?

### Step 5 — Evaluate abstraction quality

Check for:
- **Over-detailed abstraction**: interface exposes implementation details (internal data structures, algorithm parameters, storage formats) that callers should not need.
- **False abstraction**: interface looks simple but callers must understand hidden assumptions to use it correctly — important details are omitted.
- **Leaky getters/setters**: properties that merely expose internal fields without meaningful abstraction.

### Step 6 — Check information hiding

For each design decision in the module, ask: is this visible only inside the module?

Common decisions that **should** be hidden:
- Data structures and their layout
- Algorithms and optimization strategies
- Low-level details (buffer sizes, page sizes, encoding)
- Higher-level assumptions (e.g., "most inputs are small")

Flag any hidden decision that **should** be exposed (caller genuinely needs it for correct use).

### Step 7 — Detect information leakage

Scan for the same knowledge appearing in multiple places:

- Two classes that both understand a data format, protocol, or schema.
- Parallel data structures that must be kept in sync.
- Constants or magic values duplicated across classes.
- Shared assumptions about ordering, encoding, or structure.

Classify as **interface leakage** or **back-door leakage**.

### Step 8 — Detect temporal decomposition

Check if the file's class structure mirrors execution order (e.g., `Reader` → `Processor` → `Writer`). If the same knowledge is needed at multiple stages, the decomposition leaks information.

### Step 9 — Check for overexposure

Does the API for a commonly used feature force callers to learn about rarely used features? The common case should be simple; advanced options should be separate or defaulted.

## Output Format

Produce the report with these sections:

```
## Module Review: [file name(s)]

### Summary
[1–3 sentence verdict: deep/shallow, major issues]

### What Works Well
[Highlight genuinely deep modules, effective information hiding, clean abstractions, and well-designed interfaces. Call out design decisions that reduce system complexity.]

### Module Dependency Map (multi-file reviews only)
[Show which files/classes are deep modules, which are subdivisions/helpers, and the dependency direction between them.]

| File | Classification | Serves | Rationale |
|------|---------------|--------|-----------|
| ServiceX.cs | Standalone module | (entry point) | Orchestrates copying; hides queue + blob logic behind 2 public methods |
| AccessorY.cs | Subdivision | ServiceX | Internal accessor for blob storage; only used by ServiceX |
| RetryHelper.cs | Shared infrastructure | ServiceX, ServiceZ | Cross-cutting retry logic used by multiple modules |

### Class Map
| Class | File | Role | Public Members | Depth | Verdict |
|-------|------|------|----------------|-------|---------|
| ...   | file.cs | Standalone module / Subdivision / Shared | count | Deep / Shallow | ✅ / ⚠️ / ❌ |

### Interface Depth Analysis
[Per-class assessment of interface surface vs hidden functionality]

### Abstraction Quality
[Over-detailed? False? Leaky getters/setters?]

### Information Hiding
[What is well hidden? What should be hidden but isn't?]

### Information Leakage
[Shared knowledge across classes. Interface vs back-door leakage.]

### Structural Issues
[Classitis? Temporal decomposition? Overexposure?]

### Recommendations
[Numbered, actionable improvements ranked by impact]
```

## Red Flags Checklist

Use this checklist during the review. Flag any that apply:

- [ ] **Shallow Module** — interface complexity ≈ implementation complexity; hides almost nothing.
- [ ] **Information Leakage** — same knowledge encoded in multiple places.
- [ ] **Temporal Decomposition** — class structure mirrors execution order; same knowledge split across stages.
- [ ] **Overexposure** — common-case API forces learning about rare-case features.
- [ ] **Classitis** — excessive subdivision into tiny classes with many interfaces.
- [ ] **False Abstraction** — interface looks simple but omits details callers need.
- [ ] **Over-detailed Abstraction** — interface exposes unimportant implementation details.

## Examples

When you need concrete examples to illustrate findings or compare against, load:
- [Deep vs. Shallow Module Examples](./references/deep-vs-shallow-examples.md)
- [Information Hiding and Leakage Examples](./references/information-hiding-examples.md)
- [Interface Quality Example — ISessionStore](./references/interface-quality-example.md)
