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

type PartialSelection = {
        Parent : Node
        Child : Node
    }

type Selection =
    | Full of Node
    | Partial of PartialSelection

//Optimization, if a parent node is completely inside the frustum all children will be that as well -> frustum cull can be skipped

let rec lodSelect frustumTester detailTester node =      
    let intersectsFrustum = frustumTester node
    let hasChildren = not node.Children.IsEmpty
    match (intersectsFrustum, hasChildren) with
    | (true, false) -> [ Full node ]
    | (true, true) -> 
        let isInNextLodRange = detailTester node (node.LodLevel + 1)
        let converter n =
            match detailTester n n.LodLevel with
            | true -> lodSelect frustumTester detailTester n
            | false -> [ Partial { Parent = node; Child = n } ]
        match isInNextLodRange with
        | true -> List.collect converter node.Children
        | false -> [ Full node ]
    | _ -> []