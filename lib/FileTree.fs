module FileTree

open System
open System.IO

/// Tree structure representing the file system
type FileTree =
    | File of string
    | Dir of string * FileTree list

// /// A filter function that takes a file path and returns whether it matches some condition
// type Filter = string -> bool

/// full path -> name only
let filename (fd: string) : string = Path.GetFileName(fd)

let newFile (f: string) : FileTree = File(filename (f))

/// ignored files
let ignoreFiles = fun f -> not (List.contains (filename (f)) [ ".gitignore" ])

/// ignored dirs
let ignoreDirs =
    fun d -> not (List.contains (filename (d)) [ ".git"; "bin"; "tmp"; "ref"; "obj" ])


/// fix error with `~` in path
let tilde (path: string) : string =
    if path.StartsWith("~") then
        let home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

        if path.Length = 1 then
            home
        else
            Path.Combine(home, path.Substring(2))
    else
        path


/// dir depth limit
let maxDepth = 15

/// traverse file system forming tree from a given path
let rec files_ (path: string) (count: int) : FileTree =

    if count > maxDepth then
        failwith $"filetree depth overflow >{maxDepth}"

    let path = tilde (path)

    let subfiles =
        Directory.GetFiles(path) |> Array.filter ignoreFiles |> Array.map newFile

    let subdirs =
        Directory.GetDirectories(path)
        |> Array.filter ignoreDirs
        |> Array.map (fun d -> files_ d (count + 1))

    Dir(filename path, List.ofArray (Array.append subfiles subdirs))

let files (path: string) : FileTree = files_ path 0

// files "."
// files "~"
// files "~/Espruino"

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
