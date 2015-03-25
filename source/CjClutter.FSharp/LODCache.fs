module LODCache
open CjClutter.OpenGl

type node = ChunkedLodTreeFactory.ChunkedLodTreeNode

type LODCache = {
        contains : node -> bool
        beginCache : node -> unit
        get : node -> primitives.meshWithNormals
    }

let makeLODCache = 
    


