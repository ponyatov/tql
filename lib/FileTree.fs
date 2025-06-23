module FileTree

open System.IO

/// Tree structure representing the file system
type FileTree =
    | File of string
    | Dir of string * FileTree list

/// ignored directories
let ignoreFiles = [ ".gitignore" ]
let ignoreDirs = [ ".git"; "bin"; "tmp"; "ref"; "obj" ]

/// traverse file system forming tree from a given path
let rec files (path: string) : FileTree =

    let name (fd: string) : string = Path.GetFileName(fd)

    let subfiles =
        Directory.GetFiles(path)
        |> Array.filter (fun f -> not (List.contains (name (f)) ignoreFiles))
        |> Array.map (fun f -> File(name (f)))

    let subdirs =
        Directory.GetDirectories(path)
        |> Array.filter (fun d -> not (List.contains (name (d)) ignoreDirs))
        |> Array.map (fun d -> files d)

    Dir(name (path), List.ofArray (Array.append subfiles subdirs))
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
