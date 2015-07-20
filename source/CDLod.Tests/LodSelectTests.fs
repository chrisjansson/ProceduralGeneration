module LodSelectTests

open Xunit
open LodSelect
open Swensen.Unquote


[<Fact>]
let ``lod selection where root node is out of frustum should be empty`` () =
    let node = { Children = []; LodLevel = 0 }
    let actual = lodSelect (fun _ -> false) (fun _ _ -> true) node
    test <@ actual = list.Empty @>

[<Fact>]
let ``lod selection where root node is in frustum and has no children selects root`` () =
    let node = { Children = []; LodLevel = 0 }
    let actual = lodSelect (fun _ -> true) (fun _ _ -> true) node
    test <@ actual = [ node ] @>

[<Fact>]
let ``lod selection where root node is in range of the next lod level and child node is in range of it's lod level selects child`` () =
    let rootLod = 1
    let childLod = 10
    let child = { Children = []; LodLevel = childLod };
    let node = { Children = [ child ]; LodLevel = rootLod; }
    let actual = lodSelect (fun _ -> true) (fun _ _ -> true) node
    test <@ actual = [ child ] @>


[<Fact>]
let ``lod selection where root node is not in range of the next lod level selects root`` () =
    let node = { Children = [ { Children = []; LodLevel = 1 } ]; LodLevel = 0 }
    let actual = lodSelect (fun _ -> true) (fun _ _ -> false) node
    test <@ actual = [ node ] @>

//
//[<Fact>]
//let ``lod selection where node is inside the frustum in range and the last level selects node`` () =
//    let node = { Children = []; LodLevel = 0 }
//    let actual = lodSelect (fun _ -> true) (fun _ -> true) node
//    test <@ actual = [ node]  @>
//
//[<Fact>]
//let ``lod selection where root node is inside the frustum and out of rangeshould be empty`` () =
//    let node = { Children = []; LodLevel = 0 }
//    let actual = lodSelect (fun _ -> true) (fun _ -> false) node
//    test <@ actual = list.Empty  @>
//
//[<Fact>]
//let ``lod selection where node is inside the frustum in range and not in range of next lod level selects node`` () =
//    let node = {
//                Children = [ { Children = []; LodLevel = -1; } ]
//                LodLevel = 1
//            }
//
//    let detailSelector = (fun level -> match level with 1 -> true | _ -> false)
//    let actual = lodSelect (fun _ -> true) detailSelector node
//    test <@ actual = [ node ] @>
//
//[<Fact>]
//let ``lod selection where node is inside the frustum in range and in range of next lod level selects child node`` () =
//    let node = {
//                Children = [ { Children = []; LodLevel = 2; } ]
//                LodLevel = 1
//            }
//
//    let detailSelector = (fun level -> match level with 1 | 2 -> true | _ -> false)
//    let actual = lodSelect (fun _ -> true) detailSelector node
//    test <@ actual = [ { Children = []; LodLevel = 2; } ] @>