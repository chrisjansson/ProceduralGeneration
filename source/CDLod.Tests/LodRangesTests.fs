module LodRangesTests

open Xunit
open Swensen.Unquote
open LodRanges

[<Fact>]
let ``Calculates total detail balance for zero lod levels`` () =
    let actual = calculateTotalDetailBalance 0 2.0
    test <@ actual = 0.0 @>

[<Fact>]
let ``Calculates total detail balance for one lod level`` () =
    let actual = calculateTotalDetailBalance 1 2.0
    test <@ actual = 1.0 @>

[<Fact>]
let ``Calculates total detail balance for three lod levels`` () =
    let actual = calculateTotalDetailBalance 3 2.0
    test <@ actual = 7.0 @>