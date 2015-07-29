module LodRanges

type LodSettings = {
        LodLevels : int
        LodDistanceRatio : float //2.0
        VisibilityDistance : float
        MorphStartRatio : float //
    }

let calculateTotalDetailBalance lodLevels lodDistanceRatio =
    let mutable totalDetailBalance = 0.0
    let mutable currentDetailBalance = 1.0
    for i = 0 to lodLevels - 1 do
        totalDetailBalance <- totalDetailBalance + currentDetailBalance
        currentDetailBalance <- currentDetailBalance * lodDistanceRatio
    totalDetailBalance

type LodRange = {
        VisibilityRange : float
        MorphStart : float
        MorphEnd : float
    }

let makeLodRanges lodSettings =
    let lodNear = 0.0
    let lodFar = lodSettings.VisibilityDistance
    let totalDetailBalance = calculateTotalDetailBalance lodSettings.LodLevels lodSettings.LodDistanceRatio
    let sect = (lodFar - lodNear) / totalDetailBalance

    let mutable previousPosition = lodNear
    let mutable currentDetailBalance = 1.0
    let visibilityRanges = Array.zeroCreate<float> lodSettings.LodLevels
    for i = 0 to lodSettings.LodLevels - 1 do
        let visibilityRange = previousPosition + sect * currentDetailBalance
        visibilityRanges.[i] <- visibilityRange
        previousPosition <- visibilityRange
        currentDetailBalance <- currentDetailBalance * lodSettings.LodDistanceRatio

    let lodRanges = Array.zeroCreate<LodRange> lodSettings.LodLevels
    previousPosition <- lodNear
    for i = 0 to lodSettings.LodLevels - 1 do
        let lodRange = {
                VisibilityRange = visibilityRanges.[i]
                MorphEnd = visibilityRanges.[i]
                MorphStart = previousPosition + (visibilityRanges.[i] - previousPosition) * lodSettings.MorphStartRatio
            }
        lodRanges.[i] <- lodRange
        previousPosition <- lodRange.MorphStart
    lodRanges
//
//      selectionObj->m_morphEnd[i] = selectionObj->m_visibilityRanges[index];
//      selectionObj->m_morphStart[i] = prevPos + (selectionObj->m_morphEnd[i] - prevPos) * selectionObj->m_morphStartRatio;
//
//      prevPos = selectionObj->m_morphStart[i];