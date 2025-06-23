module FileTree

open System
open System.IO

/// Tree structure representing the file system
type FileTree =
    | File of string
    | Dir of string * FileTree list

/// full path -> name only
let filename (fd: string) : string = Path.GetFileName(fd)

let newFile (f: string) : FileTree = File(filename (f))

/// ignored files
let ignoreFiles (f: string) : bool =
    not (List.contains (filename (f)) [ ".gitignore" ])

/// ignored dirs
let ignoreDirs (d: string) : bool =
    not (List.contains (filename (d)) [ ".git"; "bin"; "tmp"; "ref"; "obj" ])

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

let files (path: string) : FileTree option = Some(files_ path 0)

// files "."
// files "~"
// files "~/Espruino"

type Filter<'T> = 'T option -> 'T option
type Filter<'S, 'T> = 'S -> 'T option -> 'T option

/// file name ends with
let rec ends: Filter<string, FileTree> =
    fun s ot ->
        match ot with
        | None -> None
        | Some t ->
            match t with
            | File name as f when name.EndsWith s -> Some f
            | File _ -> None
            | Dir(name, child) ->
                child
                |> List.choose (fun ft -> Some ft |> ends s)
                |> function
                    | [] -> None
                    | filtered -> Some(Dir(name, filtered))

/// select F# project files
let fsproj: Filter<FileTree> = fun t -> t |> ends ".fsproj"

/// files "." |> fsproj

let rec contains: Filter<string, FileTree> =
    fun s t ->
        match t with
        | File name as f -> if File.ReadAllText(name).Contains(s) then Some f else None
        | Dir(name, child) ->
            child
            |> List.choose (fun ft -> ft |> contains s)
            |> function
                | [] -> None
                | filtered -> Some(Dir(name, filtered))

// files "." |> fsproj
// files "~/Espruino" |> fileEnds ".pyz" |> contains "JSVAR_CACHE_SIZE"
