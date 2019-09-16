module GlslParser.ParserLibrary

(*
understanding_parser_combinators-3.fsx

Adding more informative errors to the parser library.

Related blog post: http://fsharpforfunandprofit.com/posts/understanding-parser-combinators-3/
*)


// =============================================
// Section 1 -- Labeled parsers
// =============================================


(*
// The problem:

let parseDigit = anyOf ['0'..'9']
run parseDigit "|ABC"  // Failure "Expecting '9'. Got '|'"

// We want error to say: Failure "Expecting digit. Got '|'"
*)

//
//module LabelledParser =
//
//    open System
//
//    // Aliases 
//    type ParserLabel = string
//    type ParserError = string
//
//    // Result type
//    type Result<'a> =
//        | Success of 'a
//        | Failure of ParserLabel * ParserError 
//
//    (*
//    // before
//    type Parser<'a> = Parser of (string -> Result<'a * string>)
//    *)
//
//    /// A Parser structure has a parsing function & label
//    type Parser<'a,'input> = {
//        parseFn : ('input -> Result<'a * 'input>)
//        label:  ParserLabel 
//        }
//
//    let printResult result =
//        match result with
//        | Success (value,input) -> 
//            printfn "%A" value
//        | Failure (label,error) -> 
//            printfn "Error parsing %s\n%s" label error
//
//    /// parse a char 
//    let pchar charToMatch = 
//        let label = sprintf "%c" charToMatch 
//        let innerFn input =
//            if String.IsNullOrEmpty(input) then
//                Failure (label,"No more input")
//            else
//                let first = input.[0] 
//                if first = charToMatch then
//                    let remainingInput = input.[1..]
//                    Success (charToMatch,remainingInput)
//                else
//                    let err = sprintf "Unexpected '%c'" first
//                    Failure (label,err)
//        // return the parser
//        {parseFn=innerFn;label=label}
//
//    /// Run a parser with some input
//    let run parser input = 
//        // get inner function
//        let innerFn = parser.parseFn 
//        // call inner function with input
//        innerFn input
//
//    /// get the label from a parser
//    let getLabel parser = 
//        // get label
//        parser.label
//
//    /// update the label in the parser
//    let setLabel parser newLabel = 
//        // change the inner function to use the new label
//        let newInnerFn input = 
//            let result = parser.parseFn input
//            match result with
//            | Success s ->
//                // if Success, do nothing
//                Success s 
//            | Failure (oldLabel,err) -> 
//                // if Failure, return new label
//                Failure (newLabel,err)         // <====== use newLabel here
//        // return the Parser
//        {parseFn=newInnerFn; label=newLabel}   // <====== use newLabel here 
//
//    /// infix version of setLabel
//    let ( <?> ) = setLabel
//
//
//    /// "bindP" takes a parser-producing function f, and a parser p
//    /// and passes the output of p into f, to create a new parser
//    let bindP f p =
//        let label = "unknown"                     // <====== "label" is new! 
//        let innerFn input =
//            let result1 = run p input 
//            match result1 with
//            | Failure (label,err) ->              // <====== "label" is new! 
//                // return error from parser1
//                Failure (label,err)  
//            | Success (value1,remainingInput) ->
//                // apply f to get a new parser
//                let p2 = f value1
//                // run parser with remaining input
//                run p2 remainingInput
//        {parseFn=innerFn; label=label}            // <====== "parseFn" and "label" are new!
//
//    /// Infix version of bindP
//    let ( >>= ) p f = bindP f p
//
//    /// Lift a value to a Parser
//    let returnP x = 
//        let label = sprintf "%A" x
//        let innerFn input =
//            // ignore the input and return x
//            Success (x,input)
//        // return the inner function
//        {parseFn=innerFn; label=label}
//
//    /// apply a function to the value inside a parser
//    let mapP f = 
//        bindP (f >> returnP)
//
//    /// infix version of mapP
//    let ( <!> ) = mapP
//
//    /// "piping" version of mapP
//    let ( |>> ) x f = mapP f x
//
//    /// apply a wrapped function to a wrapped value
//    let applyP fP xP =         
//        fP >>= (fun f -> 
//        xP >>= (fun x -> 
//            returnP (f x) ))
//
//    /// infix version of apply
//    let ( <*> ) = applyP
//
//    /// lift a two parameter function to Parser World
//    let lift2 f xP yP =
//        returnP f <*> xP <*> yP
//
//    /// Combine two parsers as "A andThen B"
//    let andThen p1 p2 =         
//        let label = sprintf "%s andThen %s" (getLabel p1) (getLabel p2)
//        p1 >>= (fun p1Result -> 
//        p2 >>= (fun p2Result -> 
//            returnP (p1Result,p2Result) ))
//        <?> label                       // <====== provide a custom label
//
//    /// Infix version of andThen
//    let ( .>>. ) = andThen
//
//    // combine two parsers as "A orElse B"
//    let orElse parser1 parser2 =
//        // construct a new label
//        let label = sprintf "%s orElse %s" (getLabel parser1) (getLabel parser2)
//
//        let innerFn input =
//            // run parser1 with the input
//            let result1 = run parser1 input
//
//            // test the result for Failure/Success
//            match result1 with
//            | Success result -> 
//                // if success, return the result
//                result1
//
//            | Failure (_,err) -> 
//                // if failed, run parser2 with the input
//                let result2 = run parser2 input
//
//                // return parser2's result
//                match result2 with
//                | Success _ -> 
//                    // if success, return the result
//                    result2
//
//                | Failure (_,err) -> 
//                    // if failed, return the error with overall label
//                    Failure (label,err)
//
//        // return the Parser
//        {parseFn=innerFn; label=label}
//
//    // infix version of orElse
//    let ( <|> ) = orElse
//
//    /// choose any of a list of parsers
//    let choice listOfParsers = 
//        List.reduce ( <|> ) listOfParsers 
//
//    /// choose any of a list of characters
//    let anyOf listOfChars = 
//        let label = sprintf "any of %A" listOfChars 
//        listOfChars
//        |> List.map pchar // convert into parsers
//        |> choice
//        <?> label                           // <====== provide a custom label
//
//    module Label_Test =
//
//        let parseAB = 
//            pchar 'A' .>>. pchar 'B' 
//        
//        run parseAB "A|C"  
//        |> printResult
//        // Error parsing A andThen B
//        // Unexpected '|'
//
//        let parseAB_WithLabel = 
//            pchar 'A' .>>. pchar 'B' 
//            <?> "AB"
//
//        run parseAB_WithLabel  "A|C"  
//        |> printResult
//        // Error parsing AB
//        // Unexpected '|'
//
//        let parseDigit = 
//            anyOf ['0'..'9']
//        
//        run parseDigit "|ABC"  
//        |> printResult
//        // Error parsing any of ['0'; '1'; '2'; '3'; '4'; '5'; '6'; '7'; '8'; '9']
//        // Unexpected '|'
//
//        let parseDigit_WithLabel = 
//            anyOf ['0'..'9'] 
//            <?> "digit"
//
//        run parseDigit_WithLabel "|ABC"  
//        |> printResult
//        // Error parsing digit
//        // Unexpected '|'
//
//        type Keyword = IF | FOR
//
//        let keyword_IF = 
//            pchar 'i' .>>. pchar 'f' |>> (fun _ -> IF)
//        let keyword_FOR = 
//            pchar 'f' .>>. pchar 'o' .>>. pchar 'r' |>> (fun _ -> FOR)
//        let keyword = 
//            choice [keyword_IF; keyword_FOR]
//            <?> "keyword"
//
//        run keyword "if x then"  
//        |> printResult
//        // IF
//
//        run keyword "while x then"  
//        |> printResult
//        // Error parsing keyword
//        // Unexpected 'w'
//
//// =============================================
//// Section 2 - replacing "pchar" with "satisfy"
//// =============================================
//
//module ReplacePcharWithSatisfy =
//    open System
//    open LabelledParser 
//
//
//    /// Match an input token if the predicate is satisfied
//    let satisfy predicate label =
//        let innerFn input =
//            if String.IsNullOrEmpty(input) then
//                Failure (label,"No more input")
//            else
//                let first = input.[0] 
//                if predicate first then       // <====== use predicate here
//                    let remainingInput = input.[1..]
//                    Success (first,remainingInput)
//                else
//                    let err = sprintf "Unexpected '%c'" first
//                    Failure (label,err)
//        // return the parser
//        {parseFn=innerFn;label=label}
//
//    /// parse a char 
//    let pchar charToMatch = 
//        let predicate ch = (ch = charToMatch) 
//        let label = sprintf "%c" charToMatch 
//        satisfy predicate label 
//
//    /// parse a digit
//    let digitChar = 
//        let predicate = Char.IsDigit 
//        let label = "digit"
//        satisfy predicate label 
//
//    /// parse a whitespace char
//    let whitespaceChar = 
//        let predicate = Char.IsWhiteSpace 
//        let label = "whitespace"
//        satisfy predicate label 
//
//// =============================================
//// Section 3. Adding positional context to error messages
//// =============================================
//
//
//module TextInput =
//    open System
//
//    type Position = {
//        line : int
//        column : int
//    }
//    
//    /// define an initial position
//    let initialPos = {line=0; column=0}
//
//    /// increment the column number
//    let incrCol pos = 
//        {pos with column=pos.column + 1}
//
//    /// increment the line number and set the column to 0
//    let incrLine pos = 
//        {line=pos.line + 1; column=0}
//
//    /// Define the current input state
//    type InputState = {
//        lines : string[]
//        position : Position 
//    }
//
//    // return the current line
//    let currentLine inputState = 
//        let linePos = inputState.position.line
//        if linePos < inputState.lines.Length then
//            inputState.lines.[linePos]
//        else
//            "end of file"
//
//    /// Create a new InputState from a string
//    let fromStr str = 
//        if String.IsNullOrEmpty(str) then
//            {lines=[||]; position=initialPos}
//        else
//            let separators = [| "\r\n"; "\n" |]
//            let lines = str.Split(separators, StringSplitOptions.None)
//            {lines=lines; position=initialPos}
//
//
//    /// Get the next character from the input, if any
//    /// else return None. Also return the updated InputState
//    /// Signature: InputState -> InputState * char option 
//    let nextChar input =
//        let linePos = input.position.line
//        let colPos = input.position.column
//        // three cases
//        // 1) if line >= maxLine -> 
//        //       return EOF
//        // 2) if col less than line length -> 
//        //       return char at colPos, increment colPos
//        // 3) if col at line length -> 
//        //       return NewLine, increment linePos
//
//        if linePos >= input.lines.Length then
//            input, None
//        else
//            let currentLine = currentLine input
//            if colPos < currentLine.Length then
//                let char = currentLine.[colPos]
//                let newPos = incrCol input.position 
//                let newState = {input with position=newPos}
//                newState, Some char
//            else 
//                // end of line, so return LF and move to next line
//                let char = '\n'
//                let newPos = incrLine input.position 
//                let newState = {input with position=newPos}
//                newState, Some char
//
//    // ---------  
//    // test 
//    // --------- 
//    module Input_Test =
//        let rec readAllChars input =
//            [
//                let remainingInput,charOpt = nextChar input 
//                match charOpt with
//                | None -> 
//                    // end of input
//                    ()
//                | Some ch -> 
//                    // return first character
//                    yield ch
//                    // return the remaining characters
//                    yield! readAllChars remainingInput
//            ]
//
//        fromStr "" |> readAllChars       // []
//        fromStr "a" |> readAllChars      // ['a'; '\n']
//        fromStr "ab" |> readAllChars     // ['a'; 'b'; '\n']
//        fromStr "a\nb" |> readAllChars   // ['a'; '\n'; 'b'; '\n']

