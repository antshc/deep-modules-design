# Glossary

Key terms from *A Philosophy of Software Design* (Chapters 4–5).

---

**Abstraction** — A simplified view of an entity that omits unimportant details. A module's interface is its abstraction. Two failure modes: *over-detailed* (exposes unimportant details, increasing cognitive load) and *false* (omits important details, misleading the caller).

**Back-door leakage** — Two modules share knowledge (e.g., both understand a file format or protocol) without exposing it in their interfaces. More pernicious than interface leakage because it is not obvious from reading the public API.

**Classitis** — The mistaken belief that "classes are good, so more classes are better." Splitting code into many tiny classes creates many interfaces that accumulate into high system-level complexity plus verbose boilerplate. Java I/O is a classic example.

**Cognitive load** — The amount of knowledge a developer must acquire to use or modify a module. Deep modules reduce cognitive load; shallow modules and information leakage increase it.

**Common-case simplicity** — Interfaces should be designed so that the most frequent usage is the simplest. Advanced or rare options should be separate or defaulted. Effective complexity = complexity of the commonly used features.

**Deep module** — A module that provides powerful functionality behind a simple interface. Depth = functionality ÷ interface complexity. The best modules maximize hidden functionality while minimizing interface surface. Examples: Unix I/O (5 system calls hiding hundreds of thousands of lines) and garbage collectors (zero interface).

**Defaults** — A form of partial information hiding where the module "does the right thing" without being asked. In the common case, callers don't even know the defaulted item exists; overrides are available via separate methods when needed.

**False abstraction** — An abstraction that omits important details. It looks simple but misleads the caller because critical behavior (e.g., deferred flush, crash-safety semantics) is invisible in the interface.

**Formal interface** — The part of a module's interface explicitly specified in code and enforced by the language: method signatures, parameter types, return types, exceptions, and public members.

**Informal interface** — The part of a module's interface described only in comments and documentation; not enforceable by the compiler. Includes high-level behavior, side effects, preconditions, and usage constraints. Typically larger and more complex than the formal part.

**Information hiding** — Each module should encapsulate design decisions in its implementation, invisible through its interface. Two complexity wins: (1) simpler interface — callers see an abstract view, and (2) easier evolution — changes to hidden decisions affect only the owning module. Note: `private` ≠ information hiding; getters/setters that expose internal state leak just as much as public fields.

**Information leakage** — A single design decision reflected in multiple modules. Any change to that decision forces changes across all involved modules. Comes in two forms: *interface leakage* (visible in the public API) and *back-door leakage* (shared knowledge not visible in interfaces).

**Interface leakage** — Information leakage that is visible in the module's public interface. Simpler interfaces correlate with better hiding.

**Module** — Any unit of code that has an interface and an implementation — a class, subsystem, or service. Each class in an OOP language is a module.

**Over-detailed abstraction** — An abstraction that exposes unimportant implementation details (e.g., buffer sizes, internal data structures, storage configuration) through the interface, increasing cognitive load for callers.

**Overexposure** — When the API for a commonly used feature forces callers to learn about rarely used features, increasing cognitive load unnecessarily.

**Partial hiding** — When a feature is only needed by a few callers and accessed through separate methods, it creates fewer dependencies than universally visible information. Still valuable even when full hiding is not possible.

**Shallow module** — A module whose interface complexity is comparable to its implementation complexity — it hides almost nothing. The cost of learning the interface negates the benefit of hiding internals.

**Subdivision** — A class or type that exists only to serve another class; it has no independent consumers and would not make sense on its own. When subdivision classes outnumber standalone modules in a file, it may signal classitis.

**Temporal decomposition** — Structuring code by the time order of operations (e.g., read → modify → write → three classes). This often leaks information because the same knowledge is needed at different stages. Fix: focus on *what knowledge* each module needs, not *when* tasks occur.

**Unknown unknowns** — Behaviors or constraints a developer must discover by trial and error because they are not documented in the interface. A clearly specified interface eliminates unknown unknowns.

---

## Red Flags

| Red Flag | Signal |
|---|---|
| **Shallow Module** | Cost of learning the interface negates the benefit of hiding internals; small modules tend to be shallow |
| **Information Leakage** | The same knowledge is used in multiple places (e.g., two classes that both understand a file format) |
| **Temporal Decomposition** | Execution order reflected in code structure; same knowledge encoded in multiple places |
| **Overexposure** | API for a common feature forces callers to learn about rare features |
