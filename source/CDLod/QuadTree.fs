module QuadTree

open LodSelect

let rec makeTree (splitter:Bounds -> Bounds list) depth bounds =
    match depth with
    | 0 -> { Children = []; LodLevel = depth; Bounds = bounds }
    | d -> 
        { 
            Children = bounds |> splitter |> List.map (fun b -> makeTree splitter (d - 1) b)
            LodLevel = d
            Bounds = bounds
        }

let quadXZBoundsSplitter bounds =
    let max = bounds.Max
    let min = bounds.Min
    let center = {
            X = min.X + (max.X - min.X) / 2.0
            Y = 0.0
            Z = min.Z + (max.Z - min.Z) / 2.0
        } 

    [
        { 
            Min = min; 
            Max = { X = center.X; Y = max.Y; Z = center.Z }
        }
        { 
            Min = { X = min.X; Y = min.Y; Z = center.Z } 
            Max = { X = center.X; Y = max.Y; Z = max.Z } 
        }
        { 
            Min = { X = center.X; Y = min.Y; Z = min.Z } 
            Max = { X = max.X; Y = max.Y; Z = center.Z } 
        }
        { 
            Min = { X = center.X; Y = min.Y; Z = center.Z }
            Max = max
        }
    ]

let makeXZQuadTree depth bounds = makeTree quadXZBoundsSplitter depth bounds