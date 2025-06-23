module FileSystem

let rec traverse (dir:string) =
    Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
// let rec traverseDir (query: Query) (dir: string) =
//     
//     |> Seq.filter (matchQuery query)
