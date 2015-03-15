module TweakBar
open AntTweakBar
open OpenTK.Input

type TweakBarKeyEvent = { Key : Tw.Key; Modifiers : Tw.KeyModifiers }

let map = Map.ofList [ 
            (Key.A, Tw.Key.A) 
            (Key.B, Tw.Key.B)
            (Key.C, Tw.Key.C)
            (Key.D, Tw.Key.D)
            (Key.E, Tw.Key.E)
            (Key.F, Tw.Key.F)
            (Key.G, Tw.Key.G)
            (Key.H, Tw.Key.H)
            (Key.I, Tw.Key.I)
            (Key.J, Tw.Key.J)
            (Key.K, Tw.Key.K)
            (Key.L, Tw.Key.L)
            (Key.M, Tw.Key.M)
            (Key.N, Tw.Key.N)
            (Key.O, Tw.Key.O)
            (Key.P, Tw.Key.P)
            (Key.Q, Tw.Key.Q)
            (Key.R, Tw.Key.R)
            (Key.S, Tw.Key.S)
            (Key.T, Tw.Key.T)
            (Key.U, Tw.Key.U)
            (Key.V, Tw.Key.V)
            (Key.W, Tw.Key.W)
            (Key.X, Tw.Key.X)
            (Key.Y, Tw.Key.Y)
            (Key.Z, Tw.Key.Z)
            (Key.BackSpace, Tw.Key.Backspace) 
            (Key.Clear, Tw.Key.Clear)
            (Key.Delete, Tw.Key.Delete)
            (Key.Down, Tw.Key.Down)
            (Key.End, Tw.Key.End)
            (Key.Escape, Tw.Key.Escape)
            (Key.Home, Tw.Key.Home)
            (Key.Insert, Tw.Key.Insert)
            (Key.Left, Tw.Key.Left)
            (Key.PageDown, Tw.Key.PageDown)
            (Key.PageUp, Tw.Key.PageUp)
            (Key.Pause, Tw.Key.Pause)
            (Key.Enter, Tw.Key.Return)
            (Key.Right, Tw.Key.Right)
            (Key.Space, Tw.Key.Space)
            (Key.Tab, Tw.Key.Tab)
            (Key.Up, Tw.Key.Up)
            (Key.F1, Tw.Key.F1)
            (Key.F2, Tw.Key.F2)
            (Key.F3, Tw.Key.F3)
            (Key.F4, Tw.Key.F4)
            (Key.F5, Tw.Key.F5)
            (Key.F6, Tw.Key.F6)
            (Key.F7, Tw.Key.F7)
            (Key.F8, Tw.Key.F8)
            (Key.F9, Tw.Key.F9)
            (Key.F10, Tw.Key.F10)
            (Key.F11, Tw.Key.F11)
            (Key.F12, Tw.Key.F12)
            (Key.F13, Tw.Key.F13)
            (Key.F14, Tw.Key.F14)
            (Key.F15, Tw.Key.F15)]

let convertModifiers (m:KeyModifiers) =
    let mutable twModifiers = Tw.KeyModifiers.None
    if m.HasFlag(KeyModifiers.Alt) then
        twModifiers <- twModifiers ||| Tw.KeyModifiers.Alt
    if m.HasFlag(KeyModifiers.Control) then
        twModifiers <- twModifiers ||| Tw.KeyModifiers.Ctrl
    if m.HasFlag(KeyModifiers.Shift) then
        twModifiers <- twModifiers ||| Tw.KeyModifiers.Shift
    twModifiers

let convertKeyEvent (e:KeyboardKeyEventArgs) =
    match map.TryFind e.Key with
    | Some twKey -> Some { Key = map.[e.Key]; Modifiers = convertModifiers e.Modifiers }
    | None -> None

