module FileTree

open System.IO

/// A filter function that takes a file path and returns whether it matches some condition
type Filter = string -> bool

/// filter F# projects (.xml)
let proj: Filter = fun file -> file.EndsWith(".fsproj")

/// file tree traversal with provided file names filter
let rec traverse (path: string) (filter: Filter) =
    Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)

// traverse "." proj
