module AABBSphereInteresectionTests

open Xunit
open Swensen.Unquote

[<Fact>]
let ``Sphere intersects with edge of cube`` () =
    let aabb = { Min = { X = -1.0; Y = -1.0; Z = 0.0; }; Max = { X = 1.0; Y = 1.0; Z = 0.0 } }
    let sphere = { Center = { X = 0.0; Y = 2.0; Z = 0.0; }; Radius = 1.0; }

    test <@ aabbSphereIntersects aabb sphere = true @>

[<Fact>]
let ``Sphere does not intersect with cube`` () =
    let aabb = { Min = { X = -1.0; Y = -1.0; Z = 0.0; }; Max = { X = 1.0; Y = 1.0; Z = 0.0 } }
    let sphere = { Center = { X = 0.0; Y = 2.0; Z = 0.0; }; Radius = 1.1; }

    test <@ aabbSphereIntersects aabb sphere = true @>

[<Fact>]
let ``Sphere contained by cube intersects cube`` () =
    let aabb = { Min = { X = -1.0; Y = -1.0; Z = 0.0; }; Max = { X = 1.0; Y = 1.0; Z = 0.0 } }
    let sphere = { Center = { X = 0.0; Y = 0.0; Z = 0.0; }; Radius = 0.0; }

    test <@ aabbSphereIntersects aabb sphere = true @>