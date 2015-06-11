module Result

type Result<'T> =
    | Success of 'T
    | Failure of string