module ParserWithPositionalErrors =

    open System

    // Aliases for input, etc
   
    type Position = {
        token : int
    }
    
    type InputState<'T> = {
        lines : 'T list
        position : Position 
    }
    
    type Input<'T> = InputState<'T>   // type alias
   
    
    type ParserLabel = string
    type ParserError = string

    /// Stores information about the parser position for error messages
    type ParserPosition<'T> = {
        CurrentToken : 'T
        }

    // Result type
    type Result<'a, 'input> =
        | Success of 'a
        | Failure of ParserLabel * ParserError * ParserPosition<'input> 

    /// A Parser structure has a parsing function & label
    type Parser<'a, 'input> = {
        parseFn : (Input<'input> -> Result<'a * Input<'input>, 'input>)
        label:  ParserLabel 
        }

    type RefParser<'a, 'input> =
        {
            Parser: Parser<'a, 'input>
            Set: (Input<'input> -> Result<'a * Input<'input>, 'input>) -> unit
        }
    
    let createRefParser (): RefParser<_, _> =
        let mutable parseFn =
            fun _ -> failwith "Initialize parser ref first"
        
        {
            Parser =
                {
                    parseFn = fun input -> parseFn input
                    label = "Parser ref"
                }
            Set = fun p -> parseFn <- p
        }
    
    let parserPositionFromInputState (inputState:Input<_>) = {
        CurrentToken = inputState.lines.[inputState.position.token]
        }
        
    let printResult result =
        match result with
        | Success (value,input) -> 
            printfn "%A" value
        | Failure (label,error,parserPos) -> 
