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
        Bounds : Bounds
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
    match intersectsFrustum with
    | true ->
        let hasChildren = not node.Children.IsEmpty
        let isInNextLodRange = detailTester node (node.LodLevel + 1)
        let converter n =
            match detailTester n n.LodLevel with
            | true -> lodSelect frustumTester detailTester n
            | false -> [ Partial { Parent = node; Child = n } ]
        match (hasChildren, isInNextLodRange) with
        | (false, _) -> [ Full node ]
        | (true, false) -> [ Full node]
        | (true, true) -> List.collect converter node.Children
    | false -> []