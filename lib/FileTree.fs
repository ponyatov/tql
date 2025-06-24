module FileTree

open System
open System.IO

/// Tree structure representing the file system
type FileTree =
    | File of name: string * path: string * lines: string list
    | Dir of name: string * path: string * child: FileTree list

/// full path -> name only
// let name (path: string) : string = Path.GetFileName(path)

/// build new empty file
let newFile (path: string) : FileTree =
    File(name = Path.GetFileName path, path = path, lines = [])
// newFile "/tmp/rtf"

/// ignored files
let ignoreFiles (name: string) : bool =
    not (List.contains name [ ".gitignore" ])

/// ignored dirs
let ignoreDirs (name: string) : bool =
    not (List.contains name [ ".git"; "bin"; "tmp"; "ref"; "obj" ])

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
// ["";"~";"~/";"~/tql";"/~/none"] |> List.map tilde

/// dir depth limit
let maxDepth = 15

/// max file size to read
let maxFileSize = 1024 * 1024

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

    Dir(name = Path.GetFileName path, path = path, child = List.ofArray (Array.append subfiles subdirs))

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

// files "~/Espruino" |> ends ".py"

/// select F# project files
let fsproj: Filter<FileTree> = fun t -> t |> ends ".fsproj"

// files "." |> fsproj

/// filter files contains some string
let rec contains: Filter<string, FileTree> =
    fun s ot ->
        match ot with
        | None -> None
        | Some t ->
            match t with
            | File name as f ->
                if FileInfo(name).Length > maxFileSize then None
                else if File.ReadAllText(name).Contains(s) then Some f
                else None
            | Dir(name, child) ->
                child
                |> List.choose (fun ft -> Some ft |> contains s)
                |> function
                    | [] -> None
                    | filtered -> Some(Dir(name, filtered))

// files "~/Espruino" |> fileEnds ".pyz" |> contains "JSVAR_CACHE_SIZE"
