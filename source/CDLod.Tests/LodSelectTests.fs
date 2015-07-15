module LodSelectTests

open Xunit
open LodSelect
open Swensen.Unquote

[<Fact>]
let ``lod selection where root node is out of frustum should be empty`` () =
    let node = { Children = []; LodLevel = 0 }
    let actual = lodSelect (fun _ -> false) (fun _ -> true) node
    test <@ actual = list.Empty @>

[<Fact>]
let ``lod selection where node is inside the frustum in range and the last level selects node`` () =
    let node = { Children = []; LodLevel = 0 }
    let actual = lodSelect (fun _ -> true) (fun _ -> true) node
    test <@ actual = [ node]  @>

[<Fact>]
let ``lod selection where root node is inside the frustum and out of rangeshould be empty`` () =
    let node = { Children = []; LodLevel = 0 }
    let actual = lodSelect (fun _ -> true) (fun _ -> false) node
    test <@ actual = list.Empty  @>

[<Fact>]
let ``lod selection where node is inside the frustum in range and not in range of next lod level selects node`` () =
    let node = {
                Children = [ { Children = []; LodLevel = -1; } ]
                LodLevel = 1
                }

    let detailSelector = (fun level -> match level with 1 -> true | _ -> false)
    let actual = lodSelect (fun _ -> true) detailSelector node
    test <@ actual = [ node ] @>