module LOD
open OpenTK
open CjClutter.OpenGl

type Sphere = {
        Center : Vector3d
        Radius : float
    }

type Frustum = Vector4d[]

type ViewVolumeSphereIntersector = Frustum -> Sphere -> bool

let isSphereInsideViewVolume frustum sphere =   
    let distanceToPlane (plane:Vector4d) (point:Vector3d) = 
        plane.X * point.X + plane.Y * point.Y + plane.Z * point.Z + plane.W

    Array.forall (fun p -> distanceToPlane p sphere.Center >= -sphere.Radius) frustum

let calculateK viewWidth horizontalfov =
    viewWidth / (tan (horizontalfov / 2.0))
        
let calculateScreenSpaceError (node:ChunkedLodTreeFactory.ChunkedLodTreeNode) (viewPoint:Vector3d) k =
    let nodeCenter = new Vector3d(node.Bounds.Center.X, node.Bounds.Center.Y, 0.0)
    let distance = (nodeCenter - viewPoint).Length
    (node.GeometricError / distance) * k

let findVisibleNodes (node:ChunkedLodTreeFactory.ChunkedLodTreeNode) (frustum:Frustum) viewWidth horizontalFov viewPoint allowedScreenSpaceError =
    let isInsideViewVolume = isSphereInsideViewVolume frustum
    let k = calculateK viewWidth horizontalFov

    let isDetailedEnough node =
        let screenSpaceError = calculateScreenSpaceError node viewPoint k
        screenSpaceError <= allowedScreenSpaceError

    let rec findVisibleNodesRec (node:ChunkedLodTreeFactory.ChunkedLodTreeNode) = 
        let center = new Vector3d(node.Bounds.Center.X, node.Bounds.Center.Y, 0.0)
        let delta = node.Bounds.Max - node.Bounds.Min;
        let side = max (max delta.X delta.Y) (60.0)
        let radius = sqrt (side*side + side*side);
        let sphere = { Center = center; Radius = radius }
        let m = (isInsideViewVolume sphere, node.IsLeaf(), isDetailedEnough node)
        match m with
        | (false, _, _) -> [||]
        | (_, true, _) -> [| node |]
        | (_, _, true) -> [| node |]
        | (_, _, false) -> Array.collect (fun n -> findVisibleNodesRec n) node.Nodes
        
    findVisibleNodesRec node

type ChunkedLod() = 
    interface CjClutter.OpenGl.IChunkedLod with
        member this.Calculate(root, viewportWidth, horizontalFov, cameraPosition, allowedScreenSpacePosition, frustumPlanes) =
            let a = (findVisibleNodes root frustumPlanes viewportWidth horizontalFov cameraPosition allowedScreenSpacePosition)
            new System.Collections.Generic.List<ChunkedLodTreeFactory.ChunkedLodTreeNode>(a)
    