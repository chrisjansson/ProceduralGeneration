module LodSelect

type Node = {
        Children : Node list
    }

let lodSelect frustumTester root =
    //test against frustum
    //sphere intersect
    match frustumTester root with
    | true -> [ root ]
    | false -> []