module LodSelectTests

open Xunit
open LodSelect
open Swensen.Unquote

[<Fact>]
let ``lod selection where root node is out of frustum should be empty`` () =
    let node = { Children = [] }
    let actual = lodSelect (fun _ -> false) node
    test <@ actual = list.Empty @>

[<Fact>]
let ``lod selection where root node is inside the frustum and of sufficient detail stops at root node`` () =
    let node = { Children = [] }
    let actual = lodSelect (fun _ -> true) node
    test <@ actual = [ node]  @>
