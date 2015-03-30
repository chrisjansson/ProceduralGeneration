module LODCache
open CjClutter.OpenGl

type node = ChunkedLodTreeFactory.ChunkedLodTreeNode

type LODCache = {
        contains : node -> bool
        beginCache : node -> unit
        get : node -> primitives.meshWithNormals
    }

let getNodesToDrawAndCache cache (requestedNodes:node array) =
    let rec getNodesToDrawInternal (requested, notCached) =
        let allAreCached = requested |> Array.forall (fun n -> cache.contains n)
        let isRoot = requestedNodes.Length = 1 && requestedNodes.[0].Parent = null
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

let queueNodes cache (nodes: node array) =
    let largestFirst = nodes |> Array.sortBy (fun n -> n.GeometricError)
    for n in largestFirst do
        cache.beginCache n

let getMeshesFromCache cache (requestedNodes:node array) =
    requestedNodes |> Array.map (fun n -> cache.get n)

let getMeshesToDraw cache (requestedNodes:node array) = 
    let (nodesToDraw, nodesToCache) = getNodesToDrawAndCache cache requestedNodes
    queueNodes cache nodesToCache
    getMeshesFromCache cache nodesToDraw
    
type CachedNode = {
        mutable mesh : option<primitives.meshWithNormals>
    }
   
let makeCache (chunkFactory : node -> primitives.meshWithNormals) =
    let dict = new System.Collections.Concurrent.ConcurrentDictionary<node, CachedNode>()

    let contains node = dict.ContainsKey(node)
    let get node = 
        let m = dict.[node].mesh
        match m with
        | Some mesh -> mesh
        | _ -> failwith "Something went wrong!"
    let beginCache node = 
        let cn = { CachedNode.mesh = None }
        dict.TryAdd(node, cn) |> ignore
        let work = 
            let mesh = chunkFactory node
            cn.mesh <- Some mesh


        ()

    ()




