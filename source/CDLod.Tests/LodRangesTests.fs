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

[<Fact>]
let ``Calculates lod ranges`` () =
    let firstExpected = { VisibilityRange = 571.42857148571; MorphStart = 400.0; MorphEnd = 571.42857148571 }
    let secondExpected = { VisibilityRange = 1714.285714; MorphStart = 1320.0; MorphEnd = 1714.285714 }
    let thirdExpected = { VisibilityRange = 4000.0; MorphStart = 3196.0; MorphEnd = 4000.0 }
    
    let actual = makeLodRanges { LodLevels = 3; LodDistanceRatio = 2.0; VisibilityDistance = 4000.0; MorphStartRatio = 0.7 }
    let first = actual.[0]
    let second = actual.[1]
    let third = actual.[2]

    let assertLodRange expected actual =
        Assert.Equal(expected.VisibilityRange, actual.VisibilityRange, 5)
        Assert.Equal(expected.MorphStart, actual.MorphStart, 5)
        Assert.Equal(expected.MorphEnd, actual.MorphEnd, 5)

    assertLodRange firstExpected first
    assertLodRange secondExpected second
    assertLodRange thirdExpected third
