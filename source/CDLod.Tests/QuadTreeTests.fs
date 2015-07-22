module QuadTreeTests

open LodSelect
open QuadTree
open Xunit
open Swensen.Unquote

let zero = { X = 0.0; Y = 0.0; Z = 0.0; }

let zeroBounds = {
        Max = zero
        Min = zero
    }

[<Fact>]
let ``QuadTree with depth of zero contains specified bounds and has no children`` () =
    let actual = makeQuadTree 0 zeroBounds
    test <@ { Children = []; LodLevel = 0; Bounds = zeroBounds } = actual @>