# Chapter 4: Modules Should Be Deep

a **Modules** can take many forms, such as classes, subsystems, or services. Higher-level subsystems and services are also modules; their interfaces may take different forms, such as kernel calls or HTTP requests. 
a **module** is any unit of code that has an interface and an implementation. 
Each class in an object-oriented programming language is a module.

## 4.2 What's in an interface?

An interface has **formal** and **informal** parts.

- **Formal**: explicitly specified in code; enforced by the language (method signatures, parameter types, return types, exceptions, public variables).
- **Informal**: described only in comments; not enforceable. Includes high-level behavior, side effects, and usage constraints (e.g., call order requirements).

> The informal aspects are typically larger and more complex than the formal ones.

- A clearly specified interface eliminates **"unknown unknowns"** — developers know exactly what they need to use the module, nothing more.

- The interfaces should be designed to make the **common case as simple as possible**.

- **Information hiding** reduces complexity in two ways. First, it simplifies the interface to a module. The interface reflects a simpler, more abstract view of the module’s functionality and hides the details; this reduces the cognitive load on developers who use the module.

- **Information leakage** occurs when the same knowledge is used in multiple places, such as two different classes that both understand the format of a particular type of file.

See: [C# example — ISessionStore](interface-example.md)

## 4.3 Abstractions

An **abstraction** is a simplified view of an entity that omits unimportant details. A module's interface is its abstraction.

Two failure modes:
- **Over-detailed**: includes unimportant details → increases cognitive load.
- **False abstraction**: omits important details → looks simple but isn't; misleads the caller.

The goal: minimize what the caller needs to know. Only expose details that truly matter for correct use.

> File system example: block allocation is hidden (unimportant); flush-to-storage rules are exposed (important for crash safety).

See: [C# example — Good vs. Bad Abstraction](abstraction-example-csharp-example.md)

## Figure 4.1: Deep vs. Shallow Modules
![deep-and-shallow-modules](deep-and-shallow-modules.png)

## 4.4 Deep Modules

A **deep module** provides powerful functionality behind a simple interface. Depth = benefit (functionality) vs. cost (interface complexity). The best modules maximize functionality while minimizing interface surface. The benefit provided by a module is its functionality.

**Example — Unix I/O**: just 5 system calls (`open`, `read`, `write`, `lseek`, `close`) hide hundreds of thousands of lines dealing with disk layout, caching, permissions, scheduling, and device diversity. The interface hasn't changed even as implementations evolved radically.

**Example — Garbage Collector** (Go/Java): has *no* interface at all — it invisibly reclaims memory, actually *shrinking* the system's overall interface by removing the need for manual `free`.

## 4.5 Shallow Modules

A **shallow module** has interface complexity comparable to its implementation complexity — it hides almost nothing.

- Linked-list classes are shallow: the abstraction barely simplifies over direct manipulation.
- Wrapping a one-liner (`data.put(attribute, null)`) in a method *adds* complexity (a new interface to learn) with zero abstraction benefit.

> **Red Flag — Shallow Module**: when the cost of learning the interface negates the benefit of hiding internals. Small modules tend to be shallow.

## 4.6 Classitis

**Classitis** = the mistaken belief that "classes are good, so more classes are better." Breaking code into many tiny classes creates many interfaces that accumulate into high system-level complexity, plus verbose boilerplate.

## 4.7 Java vs. Unix I/O

Java I/O is a classitis example: opening a file for buffered object reading requires three wrapper objects (`FileInputStream` → `BufferedInputStream` → `ObjectInputStream`). Buffering should be the *default*, not an explicit opt-in — design interfaces for the **common case**.

Unix I/O does this well: sequential access is the default; `lseek` exists for random access but most developers never need to know about it. **Effective complexity = complexity of the commonly used features.**

## 4.8 Conclusion

Separate interface from implementation to hide complexity. Design modules to be **deep**: simple interfaces for common use cases, significant functionality underneath. This maximizes concealed complexity.

---

# Chapter 5: Information Hiding (and Leakage)

## 5.1 Information Hiding

Each module should encapsulate design decisions in its implementation, invisible through its interface. Hidden information includes data structures, algorithms, low-level details (page sizes), and higher-level assumptions (most files are small).

**Two complexity wins:**
1. **Simpler interface** — callers see an abstract view, reducing cognitive load (e.g., B-tree users don't think about fanout or balancing).
2. **Easier evolution** — changes to hidden information affect only the owning module (e.g., new TCP congestion control doesn't touch higher-level code).

> **`private` ≠ information hiding.** Getter/setter methods that expose private fields leak information just as much as public fields.

**Partial hiding** still has value: if a feature is only needed by a few callers and accessed through separate methods, it creates fewer dependencies than universally visible information.

## 5.2 Information Leakage

Information leakage = a single design decision reflected in multiple modules. Any change to that decision forces changes across all involved modules.

- **Interface leakage**: information visible in the interface (simpler interfaces correlate with better hiding).
- **Back-door leakage**: two classes share knowledge (e.g., both know a file format) without exposing it in interfaces — *more pernicious* because it's not obvious.

**Fix strategies:**
- Merge small, tightly-coupled classes into one.
- Extract shared knowledge into a new class — but only if it can provide a simple interface that abstracts the details.

> **Red Flag — Information Leakage**: the same knowledge is used in multiple places, such as two different classes that both understand the format of a particular type of file.

## 5.3 Temporal Decomposition

Temporal decomposition = structuring code by the time order of operations (read → modify → write → three classes). This often leaks information because the same knowledge is needed at different stages.

**Example**: a file-format app split into a reader class, a modifier class, and a writer class. Both reader and writer must understand the file format → information leakage. **Fix**: combine reading and writing into a single class; the modifier uses that class in both phases.

> **Red Flag — Temporal Decomposition**: execution order reflected in code structure. If the same knowledge is used at different points in execution, it gets encoded in multiple places.

**Guideline**: focus on *what knowledge* each module needs, not *when* tasks occur.

## 5.5–5.7 Examples

See: [Chapter 5 Examples — HTTP server, parameters, defaults](ch5-information-hiding-examples.md)

> **Red Flag — Overexposure**: if the API for a commonly used feature forces users to learn about rarely used features, it increases cognitive load unnecessarily.

## 5.8 Information Hiding Within a Class

Apply information hiding *inside* a class too:
- Design private methods so each encapsulates some knowledge, hidden from the rest of the class.
- Minimize the number of places each instance variable is used — fewer access points = fewer internal dependencies.

## 5.9 Taking It Too Far

Don't hide information that callers genuinely need. If a module's behavior depends on configuration that varies by use case, expose those parameters. Auto-configuration is better when possible, but recognize when external tuning is necessary.

## 5.10 Conclusion

- Information hiding → deep modules: more functionality behind a simpler interface.
- Modules that hide little are shallow (thin functionality or fat interface).
- Design around **pieces of knowledge**, not execution order — avoid temporal decomposition.
- Encapsulate each piece of knowledge in exactly one module.
