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
        | LBracket
        | RBracket
    and TypeQualifier = SingleTypeQualifier list
    and SingleTypeQualifier =
        | StorageQualifier of Token //TODO: Can be narrowed to actual storage qualifiers
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
        let lBracket = pchar '[' >>. preturn (Token LBracket)
        let rBracket = pchar ']' >>. preturn (Token RBRacket)
        
        module Keyword =
            let private keywordP str keyword = pstring str >>. preturn (keyword |> Keyword)
            
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

            let _attribute = keywordP "attribute" Attribute
            let _varying = keywordP "varying" Varying
            let _atomic_uint = keywordP "atomic_uint" Atomic_uint
            let _layout = keywordP "layout" Layout
            let _flat = keywordP "flat" Flat
            let _smooth = keywordP "smooth" Smooth
            let _noperspective = keywordP "noperspective" Noperspective
            let _invariant = keywordP "invariant" Invariant
            let _precise = keywordP "precise" Precise
            let _break = keywordP "break" Break
            let _continue = keywordP "continue" Continue
            let _do = keywordP "do" Do
            let _for = keywordP "for" For
            let _while = keywordP "while" While
            let _switch = keywordP "switch" Switch
            let _case = keywordP "case" Case
            let _default = keywordP "default" Default
            let _if = keywordP "if" If
            let _else = keywordP "else" Else
            let _int = keywordP "int" Int
            let _void = keywordP "void" Void
            let _bool = keywordP "bool" Bool
            let _true = keywordP "true" True
            let _false = keywordP "false" False
            let _float = keywordP "float" Float
            let _double = keywordP "double" Double
            let _discard = keywordP "discard" Discard
            let _return = keywordP "return" Return
            let _vec2 = keywordP "vec2" Vec2
            let _vec3 = keywordP "vec3" Vec3
            let _vec4 = keywordP "vec4" Vec4
            let _ivec2 = keywordP "ivec2" Ivec2
            let _ivec3 = keywordP "ivec3" Ivec3
            let _ivec4 = keywordP "ivec4" Ivec4
            let _bvec2 = keywordP "bvec2" Bvec2
            let _bvec3 = keywordP "bvec3" Bvec3
            let _bvec4 = keywordP "bvec4" Bvec4
            let _uint = keywordP "uint" Uint
            let _uvec2 = keywordP "uvec2" Uvec2
            let _uvec3 = keywordP "uvec3" Uvec3
            let _uvec4 = keywordP "uvec4" Uvec4
            let _dvec2 = keywordP "dvec2" Dvec2
            let _dvec3 = keywordP "dvec3" Dvec3
            let _dvec4 = keywordP "dvec4" Dvec4
            let _mat2 = keywordP "mat2" Mat2
            let _mat3 = keywordP "mat3" Mat3
            let _mat4 = keywordP "mat4" Mat4
            let _mat2x2 = keywordP "mat2x2" Mat2x2
            let _mat2x3 = keywordP "mat2x3" Mat2x3
            let _mat2x4 = keywordP "mat2x4" Mat2x4
            let _mat3x2 = keywordP "mat3x2" Mat3x2
            let _mat3x3 = keywordP "mat3x3" Mat3x3
            let _mat3x4 = keywordP "mat3x4" Mat3x4
            let _mat4x2 = keywordP "mat4x2" Mat4x2
            let _mat4x3 = keywordP "mat4x3" Mat4x3
            let _mat4x4 = keywordP "mat4x4" Mat4x4
            let _dmat2 = keywordP "dmat2" Dmat2
            let _dmat3 = keywordP "dmat3" Dmat3
            let _dmat4 = keywordP "dmat4" Dmat4
            let _dmat2x2 = keywordP "dmat2x2" Dmat2x2
            let _dmat2x3 = keywordP "dmat2x3" Dmat2x3
            let _dmat2x4 = keywordP "dmat2x4" Dmat2x4
            let _dmat3x2 = keywordP "dmat3x2" Dmat3x2
            let _dmat3x3 = keywordP "dmat3x3" Dmat3x3
            let _dmat3x4 = keywordP "dmat3x4" Dmat3x4
            let _dmat4x2 = keywordP "dmat4x2" Dmat4x2
            let _dmat4x3 = keywordP "dmat4x3" Dmat4x3
            let _dmat4x4 = keywordP "dmat4x4" Dmat4x4
            let _lowp = keywordP "lowp" Lowp
            let _mediump = keywordP "mediump" Mediump
            let _highp = keywordP "highp" Highp
            let _precision = keywordP "precision" Precision
            let _sampler1D = keywordP "sampler1D" Sampler1D
            let _sampler1DShadow = keywordP "sampler1DShadow" Sampler1DShadow
            let _sampler1DArray = keywordP "sampler1DArray" Sampler1DArray
            let _sampler1DArrayShadow = keywordP "sampler1DArrayShadow" Sampler1DArrayShadow
            let _isampler1D = keywordP "isampler1D" Isampler1D
            let _isampler1DArray = keywordP "isampler1DArray" Isampler1DArray
            let _usampler1D = keywordP "usampler1D" Usampler1D
            let _usampler1DArray = keywordP "usampler1DArray" Usampler1DArray
            let _sampler2D = keywordP "sampler2D" Sampler2D
            let _sampler2DShadow = keywordP "sampler2DShadow" Sampler2DShadow
            let _sampler2DArray = keywordP "sampler2DArray" Sampler2DArray
            let _sampler2DArrayShadow = keywordP "sampler2DArrayShadow" Sampler2DArrayShadow
            let _isampler2D = keywordP "isampler2D" Isampler2D
            let _isampler2DArray = keywordP "isampler2DArray" Isampler2DArray
            let _usampler2D = keywordP "usampler2D" Usampler2D
            let _usampler2DArray = keywordP "usampler2DArray" Usampler2DArray
            let _sampler2DRect = keywordP "sampler2DRect" Sampler2DRect
            let _sampler2DRectShadow = keywordP "sampler2DRectShadow" Sampler2DRectShadow
            let _isampler2DRect = keywordP "isampler2DRect" Isampler2DRect
            let _usampler2DRect = keywordP "usampler2DRect" Usampler2DRect
            let _sampler2DMS = keywordP "sampler2DMS" Sampler2DMS
            let _isampler2DMS = keywordP "isampler2DMS" Isampler2DMS
            let _usampler2DMS = keywordP "usampler2DMS" Usampler2DMS
            let _sampler2DMSArray = keywordP "sampler2DMSArray" Sampler2DMSArray
            let _isampler2DMSArray = keywordP "isampler2DMSArray" Isampler2DMSArray
            let _usampler2DMSArray = keywordP "usampler2DMSArray" Usampler2DMSArray
            let _sampler3D = keywordP "sampler3D" Sampler3D
            let _isampler3D = keywordP "isampler3D" Isampler3D
            let _usampler3D = keywordP "usampler3D" Usampler3D
            let _samplerCube = keywordP "samplerCube" SamplerCube
            let _samplerCubeShadow = keywordP "samplerCubeShadow" SamplerCubeShadow
            let _isamplerCube = keywordP "isamplerCube" IsamplerCube
            let _usamplerCube = keywordP "usamplerCube" UsamplerCube
            let _samplerCubeArray = keywordP "samplerCubeArray" SamplerCubeArray
            let _samplerCubeArrayShadow = keywordP "samplerCubeArrayShadow" SamplerCubeArrayShadow
            let _isamplerCubeArray = keywordP "isamplerCubeArray" IsamplerCubeArray
            let _usamplerCubeArray = keywordP "usamplerCubeArray" UsamplerCubeArray
            let _samplerBuffer = keywordP "samplerBuffer" SamplerBuffer
            let _isamplerBuffer = keywordP "isamplerBuffer" IsamplerBuffer
            let _usamplerBuffer = keywordP "usamplerBuffer" UsamplerBuffer
            let _image1D = keywordP "image1D" Image1D
            let _iimage1D = keywordP "iimage1D" Iimage1D
            let _uimage1D = keywordP "uimage1D" Uimage1D
            let _image1DArray = keywordP "image1DArray" Image1DArray
            let _iimage1DArray = keywordP "iimage1DArray" Iimage1DArray
            let _uimage1DArray = keywordP "uimage1DArray" Uimage1DArray
            let _image2D = keywordP "image2D" Image2D
            let _iimage2D = keywordP "iimage2D" Iimage2D
            let _uimage2D = keywordP "uimage2D" Uimage2D
            let _image2DArray = keywordP "image2DArray" Image2DArray
            let _iimage2DArray = keywordP "iimage2DArray" Iimage2DArray
            let _uimage2DArray = keywordP "uimage2DArray" Uimage2DArray
            let _image2DRect = keywordP "image2DRect" Image2DRect
            let _iimage2DRect = keywordP "iimage2DRect" Iimage2DRect
            let _uimage2DRect = keywordP "uimage2DRect" Uimage2DRect
            let _image2DMS = keywordP "image2DMS" Image2DMS
            let _iimage2DMS = keywordP "iimage2DMS" Iimage2DMS
            let _uimage2DMS = keywordP "uimage2DMS" Uimage2DMS
            let _image2DMSArray = keywordP "image2DMSArray" Image2DMSArray
            let _iimage2DMSArray = keywordP "iimage2DMSArray" Iimage2DMSArray
            let _uimage2DMSArray = keywordP "uimage2DMSArray" Uimage2DMSArray
            let _image3D = keywordP "image3D" Image3D
            let _iimage3D = keywordP "iimage3D" Iimage3D
            let _uimage3D = keywordP "uimage3D" Uimage3D
            let _imageCube = keywordP "imageCube" ImageCube
            let _iimageCube = keywordP "iimageCube" IimageCube
            let _uimageCube = keywordP "uimageCube" UimageCube
            let _imageCubeArray = keywordP "imageCubeArray" ImageCubeArray
            let _iimageCubeArray = keywordP "iimageCubeArray" IimageCubeArray
            let _uimageCubeArray = keywordP "uimageCubeArray" UimageCubeArray
            let _imageBuffer = keywordP "imageBuffer" ImageBuffer
            let _iimageBuffer = keywordP "iimageBuffer" IimageBuffer
            let _uimageBuffer = keywordP "uimageBuffer" UimageBuffer
            let _struct = keywordP "struct" Struct
    
                    
    let typeNameList endP = many1Till anyChar endP
    
    
    
    module StorageQualifierParser =
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
    
    module TypeQualifierParser =
        let singleTypeQualifier =
            StorageQualifierParser.storageQualifier |>> StorageQualifier

        let typeQualifierParser: Parser<TypeQualifier, _> = many1 singleTypeQualifier
    
    module TypeSpecifierParser =
        let typeSpecifierNonArray =
            TokensParsers.Keyword._void
            <|> TokensParsers.Keyword._float
            <|> TokensParsers.Keyword._double
            <|> TokensParsers.Keyword._int
            <|> TokensParsers.Keyword._uint
            <|> TokensParsers.Keyword._bool
            <|> TokensParsers.Keyword._vec2
            <|> TokensParsers.Keyword._vec3
            <|> TokensParsers.Keyword._vec4
            <|> TokensParsers.Keyword._dvec2
            <|> TokensParsers.Keyword._dvec3
            <|> TokensParsers.Keyword._dvec4
            <|> TokensParsers.Keyword._bvec2
            <|> TokensParsers.Keyword._bvec3
            <|> TokensParsers.Keyword._bvec4
            <|> TokensParsers.Keyword._ivec2
            <|> TokensParsers.Keyword._ivec3
            <|> TokensParsers.Keyword._ivec4
            <|> TokensParsers.Keyword._uvec2
            <|> TokensParsers.Keyword._uvec3
            <|> TokensParsers.Keyword._uvec4
            <|> TokensParsers.Keyword._mat2
            <|> TokensParsers.Keyword._mat3
            <|> TokensParsers.Keyword._mat4
            <|> TokensParsers.Keyword._mat2x2
            <|> TokensParsers.Keyword._mat2x3
            <|> TokensParsers.Keyword._mat2x4
            <|> TokensParsers.Keyword._mat3x2
            <|> TokensParsers.Keyword._mat3x3
            <|> TokensParsers.Keyword._mat3x4
            <|> TokensParsers.Keyword._mat4x2
            <|> TokensParsers.Keyword._mat4x3
            <|> TokensParsers.Keyword._mat4x4
            <|> TokensParsers.Keyword._dmat2
            <|> TokensParsers.Keyword._dmat3
            <|> TokensParsers.Keyword._dmat4
            <|> TokensParsers.Keyword._dmat2x2
            <|> TokensParsers.Keyword._dmat2x3
            <|> TokensParsers.Keyword._dmat2x4
            <|> TokensParsers.Keyword._dmat3x2
            <|> TokensParsers.Keyword._dmat3x3
            <|> TokensParsers.Keyword._dmat3x4
            <|> TokensParsers.Keyword._dmat4x2
            <|> TokensParsers.Keyword._dmat4x3
            <|> TokensParsers.Keyword._dmat4x4
            <|> TokensParsers.Keyword._atomic_uint
            <|> TokensParsers.Keyword._sampler2D
            <|> TokensParsers.Keyword._sampler3D
            <|> TokensParsers.Keyword._samplerCube
            <|> TokensParsers.Keyword._sampler2DShadow
            <|> TokensParsers.Keyword._samplerCubeShadow
            <|> TokensParsers.Keyword._sampler2DArray
            <|> TokensParsers.Keyword._sampler2DArrayShadow
            <|> TokensParsers.Keyword._samplerCubeArray
            <|> TokensParsers.Keyword._samplerCubeArrayShadow
            <|> TokensParsers.Keyword._isampler2D
            <|> TokensParsers.Keyword._isampler3D
            <|> TokensParsers.Keyword._isamplerCube
            <|> TokensParsers.Keyword._isampler2DArray
            <|> TokensParsers.Keyword._isamplerCubeArray
            <|> TokensParsers.Keyword._usampler2D
            <|> TokensParsers.Keyword._usampler3D
            <|> TokensParsers.Keyword._usamplerCube
            <|> TokensParsers.Keyword._usampler2DArray
            <|> TokensParsers.Keyword._usamplerCubeArray
            <|> TokensParsers.Keyword._sampler1D
            <|> TokensParsers.Keyword._sampler1DShadow
            <|> TokensParsers.Keyword._sampler1DArray
            <|> TokensParsers.Keyword._sampler1DArrayShadow
            <|> TokensParsers.Keyword._isampler1D
            <|> TokensParsers.Keyword._isampler1DArray
            <|> TokensParsers.Keyword._usampler1D
            <|> TokensParsers.Keyword._usampler1DArray
            <|> TokensParsers.Keyword._sampler2DRect
            <|> TokensParsers.Keyword._sampler2DRectShadow
            <|> TokensParsers.Keyword._isampler2DRect
            <|> TokensParsers.Keyword._usampler2DRect
            <|> TokensParsers.Keyword._samplerBuffer
            <|> TokensParsers.Keyword._isamplerBuffer
            <|> TokensParsers.Keyword._usamplerBuffer
            <|> TokensParsers.Keyword._sampler2DMS
            <|> TokensParsers.Keyword._isampler2DMS
            <|> TokensParsers.Keyword._usampler2DMS
            <|> TokensParsers.Keyword._sampler2DMSArray
            <|> TokensParsers.Keyword._isampler2DMSArray
            <|> TokensParsers.Keyword._usampler2DMSArray
            <|> TokensParsers.Keyword._image2D
            <|> TokensParsers.Keyword._iimage2D
            <|> TokensParsers.Keyword._uimage2D
            <|> TokensParsers.Keyword._image3D
            <|> TokensParsers.Keyword._iimage3D
            <|> TokensParsers.Keyword._uimage3D
            <|> TokensParsers.Keyword._imageCube
            <|> TokensParsers.Keyword._iimageCube
            <|> TokensParsers.Keyword._uimageCube
            <|> TokensParsers.Keyword._imageBuffer
            <|> TokensParsers.Keyword._iimageBuffer
            <|> TokensParsers.Keyword._uimageBuffer
            <|> TokensParsers.Keyword._image1D
            <|> TokensParsers.Keyword._iimage1D
            <|> TokensParsers.Keyword._uimage1D
            <|> TokensParsers.Keyword._image1DArray
            <|> TokensParsers.Keyword._iimage1DArray
            <|> TokensParsers.Keyword._uimage1DArray
            <|> TokensParsers.Keyword._image2DRect
            <|> TokensParsers.Keyword._iimage2DRect
            <|> TokensParsers.Keyword._uimage2DRect
            <|> TokensParsers.Keyword._image2DArray
            <|> TokensParsers.Keyword._iimage2DArray
            <|> TokensParsers.Keyword._uimage2DArray
            <|> TokensParsers.Keyword._imageCubeArray
            <|> TokensParsers.Keyword._iimageCubeArray
            <|> TokensParsers.Keyword._uimageCubeArray
            <|> TokensParsers.Keyword._image2DMS
            <|> TokensParsers.Keyword._iimage2DMS
            <|> TokensParsers.Keyword._uimage2DMS
            <|> TokensParsers.Keyword._image2DMSArray
            <|> TokensParsers.Keyword._iimage2DMSArray
            <|> TokensParsers.Keyword._uimage2DMSArray

        let constantExpression = preturn () //TODO:
        
        let singleArraySpecifier =
            (TokensParsers.lBracket .>> TokensParsers.rBracket >>. preturn ())
            <|> (TokensParsers.lBracket .>> constantExpression .>> TokensParsers.rBracket >>. preturn ())
        
        let arraySpecifier =
            many1 singleArraySpecifier
        
        ()
    
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
    CharParsers.run Parser.StorageQualifierParser.storageQualifier "uniform" |> printfn "%A"
    
    printfn "Hello World from F#!"
    0 // return an integer exit code
