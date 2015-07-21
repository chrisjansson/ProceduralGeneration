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
    test <@ actual = [ Selection.Full node ] @>

[<Fact>]
let ``lod selection where root node is in range of the next lod level and child node is in range of it's lod level selects child`` () =
    let rootLod = 1
    let childLod = 10
    let child = { Children = []; LodLevel = childLod };
    let node = { Children = [ child ]; LodLevel = rootLod; }
    let actual = lodSelect (fun _ -> true) (fun _ _ -> true) node
    test <@ actual = [ Selection.Full child ] @>

[<Fact>]
let ``lod selection where middle node is not in range of next lod level selects middle node`` () =
    let leaf = { Children = []; LodLevel = 10; }
    let middleLevel = { Children = [ leaf ]; LodLevel = 20; }
    let root = { Children = [ middleLevel ]; LodLevel = 30; }

    let detailTester n l =
        match (n, l) with
        | _ when n = root -> true
        | _ when (n, l) = (middleLevel, middleLevel.LodLevel) -> true
        | _ when (n, l) = (middleLevel, middleLevel.LodLevel + 1) -> false
        | _ -> false

    let actual = lodSelect (fun _ -> true) detailTester root
    test <@ actual = [ Selection.Full middleLevel ] @>

[<Fact>]
let ``lod selection where root node is in range of the next lod level and child node is not in range of it's lod level selects parent over child area`` () =
    let rootLod = 1
    let childLod = 10
    let child = { Children = []; LodLevel = childLod };
    let root = { Children = [ child ]; LodLevel = rootLod; }

    let detailSelector n l =
        match n with
        | _ when n = root -> true
        | _ -> false

    let actual = lodSelect (fun _ -> true) detailSelector root
    test <@ actual = [ Selection.Partial { Parent = root; Child = child } ] @>