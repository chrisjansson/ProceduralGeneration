module LODCache
open CjClutter.OpenGl

type node = ChunkedLodTreeFactory.ChunkedLodTreeNode

type LODCache = {
        contains : node -> bool
        beginCache : node -> unit
        get : node -> primitives.meshWithNormals
    }

let getNodesToDraw cache (requestedNodes:node array) =
    let rec getNodesToDrawInternal (requested, notCached) =
        let allAreCached = requested |> Array.forall (fun n -> cache.contains n)
        let isRoot = requestedNodes.Length = 1
        match allAreCached || isRoot with
        | true -> (requestedNodes, notCached)
        | _ -> 
            let notCachedNodes = requestedNodes |> Array.filter (fun n -> not (cache.contains n))
            let notCachedNodesParents = notCachedNodes |> Array.map (fun n -> n.Parent) |> Array.distinct
            let cachedNodes = requestedNodes |> Array.filter (fun n -> cache.contains n)
            let cachedNodesReadyToBeDrawn = cachedNodes |> Array.filter (fun n -> not (Array.contains n notCachedNodesParents))
            let nodesToRequest = Array.concat [cachedNodesReadyToBeDrawn; notCachedNodesParents]
            getNodesToDrawInternal (nodesToRequest, Array.concat [notCachedNodes; notCached])

    getNodesToDrawInternal (requestedNodes, [||])


