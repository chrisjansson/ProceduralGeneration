module LodSelectTests

open Xunit
open LodSelect
open Swensen.Unquote

let zero = { X = 0.0; Y = 0.0; Z = 0.0; }

let zeroBounds = {
        Max = zero
        Min = zero
    }

[<Fact>]
let ``lod selection where root node is out of frustum should be empty`` () =
    let node = { Children = []; LodLevel = 0; Bounds = zeroBounds }
    let actual = lodSelect (fun _ -> false) (fun _ _ -> true) node
    test <@ actual = list.Empty @>

[<Fact>]
let ``lod selection where root node is in frustum and has no children selects root`` () =
    let node = { Children = []; LodLevel = 0; Bounds = zeroBounds }
    let actual = lodSelect (fun _ -> true) (fun _ _ -> true) node
    test <@ actual = [ Selection.Full node ] @>

[<Fact>]
let ``lod selection where root node is in range of the next lod level and child node is in range of it's lod level selects child`` () =
    let rootLod = 1
    let childLod = 10
    let child = { Children = []; LodLevel = childLod; Bounds = zeroBounds };
    let node = { Children = [ child ]; LodLevel = rootLod; Bounds = zeroBounds }
    let actual = lodSelect (fun _ -> true) (fun _ _ -> true) node
    test <@ actual = [ Selection.Full child ] @>

[<Fact>]
let ``lod selection where middle node is not in range of next lod level selects middle node`` () =
    let leaf = { Children = []; LodLevel = 10; Bounds = zeroBounds }
    let middleLevel = { Children = [ leaf ]; LodLevel = 20; Bounds = zeroBounds }
    let root = { Children = [ middleLevel ]; LodLevel = 30; Bounds = zeroBounds }

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
    let child = { Children = []; LodLevel = childLod; Bounds = zeroBounds };
    let root = { Children = [ child ]; LodLevel = rootLod; Bounds = zeroBounds }

    let detailSelector n l =
        match n with
        | _ when n = root -> true
        | _ -> false

    let actual = lodSelect (fun _ -> true) detailSelector root
    test <@ actual = [ Selection.Partial { Parent = root; Child = child } ] @>

[<Fact>]
let ``lod selection integration test`` () =
    let leafInRange = { Children = []; LodLevel = 1; Bounds = zeroBounds }
    let leafOutOfRage = { Children = []; LodLevel = 2; Bounds = zeroBounds }
    let firstMiddleNode = { Children = [ leafInRange; leafOutOfRage ]; LodLevel = 3; Bounds = zeroBounds }
    let middleNodeOutOfFrustum = { Children = []; LodLevel = 4; Bounds = zeroBounds }
    let middleNodeWithEnoughDetail = { Children = [ { Children = []; LodLevel = 5; Bounds = zeroBounds } ]; LodLevel = 6; Bounds = zeroBounds }
    let root = { Children = [ firstMiddleNode; middleNodeOutOfFrustum; middleNodeWithEnoughDetail ]; LodLevel = 7; Bounds = zeroBounds }

    let frustumIntersector n =
        match n with
        | _ when n = middleNodeOutOfFrustum -> false
        | _ -> true

    let detailSelector n l =
        match n with
        | _ when (n, l) = (middleNodeWithEnoughDetail, middleNodeWithEnoughDetail.LodLevel + 1) -> false
        | _ when n = leafOutOfRage -> false
        | _ -> true

    let actual = lodSelect frustumIntersector detailSelector root
    test <@ actual = [ Full leafInRange; Partial { Parent = firstMiddleNode; Child = leafOutOfRage }; Full middleNodeWithEnoughDetail ] @>