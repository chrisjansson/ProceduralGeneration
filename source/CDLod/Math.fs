[<AutoOpen>]
module Math

type Point = {
        X : float
        Y : float
        Z : float
    }

type Bounds = {
        Min : Point
        Max : Point
    }

type Sphere = {
        Center : Point
        Radius : float
    }

type Size = Point

let getBoundsSize bounds =
    {
        X = bounds.Max.X - bounds.Min.X
        Y = bounds.Max.Y - bounds.Min.Y
        Z = bounds.Max.Z - bounds.Min.Z
    }

let getBoundsCenter bounds =
    let min = bounds.Min
    let max = bounds.Max
    let size = getBoundsSize bounds
    {
        X = min.X + size.X / 2.0
        Y = min.Y + size.Y / 2.0
        Z = min.Z + size.Z / 2.0
    }

let aabbSphereIntersects aabb sphere =
    let mutable d = 0.0
    
    if sphere.Center.X < aabb.Min.X then
        let s = sphere.Center.X - aabb.Min.X
        d <- d + s * s    
    else if sphere.Center.X > aabb.Max.X then
        let s = sphere.Center.X - aabb.Max.X
        d <- d + s * s

    if sphere.Center.Y < aabb.Min.Y then
        let s = sphere.Center.Y - aabb.Min.Y
        d <- d + s * s    
    else if sphere.Center.Y > aabb.Max.Y then
        let s = sphere.Center.Y - aabb.Max.Y
        d <- d + s * s

    if sphere.Center.Z < aabb.Min.Z then
        let s = sphere.Center.Z - aabb.Min.Z
        d <- d + s * s    
    else if sphere.Center.Z > aabb.Max.Z then
        let s = sphere.Center.Z - aabb.Max.Z
        d <- d + s * s

    d <= sphere.Radius * sphere.Radius