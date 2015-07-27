module LodRanges

type LodSettings = {
        LodLevels : int
        LodDistanceRatio : float
    }

let calculateTotalDetailBalance lodLevels lodDistanceRatio =
    let mutable totalDetailBalance = 0.0
    let mutable currentDetailBalance = 1.0
    for i = 0 to lodLevels - 1 do
        totalDetailBalance <- totalDetailBalance + currentDetailBalance
        currentDetailBalance <- currentDetailBalance * lodDistanceRatio
    totalDetailBalance

let makeLodRanges lodSettings =
    ()