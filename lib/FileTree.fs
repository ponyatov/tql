module FileTree

open System.IO

/// Tree structure representing the file system
type FileTree =
    | File of string
    | Dir of string * FileTree list

/// A filter function that takes a file path and returns whether it matches some condition
type Filter = string -> bool

/// full path -> name only
let filename (fd: string) : string = Path.GetFileName(fd)

let newFile (f: string) : FileTree = File(filename (f))

/// ignored files
let ignoreFiles: Filter =
    fun f -> not (List.contains (filename (f)) [ ".gitignore" ])

/// ignored dirs
let ignoreDirs: Filter =
    fun d -> not (List.contains (filename (d)) [ ".git"; "bin"; "tmp"; "ref"; "obj" ])

/// traverse file system forming tree from a given path
let rec files (path: string) : FileTree =

    let subfiles =
        Directory.GetFiles(path) |> Array.filter ignoreFiles |> Array.map newFile

    let subdirs =
        Directory.GetDirectories(path) |> Array.filter ignoreDirs |> Array.map files

    Dir(filename path, List.ofArray (Array.append subfiles subdirs))
// files "."

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
