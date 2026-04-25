# Example: General-Purpose vs. Special-Purpose Text API

Source: *A Philosophy of Software Design*, Chapter 6.2–6.3

## The Context

Students built a GUI text editor. It had to support file display, click-to-position editing, multiple simultaneous views, and multi-level undo/redo. Each project included a text class managing the in-memory file content.

---

## Special-Purpose Design (Violation)

Students modeled the API directly after visible editor features:

```csharp
void backspace(Cursor cursor);         // delete char left of cursor
void delete(Cursor cursor);            // delete char right of cursor
void deleteSelection(Selection selection); // delete selected range
```

Each method maps 1:1 to one UI operation. The `Cursor` and `Selection` types are UI-level abstractions.

**Problems:**
- Many shallow methods, each used in only one place.
- `Cursor` and `Selection` (UI concepts) leak into the text class.
- Adding any new UI feature requires a new method in the text class.
- Developer working on UI must learn every text method; developer working on the text class must understand UI behavior.
- Classes cannot evolve independently.

---

## General-Purpose Design (Fix)

Reduce the API to primitive text operations only:

```csharp
void insert(Position position, String newText);
void delete(Position start, Position end);
Position changePosition(Position position, int numChars); // negative = backwards
```

- `Position` is a neutral type (not UI-specific like `Cursor`).
- `insert` and `delete` operate on arbitrary ranges — no UI semantics.
- `changePosition` navigates text, automatically wrapping lines.

**Implementing UI operations with the general-purpose API:**

```csharp
// Delete key: delete character to the right
text.delete(cursor, text.changePosition(cursor, 1));

// Backspace key: delete character to the left
text.delete(text.changePosition(cursor, -1), cursor);
```

The behavior of each operation is now *explicit and obvious* at the call site — no need to read the text class to understand what `backspace` deletes.

---

## Key Decision

> Replace multiple special-purpose methods (one per UI action) with a small set of general-purpose primitives. Use a neutral `Position` type instead of a UI-specific `Cursor`.

This single decision:
- Pushed all specialization upward into the UI code.
- Left the text class free of any UI knowledge.
- Reduced the number of methods while *increasing* the class's capability (the same two methods handle all deletion scenarios, and the class can now serve non-editor applications too).
- Made the behavior at each call site obvious, eliminating a false abstraction (`backspace` was hiding information the UI developer needed to see).
