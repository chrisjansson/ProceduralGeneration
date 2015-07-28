module LodRanges

type LodSettings = {
        LodLevels : int
        LodDistanceRatio : float
        VisibilityDistance : float
    }

let calculateTotalDetailBalance lodLevels lodDistanceRatio =
    let mutable totalDetailBalance = 0.0
    let mutable currentDetailBalance = 1.0
    for i = 0 to lodLevels - 1 do
        totalDetailBalance <- totalDetailBalance + currentDetailBalance
        currentDetailBalance <- currentDetailBalance * lodDistanceRatio
    totalDetailBalance

let makeLodRanges lodSettings =
    let lodNear = 0.0
    let lodFar = lodSettings.VisibilityDistance
    let totalDetailBalance = calculateTotalDetailBalance lodSettings.LodLevels lodSettings.LodDistanceRatio
    let sect = (lodFar - lodNear) / totalDetailBalance

    let mutable previousPosition = lodNear
    let mutable currentDetailBalance = 1.0
    let lodRanges = Array.zeroCreate<float> lodSettings.LodLevels
    for i = 0 to lodSettings.LodLevels - 1 do
        let visibilityRange = previousPosition + sect * currentDetailBalance
        lodRanges.[i] <- visibilityRange
        previousPosition <- visibilityRange
        currentDetailBalance <- currentDetailBalance * lodSettings.LodDistanceRatio
    lodRanges
