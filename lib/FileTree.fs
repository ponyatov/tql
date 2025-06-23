module FileTree

open System.IO

/// A filter function that takes a file path and returns whether it matches some condition
type Filter = string -> bool


let rec traverse (path: string) (filter: Filter) =
    Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)

traverse "."
