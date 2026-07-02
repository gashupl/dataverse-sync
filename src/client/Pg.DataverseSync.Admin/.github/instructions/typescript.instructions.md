---
name: 'TypeScript coding conventions'
description: 'Coding style conventions for TypeScript files'
applyTo: '**/*.ts,**/*.tsx'
---

# TypeScript Coding Conventions

## Semicolons
Always terminate statements with a semicolon wherever the TypeScript grammar permits it. This includes variable declarations, assignments, function calls, return statements, throw statements, import/export statements, and type aliases.

## try-catch-finally formatting
The `catch` and `finally` keywords must each begin on their own new line, separate from the closing brace `}` of the preceding block. Do not place `catch` or `finally` on the same line as the closing `}`.

**Correct:**
```ts
try {
    doSomething();
}
catch (error) {
    handleError(error);
}
finally {
    cleanup();
}
```

**Incorrect:**
```ts
try {
    doSomething();
} catch (error) {
    handleError(error);
} finally {
    cleanup();
}
```

## Logical separation with empty lines
Use a single empty line to visually separate logically distinct sections within a function or block. For example, separate variable declarations from the logic that uses them, separate distinct steps in a multi-step operation, and separate the return statement from the preceding logic.
