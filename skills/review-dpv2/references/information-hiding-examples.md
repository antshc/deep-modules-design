# Information Hiding and Leakage Examples

## Good Information Hiding — B-tree

B-tree users don't need to think about fanout or balancing. The data structure's interface exposes lookup/insert/delete; all tree-maintenance complexity is hidden.

**Two wins**: simpler interface (callers think in terms of keys and values) and easier evolution (balancing algorithm can change without affecting callers).

## Good Information Hiding — TCP Congestion Control

Higher-level code sends and receives data. New congestion-control algorithms can be deployed without touching any code above the transport layer.

**Win**: evolution — the hidden decision (congestion algorithm) changes independently.

## Bad — Getters/Setters That Leak

`private` fields with public getter/setter methods expose internal state just as much as public fields. The `private` keyword alone is not information hiding — the information is still visible through the interface.

## Information Leakage — File Format Split

A file-format app split into a reader class, a modifier class, and a writer class. Both reader and writer must understand the file format → information leakage (same knowledge in two places).

**Fix**: combine reading and writing into a single class that owns the format knowledge; the modifier uses that class in both phases.

**Root cause — Temporal decomposition**: the class structure mirrors execution order (read → modify → write). Focus on *what knowledge* each module needs, not *when* tasks occur.

## Back-Door Leakage

Two classes that both understand a protocol, schema, or encoding without exposing it in their interfaces. This is more pernicious than interface leakage because it's not obvious from reading the public API.

**Fix strategies**:
- Merge small, tightly-coupled classes into one.
- Extract shared knowledge into a new class — but only if it can provide a simple interface that abstracts the details.

## Overexposure

An API for a commonly used feature forces callers to learn about rarely used features. This increases cognitive load unnecessarily.

**Example**: requiring callers to specify buffer sizes, encoding, or caching policy when most just want the default behavior.
