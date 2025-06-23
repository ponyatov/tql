module FileTree

open System.IO

/// Traverse files without filtering (for use with piping)
let files (path: string) =
    Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
// files "."

/// A filter function that takes a file path and returns whether it matches some condition
type Filter = string -> bool

/// filter F# projects (.xml)
let fsproj: Filter = fun file -> file.EndsWith(".fsproj")

// Create a content filter looks into file
let contentFilter (text: string) : Filter =
    fun file ->
        try
            File.ReadAllText(file).Contains(text)
        with _ ->
            false

// files "."
// |> Seq.filter fsproj
// |> Seq.filter (contentFilter "TQL")
