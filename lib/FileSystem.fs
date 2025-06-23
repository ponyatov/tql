module FileSystem

let rec traverseDir (query: Query) (dir: string) =
    Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
    |> Seq.filter (matchQuery query)
