# Example: Editor Undo Mechanism — General vs. Special-Purpose

Source: *A Philosophy of Software Design*, Chapter 6.7

## The Context

The GUI editor required multi-level undo/redo for all changes: text edits, selection changes, cursor movement, and scroll position. Several student projects implemented the entire undo mechanism inside the text class.

---

## Special-Purpose Design (Violation)

The text class managed the history list and added entries whenever text changed. For non-text changes (selection, cursor, view), the UI code called extra methods on the text class, which then added those entries too. On undo, the text class processed the list — updating its own internals for text entries, and calling back into the UI for everything else.

**Problems:**
- The history list (a general mechanism) lived in the text class alongside special-purpose handlers for selection, cursor, and view — UI concepts with no business in the text class.
- Information leakage: the text class and UI had to share knowledge of undo entry formats and call each other through extra back-and-forth methods.
- Adding any new undoable entity required modifying the text class.
- The general-purpose history core had little to do with the general-purpose text facilities — two unrelated concerns crammed into one class.

---

## General-Purpose Design (Fix)

Extract the general-purpose core into a dedicated `History` class:

```csharp
public class History {
    public interface Action {
        void redo();
        void undo();
    }
    void addAction(Action action) { ... }
    void addFence() { ... }  // marks undo group boundary
    void undo() { ... }
    void redo() { ... }
}
```

`History` knows nothing about what the actions do or how they implement undo/redo. It only manages the list and walks it.

**Special-purpose action types live outside `History`**, each implemented by the module that understands that operation:

- Text class creates `UndoableInsert` and `UndoableDelete` on every text modification and calls `History.addAction(...)`.
- UI code creates `UndoableSelection` and `UndoableCursor` when those change.

**Grouping policy** is handled by high-level UI code via `History.addFence()` — it decides which actions belong to one logical undo step.

---

## Responsibilities After the Split

| Category | Who owns it |
|---|---|
| General-purpose history management (list, walk) | `History` class |
| Specific undo/redo logic per action type | Individual action classes (`UndoableInsert`, etc.) |
| Grouping policy (what counts as one undo step) | High-level UI code via `addFence()` |

None of the three needs to understand the others. Adding a new undoable entity means creating a new `Action` implementation — zero changes to `History`.

---

## Key Decision

> Separate the general-purpose undo infrastructure (`History`) from the special-purpose undo implementations (action subclasses). Push action-specific logic down into `Action` implementations; push grouping policy up into the UI layer.

This single decision:
- Removed all UI concepts from the text class.
- Made `History` reusable across any application, not just this editor.
- Eliminated the back-and-forth callback coupling between text class and UI.
- Made adding new undoable entities a local change (one new class) with no ripple effect.

> Note: special-purpose undo code that handles text modifications (`UndoableInsert`, `UndoableDelete`) still lives in the text class — it is special-purpose relative to the undo mechanism, but closely related to text operations. The important separation is from the *general-purpose undo infrastructure*, not from the text class.
