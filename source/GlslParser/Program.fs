// Learn more about F# at http://fsharp.org

open FParsec
open System

module Parser =
    open FParsec
    type AST =
        | TranslationUnit of ExternalDeclaration list
        | Ignore
//        | Declaration of Declaration
        | Token of Token
    and ExternalDeclaration =
        | FunctionDefinition
        | Declaration of Declaration
        | SemiColon
    and Token =
        | Comma
        | Colon
        | Equal
        | SemiColon
        | Bang
        | Dash
        | Tilde
        | Plus
        | Star
        | Slash
        | Percent
        | Keyword of Keyword
        | LParens
        | RParens 
    and Keyword =
        | Const
        | Uniform
        | Buffer
        | Shared
        | Attribute
        | Varying
        | Coherent
        | Volatile
        | Restrict
        | Readonly
        | Writeonly
        | Atomic_uint
        | Layout
        | Centroid
        | Flat
        | Smooth
        | Noperspective
        | Patch
        | Sample
        | Invariant
        | Precise
        | Break
        | Continue
        | Do
        | For
        | While
        | Switch
        | Case
        | Default
        | If
        | Else
        | Subroutine
        | In
        | Out
        | InOut
        | Int
        | Void
        | Bool
        | True
        | False
        | Float
        | Double
        | Discard
        | Return
        | Vec2
        | Vec3
        | Vec4
        | Ivec2
        | Ivec3
        | Ivec4
        | Bvec2
        | Bvec3
        | Bvec4
        | Uint
        | Uvec2
        | Uvec3
        | Uvec4
        | Dvec2
        | Dvec3
        | Dvec4
        | Mat2
        | Mat3
        | Mat4
        | Mat2x2
        | Mat2x3
        | Mat2x4
        | Mat3x2
        | Mat3x3
        | Mat3x4
        | Mat4x2
        | Mat4x3
        | Mat4x4
        | Dmat2
        | Dmat3
        | Dmat4
        | Dmat2x2
        | Dmat2x3
        | Dmat2x4
        | Dmat3x2
        | Dmat3x3
        | Dmat3x4
        | Dmat4x2
        | Dmat4x3
        | Dmat4x4
        | Lowp
        | Mediump
        | Highp
        | Precision
        | Sampler1D
        | Sampler1DShadow
        | Sampler1DArray
        | Sampler1DArrayShadow
        | Isampler1D
        | Isampler1DArray
        | Usampler1D
        | Usampler1DArray
        | Sampler2D
        | Sampler2DShadow
        | Sampler2DArray
        | Sampler2DArrayShadow
        | Isampler2D
        | Isampler2DArray
        | Usampler2D
        | Usampler2DArray
        | Sampler2DRect
        | Sampler2DRectShadow
        | Isampler2DRect
        | Usampler2DRect
        | Sampler2DMS
        | Isampler2DMS
        | Usampler2DMS
        | Sampler2DMSArray
        | Isampler2DMSArray
        | Usampler2DMSArray
        | Sampler3D
        | Isampler3D
        | Usampler3D
        | SamplerCube
        | SamplerCubeShadow
        | IsamplerCube
        | UsamplerCube
        | SamplerCubeArray
        | SamplerCubeArrayShadow
        | IsamplerCubeArray
        | UsamplerCubeArray
        | SamplerBuffer
        | IsamplerBuffer
        | UsamplerBuffer
        | Image1D
        | Iimage1D
        | Uimage1D
        | Image1DArray
        | Iimage1DArray
        | Uimage1DArray
        | Image2D
        | Iimage2D
        | Uimage2D
        | Image2DArray
        | Iimage2DArray
        | Uimage2DArray
        | Image2DRect
        | Iimage2DRect
        | Uimage2DRect
        | Image2DMS
        | Iimage2DMS
        | Uimage2DMS
        | Image2DMSArray
        | Iimage2DMSArray
        | Uimage2DMSArray
        | Image3D
        | Iimage3D
        | Uimage3D
        | ImageCube
        | IimageCube
        | UimageCube
        | ImageCubeArray
        | IimageCubeArray
        | UimageCubeArray
        | ImageBuffer
        | IimageBuffer
        | UimageBuffer
        | Struct

    and Declaration =
        | FunctionPrototype
        | InitDeclaratorList
        | PrecisionThing 
        
    module TokensParsers =
        let comma _ = pchar ',' >>. preturn (Token Comma)
        let colon _ = pchar ':' >>. preturn (Token Colon)
        let equal _ = pchar '=' >>. preturn (Token Equal)
        let bang _ = pchar '!' >>. preturn (Token Bang)
        let dash _ = pchar '-' >>. preturn (Token Dash)
        let tilde _ = pchar '~' >>. preturn (Token Tilde)
        let plus _ = pchar '+' >>. preturn (Token Plus)
        let star _ = pchar '*' >>. preturn (Token Star)
        let slash _ = pchar '/' >>. preturn (Token Slash)
        let percent _ = pchar '%' >>. preturn (Token Percent)
        let semiColon = pchar ';' >>. preturn (Token SemiColon)
        let lParens = pchar '(' >>. preturn (Token LParens)
        let rParens = pchar ')' >>. preturn (Token RParens)
        
        module Keyword =
            let private keywordP str keyword = pstring str >>. preturn (keyword |> Keyword |> Token)
            
            let _const = keywordP "const" Const
            let _in = keywordP "in" In
            let _out = keywordP "out" Out
            let _inout = keywordP "inout" InOut
            let _centroid = keywordP "centroid" Centroid
            let _patch = keywordP "patch" Patch
            let _sample = keywordP "sample" Sample
            let _uniform = keywordP "uniform" Uniform
            let _buffer = keywordP "buffer" Buffer
            let _shared = keywordP "shared" Shared
            let _coherent = keywordP "coherent" Coherent
            let _volatile = keywordP "volatile" Volatile
            let _restrict = keywordP "restrict" Restrict
            let _readonly = keywordP "readonly" Readonly
            let _writeonly = keywordP "writeonly" Writeonly
            let _subroutine = keywordP "subroutine" Subroutine
                
    let typeNameList endP = many1Till anyChar endP
    
    module StorageQualifier =
        let storageQualifier =
            TokensParsers.Keyword._const
            <|> TokensParsers.Keyword._in
            <|> TokensParsers.Keyword._out
            <|> TokensParsers.Keyword._inout
            <|> TokensParsers.Keyword._centroid
            <|> TokensParsers.Keyword._patch
            <|> TokensParsers.Keyword._sample
            <|> TokensParsers.Keyword._uniform
            <|> TokensParsers.Keyword._buffer
            <|> TokensParsers.Keyword._shared
            <|> TokensParsers.Keyword._coherent
            <|> TokensParsers.Keyword._volatile
            <|> TokensParsers.Keyword._restrict
            <|> TokensParsers.Keyword._readonly
            <|> TokensParsers.Keyword._writeonly
            <|> TokensParsers.Keyword._subroutine
            <|> (TokensParsers.Keyword._subroutine .>> TokensParsers.lParens .>> (typeNameList TokensParsers.rParens) .>> TokensParsers.rParens)
    
    let functionDefinition = preturn FunctionDefinition //TODO: To parse uniforms this can probably be skipped
    
    module Declaration =
        let functionPrototype = preturn (FunctionPrototype) .>> TokensParsers.semiColon
    
        let initDeclaratorList = preturn (InitDeclaratorList) .>> TokensParsers.semiColon
    
    let declaration =
        Declaration.functionPrototype
            
    
    let externalDeclaration =
        functionDefinition
            <|> (declaration |>> ExternalDeclaration.Declaration)
            <|> (TokensParsers.semiColon >>. preturn ExternalDeclaration.SemiColon)
    
    let translationUnit = many1 externalDeclaration |>> (fun ed -> TranslationUnit ed)

[<EntryPoint>]
let main argv =
    try 
        CharParsers.run Parser.translationUnit "" |> ignore
    with _ ->
        ()
    CharParsers.run Parser.StorageQualifier.storageQualifier "uniform" |> printfn "%A"
    
    printfn "Hello World from F#!"
    0 // return an integer exit code
