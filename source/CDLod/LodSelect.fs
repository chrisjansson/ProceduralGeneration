module LodSelect

type Point = {
        X : float
        Y : float
        Z : float
    }

type Bounds = {
        Min : Point
        Max : Point
    }

type Node = {
        Children : Node list
        LodLevel : int
    }

//Optimization, if a parent node is completely inside the frustum all children will be that as well -> frustum cull can be skipped

let lodSelect frustumTester detailTester node =
    let intersectsFrustum = frustumTester node
    let isInRange = detailTester node.LodLevel
    //test against frustum
    //sphere intersect
    match (intersectsFrustum, isInRange) with
    | (true, true) -> [ node ]
    | _ -> []