module LodSelect

type Node = {
        Children : Node list
    }

//Optimization, if a parent node is completely inside the frustum all children will be that as well -> frustum cull can be skipped

let lodSelect frustumTester root =
    //test against frustum
    //sphere intersect
    match frustumTester root with
    | true -> [ root ]
    | false -> []