module TweakBarGui
open AntTweakBar
open System.Reflection
open System.Reactive
open System.Reactive.Linq

type Color = OpenTK.Vector3

let readColor (c:ColorVariable) =
    new Color(c.R, c.G, c.B)

let makeColorVariable b (c:Color) l =
    let colorVariable = new ColorVariable(b)
    colorVariable.R <- c.X
    colorVariable.G <- c.Y
    colorVariable.B <- c.Z
    colorVariable.Label <- l
    let obs = colorVariable.Changed |> Observable.map (fun _ -> readColor colorVariable)
    obs.StartWith(readColor colorVariable)

let makeFloatVariable b d l =
    let doubleVariable = new DoubleVariable(b)
    doubleVariable.Label <- l
    doubleVariable.Value <- d
    let obs = doubleVariable.Changed |> Observable.map (fun _ -> doubleVariable.Value)
    obs.StartWith(doubleVariable.Value)
