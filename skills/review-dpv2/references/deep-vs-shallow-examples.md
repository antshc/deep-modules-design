# Deep vs. Shallow Module Examples

## Deep Module — Unix I/O

Just 5 system calls (`open`, `read`, `write`, `lseek`, `close`) hide hundreds of thousands of lines dealing with disk layout, caching, permissions, scheduling, and device diversity. The interface hasn't changed even as implementations evolved radically.

**Why it's deep**: tiny interface surface, massive hidden functionality. Sequential access is the default; `lseek` exists for random access but most callers never need it. The common case requires no extra setup.

## Deep Module — Garbage Collector

Go/Java GC has *no* interface at all — it invisibly reclaims memory, actually *shrinking* the system's overall interface by removing the need for manual `free`.

**Why it's deep**: zero interface cost, significant hidden complexity.

## Shallow Module — Linked List Wrapper

A linked-list class where the abstraction barely simplifies over direct pointer manipulation. Wrapping a one-liner (e.g., `data.put(attribute, null)`) in a method *adds* complexity (a new interface to learn) with zero abstraction benefit.

**Why it's shallow**: the cost of learning the interface negates the benefit of hiding internals.

## Classitis — Java I/O

Opening a file for buffered object reading requires three wrapper objects (`FileInputStream` → `BufferedInputStream` → `ObjectInputStream`). Buffering is an explicit opt-in, forcing callers to know about layering details they shouldn't need.

**Why it's bad**: the common case (buffered reading) requires assembling multiple objects. Contrast with Unix I/O where the common case is the default and advanced options are separate.

**Principle**: design interfaces for the common case. Effective complexity = complexity of the commonly used features.
