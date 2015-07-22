module QuadTree

open LodSelect

let makeQuadTree d bounds =
    { Children = []; LodLevel = 0; Bounds = bounds }