//            let errorLine = parserPos.currentLine
//            let colPos = parserPos.column
//            let linePos = parserPos.line
//            let failureCaret = sprintf "%*s^%s" colPos "" error
            // examples of formatting
            //   sprintf "%*s^%s" 0 "" "test"
            //   sprintf "%*s^%s" 10 "" "test"
//            printfn "Line:%i Col:%i Error parsing %s\n%s\n%s" linePos colPos label errorLine failureCaret 
            printfn "Token: %i %s" parserPos.CurrentToken label

    (*
    let exampleError = 
        Failure ("identifier", "unexpected |",
                 {currentLine = "123 ab|cd"; line=1; column=6})
    printResult exampleError 
    // Line:1 Col:6 Error parsing identifier
    // 123 ab|cd
    //       ^unexpected |
    *)

    /// Run the parser on a InputState
    let runOnInput parser input = 
        // call inner function with input
        parser.parseFn input

    /// get the label from a parser
    let getLabel parser = 
        // get label
        parser.label

    /// update the label in the parser
    let setLabel parser newLabel = 
        // change the inner function to use the new label
        let newInnerFn input = 
            let result = parser.parseFn input
            match result with
            | Success s ->
                // if Success, do nothing
                Success s 
            | Failure (oldLabel,err,pos) -> 
                // if Failure, return new label
                Failure (newLabel,err,pos) 
        // return the Parser
        {parseFn=newInnerFn; label=newLabel}

    /// infix version of setLabel
    let ( <?> ) = setLabel


    /// Match an input token if the predicate is satisfied
    let satisfy predicate label =
        let innerFn (input: InputState<_>) =
            let lines =
                List.skip input.position.token input.lines
            
            match lines with
            | [] -> 
                let err = "No more input"
                let pos = parserPositionFromInputState input
                //Failure (label,err)     // <====== old version
                Failure (label,err,pos)   // <====== new version
            | head::tail ->
                if predicate head then
                    let remainingInput = { input with position = { input.position with token = input.position.token + 1 } }
                    Success (head,remainingInput)
                else
                    let err = sprintf "Unexpected '%A'" head
                    let pos = parserPositionFromInputState input
                    //Failure (label,err)     // <====== old version
                    Failure (label,err,pos)   // <====== new version
        // return the parser
        {parseFn=innerFn;label=label}

    /// "bindP" takes a parser-producing function f, and a parser p
    /// and passes the output of p into f, to create a new parser
    let bindP f p =
        let label = "unknown"
        let innerFn input =
            let result1 = runOnInput p input 
            match result1 with
            | Failure (label,err,pos) ->       // <====== new with pos
                // return error from parser1
                Failure (label,err,pos)  
            | Success (value1,remainingInput) ->
                // apply f to get a new parser
                let p2 = f value1
                // run parser with remaining input
                runOnInput p2 remainingInput
        {parseFn=innerFn; label=label}

    /// Infix version of bindP
    let ( >>= ) p f = bindP f p

    /// Lift a value to a Parser
    let returnP x = 
        let label = sprintf "%A" x
        let innerFn input =
            // ignore the input and return x
            Success (x,input)
        // return the inner function
        {parseFn=innerFn; label=label}

    /// apply a function to the value inside a parser
    let mapP f = 
        bindP (f >> returnP)

    /// infix version of mapP
    let ( <!> ) = mapP

    /// "piping" version of mapP
    let ( |>> ) x f = mapP f x

    /// apply a wrapped function to a wrapped value
    let applyP fP xP =         
        fP >>= (fun f -> 
        xP >>= (fun x -> 
            returnP (f x) ))

    /// infix version of apply
    let ( <*> ) = applyP

    /// lift a two parameter function to Parser World
    let lift2 f xP yP =
        returnP f <*> xP <*> yP

    /// Combine two parsers as "A andThen B"
    let andThen p1 p2 =         
        let label = sprintf "%s andThen %s" (getLabel p1) (getLabel p2)
        p1 >>= (fun p1Result -> 
        p2 >>= (fun p2Result -> 
            returnP (p1Result,p2Result) ))
        <?> label

    /// Infix version of andThen
    let ( .>>. ) = andThen

    /// Combine two parsers as "A orElse B"
    let orElse p1 p2 =
        let label = sprintf "%s orElse %s" (getLabel p1) (getLabel p2)
        let innerFn input =
            // run parser1 with the input
            let result1 = runOnInput p1 input

            // test the result for Failure/Success
            match result1 with
            | Success result -> 
                // if success, return the original result
                result1

            | Failure _ -> 
                // if failed, run parser2 with the input
                let result2 = runOnInput p2 input

                // return parser2's result
                result2 

        // return the inner function
        {parseFn=innerFn; label=label}

    /// Infix version of orElse
    let ( <|> ) = orElse

    /// Choose any of a list of parsers
    let choice listOfParsers = 
        List.reduce ( <|> ) listOfParsers 

    let rec sequence parserList =
        // define the "cons" function, which is a two parameter function
        let cons head tail = head::tail

        // lift it to Parser World
        let consP = lift2 cons

        // process the list of parsers recursively
        match parserList with
        | [] -> 
            returnP []
        | head::tail ->
            consP head (sequence tail)

    /// (helper) match zero or more occurences of the specified parser
    let rec parseZeroOrMore parser input =
        // run parser with the input
        let firstResult = runOnInput parser input 
        // test the result for Failure/Success
        match firstResult with
        | Failure (_,_,_) -> 
            // if parse fails, return empty list
            ([],input)  
        | Success (firstValue,inputAfterFirstParse) -> 
            // if parse succeeds, call recursively
            // to get the subsequent values
            let (subsequentValues,remainingInput) = 
                parseZeroOrMore parser inputAfterFirstParse
            let values = firstValue::subsequentValues
            (values,remainingInput)  

    /// matches zero or more occurences of the specified parser
    let many parser = 
        let label = sprintf "many %s" (getLabel parser)
        let rec innerFn input =
            // parse the input -- wrap in Success as it always succeeds
            Success (parseZeroOrMore parser input)
        {parseFn=innerFn; label=label}

    /// matches one or more occurences of the specified parser
    let many1 p =         
        let label = sprintf "many1 %s" (getLabel p)

        p      >>= (fun head -> 
        many p >>= (fun tail -> 
            returnP (head::tail) ))
        <?> label

    /// Parses an optional occurrence of p and returns an option value.
    let opt p = 
        let label = sprintf "opt %s" (getLabel p)
        let some = p |>> Some
        let none = returnP None
        (some <|> none) <?> label

    /// Keep only the result of the left side parser
    let (.>>) p1 p2 = 
        // create a pair
        p1 .>>. p2 
        // then only keep the first value
        |> mapP (fun (a,b) -> a) 

    /// Keep only the result of the right side parser
    let (>>.) p1 p2 = 
        // create a pair
        p1 .>>. p2 
        // then only keep the second value
        |> mapP (fun (a,b) -> b) 

    /// Keep only the result of the middle parser
    let between p1 p2 p3 = 
        p1 >>. p2 .>> p3 

    /// Parses one or more occurrences of p separated by sep
    let sepBy1 p sep =
        let sepThenP = sep >>. p            
        p .>>. many sepThenP 
        |>> fun (p,pList) -> p::pList

    /// Parses zero or more occurrences of p separated by sep
    let sepBy p sep =
        sepBy1 p sep <|> returnP []
