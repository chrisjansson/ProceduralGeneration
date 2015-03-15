module BlinnMaterialTweakBar
open TweakBarGui
open System.Reactive.Linq

type BlinnMaterial = { 
    AmbientColor : Color
    DiffuseColor : Color 
    SpecularColor : Color
    SpecularExp : float }

let makeBlinnMaterialView b d =
    let ambientObservable = makeColorVariable b d.AmbientColor "Ambient"
    let diffuseObservable = makeColorVariable b d.DiffuseColor "Diffuse"
    let specularObservable = makeColorVariable b d.SpecularColor "Specular"
    let specularExpObservable = makeFloatVariable b d.SpecularExp "Specular Exponent"
    Observable.CombineLatest(
        ambientObservable, 
        diffuseObservable, 
        specularObservable,
        specularExpObservable,
        (fun a d s se-> { AmbientColor = a; DiffuseColor = d; SpecularColor = s; SpecularExp = se}))
