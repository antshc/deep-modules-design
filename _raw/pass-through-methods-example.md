# Pass-Through Methods — TextDocument Example

## Context

A GUI text editor project had three intertwined classes: `TextDocument`, `TextArea`, and `TextDocumentListener`. The `TextDocument` class acted as a thin wrapper over `TextArea`, forwarding almost every call.

## Violation

```java
public class TextDocument ... {
    private TextArea textArea;
    private TextDocumentListener listener;

    public Character getLastTypedCharacter() {
        return textArea.getLastTypedCharacter();
    }
    public int getCursorOffset() {
        return textArea.getCursorOffset();
    }
    public void insertString(String textToInsert, int offset) {
        textArea.insertString(textToInsert, offset);
    }
    public void willInsertString(String stringToInsert, int offset) {
        if (listener != null) {
            listener.willInsertString(this, stringToInsert, offset);
        }
    }
}
```

### Problems

- 13 of 15 public methods were pass-through methods — the class is almost entirely hollow.
- Each pass-through method increases interface complexity without adding functionality → shallow class.
- Creates tight coupling: if `TextArea.insertString` signature changes, `TextDocument.insertString` must change too.
- The interface for `insertString` lives in `TextDocument`, but the implementation lives entirely in `TextArea` — responsibility split across two classes for no reason.

## Fix Options (Figure 7.1)

Three refactoring strategies to eliminate pass-through methods:

1. **Expose the lower class directly** — let callers invoke `TextArea` instead of `TextDocument`, removing the feature from the higher class entirely.
2. **Redistribute functionality** — move methods between classes so each has a coherent, non-overlapping set of responsibilities with no inter-class forwarding.
3. **Merge the classes** — if responsibilities can't be disentangled, combine into a single class.

### Actual Fix

The student eliminated pass-through methods by moving methods between classes and collapsing three classes (`TextDocument`, `TextArea`, `TextDocumentListener`) into two, with clearly differentiated responsibilities.

## Key Decision

> **Key Decision — Pass-Through Methods**: When a class forwards most calls to another class with the same signature, the division of responsibility is wrong. Fix by exposing the lower class directly, redistributing functionality, or merging classes — whichever produces coherent, non-overlapping abstractions.
