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
    viewWidth / (tan horizontalfov / 2.0)
        
let calculateScreenSpaceError (node:ChunkedLodTreeFactory.ChunkedLodTreeNode) (viewPoint:Vector3d) k =
    let distance = (node.Bounds.Center - viewPoint).Length
    (node.GeometricError / distance) * k

let findVisibleNodes (node:ChunkedLodTreeFactory.ChunkedLodTreeNode) (frustum:Frustum) viewWidth horizontalFov =
    let isInsideViewVolume = isSphereInsideViewVolume frustum
    let k = calculateK viewWidth horizontalFov