//
//
//    module PositionalError_Test =
//
//        /// parse a char 
//        let pchar charToMatch = 
//            let predicate ch = (ch = charToMatch) 
//            let label = sprintf "%c" charToMatch 
//            satisfy predicate label 
//
//        let parseAB = 
//            pchar 'A' .>>. pchar 'B' 
//            <?> "AB"
//
//        run parseAB "A|C"  
//        |> printResult
//        // Line:0 Col:1 Error parsing AB
//        // A|C
//        //  ^Unexpected '|'
//
//
//
//// =============================================
//// Section 4. Adding some standard parsers to the library
//// =============================================
//
//module StandardParsers =
//    open System
//    open ParserWithPositionalErrors
//
//    // ------------------------------
//    // char and string parsing
//    // ------------------------------
//            
//    /// parse a char 
//    let pchar charToMatch = 
//        // label is just the character
//        let label = sprintf "%c" charToMatch 
//
//        let predicate ch = (ch = charToMatch) 
//        satisfy predicate label 
//
//    /// Choose any of a list of characters
//    let anyOf listOfChars = 
//        let label = sprintf "anyOf %A" listOfChars 
//        listOfChars
//        |> List.map pchar // convert into parsers
//        |> choice
//        <?> label
//
//    /// Convert a list of chars to a string
//    let charListToStr charList =
//        String(List.toArray charList) 
//
//    /// Parses a sequence of zero or more chars with the char parser cp. 
//    /// It returns the parsed chars as a string.
//    let manyChars cp =
//        many cp
//        |>> charListToStr
//
//    /// Parses a sequence of one or more chars with the char parser cp. 
//    /// It returns the parsed chars as a string.
//    let manyChars1 cp =
//        many1 cp
//        |>> charListToStr
//
//    /// parse a specific string
//    let pstring str = 
//        // label is just the string
//        let label = str 
//
//        str
//        // convert to list of char
//        |> List.ofSeq
//        // map each char to a pchar
//        |> List.map pchar 
//        // convert to Parser<char list>
//        |> sequence
//        // convert Parser<char list> to Parser<string>
//        |> mapP charListToStr 
//        <?> label
//
//    module StringParsers_Test =
//
//        run (pstring "AB") "ABC"  
//        |> printResult   
//        // Success
//        // "AB"
//
//        run (pstring "AB") "A|C"  
//        |> printResult
//        // Line:0 Col:1 Error parsing AB
//        // A|C
//        //  ^Unexpected '|'
//
//
//
//    // ------------------------------
//    // whitespace parsing
//    // ------------------------------
//
//    /// parse a whitespace char
//    let whitespaceChar = 
//        let predicate = Char.IsWhiteSpace 
//        let label = "whitespace"
//        satisfy predicate label 
//
//    /// parse zero or more whitespace char
//    let spaces = many whitespaceChar
//
//    /// parse one or more whitespace char
//    let spaces1 = many1 whitespaceChar
//
//    module WhitespaceParsers_Test =
//
//        run spaces " ABC"  
//        |> printResult   
//        // [' ']
//
//        run spaces "A"  
//        |> printResult
//        // []
//
//        run spaces1 " ABC"  
//        |> printResult   
//        // [' ']
//
//        run spaces1 "A"  
//        |> printResult
//        // Line:0 Col:0 Error parsing many1 whitespace
//        // A
//        // ^Unexpected 'A'
//
//
//    // ------------------------------
//    // number parsing
//    // ------------------------------
//
//    /// parse a digit
//    let digitChar = 
//        let predicate = Char.IsDigit 
//        let label = "digit"
//        satisfy predicate label 
//
//
//    // parse an integer
//    let pint = 
//        let label = "integer" 
//
//        // helper
//        let resultToInt (sign,digits) = 
//            let i = digits |> int  // ignore int overflow for now
//            match sign with
//            | Some ch -> -i  // negate the int
//            | None -> i
//            
//        // define parser for one or more digits
//        let digits = manyChars1 digitChar 
//
//        // an "int" is optional sign + one or more digits
//        opt (pchar '-') .>>. digits 
//        |> mapP resultToInt
//        <?> label
//
//    // parse a float
//    let pfloat = 
//        let label = "float" 
//
//        // helper
//        let resultToFloat (((sign,digits1),point),digits2) = 
//            let fl = sprintf "%s.%s" digits1 digits2 |> float
//            match sign with
//            | Some ch -> -fl  // negate the float
//            | None -> fl
//            
//        // define parser for one or more digits 
//        let digits = manyChars1 digitChar 
//
//        // a float is sign, digits, point, digits (ignore exponents for now)
//        opt (pchar '-') .>>. digits .>>. pchar '.' .>>. digits 
//        |> mapP resultToFloat
//        <?> label
//
//    module NumericParsers_Test =
//
//        run pint "-123Z" 
//        |> printResult   
//        // -123
//
//        run pint "-Z123" 
//        |> printResult
//        // Line:0 Col:1 Error parsing integer
//        // -Z123
//        //  ^Unexpected 'Z'
//
//        run pfloat "-123.45Z" 
//        |> printResult   
//        // -123.45
//
//        run pfloat "-123Z45" 
//        |> printResult
//        // Line:0 Col:4 Error parsing float
//        // -123Z45
//        //     ^Unexpected 'Z'