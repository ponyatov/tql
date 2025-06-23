# `tql`
## Multi-Syntax Code / Text / Template Query Language

(c) Dmitry Ponyatov <dponyatov@gmail.com> 2025 MIT

github: https://github.com/ponyatov/tql

1. **[[FileTree]] Traversal**
    - Recursive directory scanning with filtering (e.g., by file extension).
    - Support for ignoring directories (e.g., `bin/`, `obj/`).
2. **Multi-Language Syntax Parsing**
    - Extensible parsers for different programming languages 
	    - embedded: C/C++, Rust
	    - project managements & sritps: GNU make, CMake, Python
	    - self-analyzing: F#, JavaScript
    - Abstract Syntax Tree (AST) generation for queried files.
3. **Modular Parser System**
    - Plugins for new languages (e.g., load parsers dynamically).
    - Integration with existing parser libraries (e.g., `FSharp.Text.Lexing`, [[syntax/ANTLR|ANTLR]]).
4. **Query Language Engine**
    - A [[lang/DSL|DSL]] (Domain-Specific Language) to search/transform code/text (e.g., [[XPath]]-like queries for syntax trees).
    - [[lang/pattern matching|pattern matching]] (e.g., "find all `if` statements in Python files").
5. **Visual Studio Code Extension**:
    - Integrate with [[LSP]] (Language Server Protocol) for IDE-like queries.
	- Syntax Highlighting for the [[TQL]] query language itself.
6. **Storage Backend**
    - Persistent storage for parsed syntax trees (e.g., [[LiteDB]], JSON files).
    - Caching to avoid re-parsing unchanged files.
