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
let ``Tree with depth of zero contains specified bounds and has no children`` () =
    let actual = makeTree (fun _ -> []) 0 zeroBounds
    test <@ { Children = []; LodLevel = 0; Bounds = zeroBounds } = actual @>

[<Fact>]
let ``Tree with depth of one contains specified bounds and has one child because of singular bounds split`` () =
    let actual = makeTree (fun _ -> [ zeroBounds ]) 1 zeroBounds
    test <@ { Children = [ { Children = []; LodLevel = 0; Bounds = zeroBounds } ]; LodLevel = 1; Bounds = zeroBounds } = actual @>

[<Fact>]
let ``Tree with depth of one contains specified bounds and has three children because of bounds split`` () =
    let actual = makeTree (fun _ -> [ zeroBounds; zeroBounds; zeroBounds ]) 1 zeroBounds
    test <@ { 
                Children = [ 
                                { Children = []; LodLevel = 0; Bounds = zeroBounds } 
                                { Children = []; LodLevel = 0; Bounds = zeroBounds } 
                                { Children = []; LodLevel = 0; Bounds = zeroBounds } 
                            ]; 
                LodLevel = 1; 
                Bounds = zeroBounds 
            } = actual @>

[<Fact>]
let ``Splits bounds in four pieces in XZ plane``() =
    let bounds = { Min = { X = -1.0; Y = -10.0; Z = -1.0; }; Max = { X = 1.0; Y = 10.0; Z = 1.0; } }
    let actual = quadXZBoundsSplitter bounds

    let expected = 
        [
            { Min = { X = -1.0; Y = -10.0; Z = -1.0; }; Max = { X = 0.0; Y = 10.0; Z = 0.0; } }
            { Min = { X = -1.0; Y = -10.0; Z = 0.0; }; Max = { X = 0.0; Y = 10.0; Z = 1.0; } }
            { Min = { X = 0.0; Y = -10.0; Z = -1.0; }; Max = { X = 1.0; Y = 10.0; Z = 0.0; } }
            { Min = { X = 0.0; Y = -10.0; Z = 0.0; }; Max = { X = 1.0; Y = 10.0; Z = 1.0; } }
        ]

    test <@ expected = actual @>

[<Fact>]
let ``QuadTree example`` () =
    let bounds = { Min = { X = -1.0; Y = -10.0; Z = -1.0; }; Max = { X = 1.0; Y = 10.0; Z = 1.0; } }
    
    let expected = {
            Children = 
                [
                    { 
                        Children = []
                        LodLevel = 0
                        Bounds = { Min = { X = -1.0; Y = -10.0; Z = -1.0; }; Max = { X = 0.0; Y = 10.0; Z = 0.0; } }
                    }
                    { 
                        Children = []
                        LodLevel = 0
                        Bounds = { Min = { X = -1.0; Y = -10.0; Z = 0.0; }; Max = { X = 0.0; Y = 10.0; Z = 1.0; } }
                    }
                    { 
                        Children = []
                        LodLevel = 0
                        Bounds = { Min = { X = 0.0; Y = -10.0; Z = -1.0; }; Max = { X = 1.0; Y = 10.0; Z = 0.0; } }
                    }
                    { 
                        Children = []
                        LodLevel = 0
                        Bounds = { Min = { X = 0.0; Y = -10.0; Z = 0.0; }; Max = { X = 1.0; Y = 10.0; Z = 1.0; } }
                    }
                ]
            LodLevel = 1
            Bounds = bounds
        }

    let actual = makeXZQuadTree 1 bounds
    test <@ expected = actual @>
