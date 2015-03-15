module TweakBarGuiViewModel
open TweakBarGui
open BlinnMaterialTweakBar
open System.Reactive.Linq

type ViewModel = {
        IntegrationSpeed : float
        BlinnMaterial : BlinnMaterial
    }

let makeViewModel b d =
    let blinnObservable = makeBlinnMaterialView b d.BlinnMaterial
    let integrationSpeedObservable = makeFloatVariable b d.IntegrationSpeed "Integration Speed"
    Observable.CombineLatest(blinnObservable, integrationSpeedObservable, fun bm is -> { IntegrationSpeed = is; BlinnMaterial = bm})
