module Math

    let clamp min max v =
        match v with
        | _ when v < min -> min
        | _ when v > max -> max
        | _ -> v
