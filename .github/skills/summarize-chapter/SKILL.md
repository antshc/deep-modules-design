---
name: summarize-chapter
description: "Summarize a book chapter into concise structured notes and extract code examples into separate files. Use when: taking chapter notes, distilling design principles, writing design summaries, creating example files from chapter content, or organizing design knowledge into a reference document."
argument-hint: "Chapter text, section name, or raw notes file to summarize"
---

# Chapter Summarization

Make a minimalistic summary, but do not miss important keypoints of the chapter. The example must go to a separate file and be referenced. Key decisions explained in the example are added to the main doc.

## Rules

- **Bold term** + one-line definition for concepts.
- Bullets for rules, failure modes, sub-points.
- Blockquote `>` for warnings or red flags.
- Book-supplied examples inline as a single bold line.
- Code examples (violation + fix) → extract to `<slug>-example.md`, reference with `See:` link.
- Match source section numbering (`## 4.4 Deep Modules`).
- No transitional sentences, restatements, or commentary not in the source.

## Code Example Handling

1. Create `<slug>-example.md` with: title, context, violation + problems, fix + explanation, **Key Decision**.
2. In the main notes, add the **Key Decision** as a blockquote and a `See:` link at the point of reference.

```markdown
> **Key Decision — Text API**: Replace special-purpose methods with general-purpose
> primitives (`insert`, `delete`). Use a neutral `Position` type instead of `Cursor`.

See: [Text editor — general-purpose vs. special-purpose API](./text-editor-general-purpose-api-example.md)
```
