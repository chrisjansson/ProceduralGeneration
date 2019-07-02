module GlslParser.Tokenizer

open FParsec

type Token = 
    | CONST
    | BOOL
    | FLOAT
    | INT
    | UINT
    | DOUBLE
    | BVEC2
    | BVEC3
    | BVEC4
    | IVEC2
    | IVEC3
    | IVEC4
    | UVEC2
    | UVEC3
    | UVEC4
    | VEC2
    | VEC3
    | VEC4
    | MAT2
    | MAT3
    | MAT4
    | CENTROID
    | IN
    | OUT
    | INOUT
    | UNIFORM
    | PATCH
    | SAMPLE
    | BUFFER
    | SHARED
    | COHERENT
    | VOLATILE
    | RESTRICT
    | READONLY
    | WRITEONLY
    | NOPERSPECTIVE
    | FLAT
    | SMOOTH
    | LAYOUT
    | MAT2X2
    | MAT2X3
    | MAT2X4
    | MAT3X2
    | MAT3X3
    | MAT3X4
    | MAT4X2
    | MAT4X3
    | MAT4X4
    | DVEC2
    | DVEC3
    | DVEC4
    | DMAT2
    | DMAT3
    | DMAT4
    | DMAT2X2
    | DMAT2X3
    | DMAT2X4
    | DMAT3X2
    | DMAT3X3
    | DMAT3X4
    | DMAT4X2
    | DMAT4X3
    | DMAT4X4
    | ATOMICUINT
    | SAMPLER2D
    | SAMPLER3D
    | SAMPLERCUBE
    | SAMPLER2DSHADOW
    | SAMPLERCUBESHADOW
    | SAMPLER2DARRAY
    | SAMPLER2DARRAYSHADOW
    | ISAMPLER2D
    | ISAMPLER3D
    | ISAMPLERCUBE
    | ISAMPLER2DARRAY
    | USAMPLER2D
    | USAMPLER3D
    | USAMPLERCUBE
    | USAMPLER2DARRAY
    | SAMPLER1D
    | SAMPLER1DSHADOW
    | SAMPLER1DARRAY
    | SAMPLER1DARRAYSHADOW
    | ISAMPLER1D
    | ISAMPLER1DARRAY
    | USAMPLER1D
    | USAMPLER1DARRAY
    | SAMPLER2DRECT
    | SAMPLER2DRECTSHADOW
    | ISAMPLER2DRECT
    | USAMPLER2DRECT
    | SAMPLERBUFFER
    | ISAMPLERBUFFER
    | USAMPLERBUFFER
    | SAMPLERCUBEARRAY
    | SAMPLERCUBEARRAYSHADOW
    | ISAMPLERCUBEARRAY
    | USAMPLERCUBEARRAY
    | SAMPLER2DMS
    | ISAMPLER2DMS
    | USAMPLER2DMS
    | SAMPLER2DMSARRAY
    | ISAMPLER2DMSARRAY
    | USAMPLER2DMSARRAY
    | IMAGE2D
    | IIMAGE2D
    | UIMAGE2D
    | IMAGE3D
    | IIMAGE3D
    | UIMAGE3D
    | IMAGECUBE
    | IIMAGECUBE
    | UIMAGECUBE
    | IMAGEBUFFER
    | IIMAGEBUFFER
    | UIMAGEBUFFER
    | IMAGE2DARRAY
    | IIMAGE2DARRAY
    | UIMAGE2DARRAY
    | IMAGECUBEARRAY
    | IIMAGECUBEARRAY
    | UIMAGECUBEARRAY
    | IMAGE1D
    | IIMAGE1D
    | UIMAGE1D
    | IMAGE1DARRAY
    | IIMAGE1DARRAY
    | UIMAGE1DARRAY
    | IMAGE2DRECT
    | IIMAGE2DRECT
    | UIMAGE2DRECT
    | IMAGE2DMS
    | IIMAGE2DMS
    | UIMAGE2DMS
    | IMAGE2DMSARRAY
    | IIMAGE2DMSARRAY
    | UIMAGE2DMSARRAY
    | STRUCT
    | VOID
    | WHILE
    | BREAK
    | CONTINUE
    | DO
    | ELSE
    | FOR
    | IF
    | DISCARD
    | RETURN
    | SWITCH
    | CASE
    | DEFAULT
    | SUBROUTINE
    | IDENTIFIER of string
    | TYPENAME
    | FLOATCONSTANT
    | INTCONSTANT
    | UINTCONSTANT
    | BOOLCONSTANT of bool
    | DOUBLECONSTANT
    | FIELDSELECTION
    | LEFTOP
    | RIGHTOP
    | INCOP
    | DECOP
    | LEOP
    | GEOP
    | EQOP
    | NEOP
    | ANDOP
    | OROP
    | XOROP
    | MULASSIGN
    | DIVASSIGN
    | ADDASSIGN
    | MODASSIGN
    | LEFTASSIGN
    | RIGHTASSIGN
    | ANDASSIGN
    | XORASSIGN
    | ORASSIGN
    | SUBASSIGN
    | LEFTPAREN
    | RIGHTPAREN
    | LEFTBRACKET
    | RIGHTBRACKET
    | LEFTBRACE
    | RIGHTBRACE
    | DOT
    | COMMA
    | COLON
    | EQUAL
    | SEMICOLON
    | BANG
    | DASH
    | TILDE
    | PLUS
    | STAR
    | SLASH
    | PERCENT
    | LEFTANGLE
    | RIGHTANGLE
    | VERTICALBAR
    | CARET
    | AMPERSAND
    | QUESTION
    | INVARIANT
    | PRECISE
    | HIGHPRECISION
    | MEDIUMPRECISION
    | LOWPRECISION
    | PRECISION
    | ATTRIBUTE //TODO: Check these, these are keywords but not tokens?
    | VARYING
     
module Keyword =
    let keywordP str token =
        pstring str >>. preturn token
        
    let _const: Parser<_, unit> = keywordP "const" CONST
    let _uniform: Parser<_, unit> = keywordP "uniform" UNIFORM
    let _buffer: Parser<_, unit> = keywordP "buffer" BUFFER
    let _shared: Parser<_, unit> = keywordP "shared" SHARED
    let _attribute: Parser<_, unit> = keywordP "attribute" ATTRIBUTE
    let _varying: Parser<_, unit> = keywordP "varying" VARYING
    let _coherent: Parser<_, unit> = keywordP "coherent" COHERENT
    let _volatile: Parser<_, unit> = keywordP "volatile" VOLATILE
    let _restrict: Parser<_, unit> = keywordP "restrict" RESTRICT
    let _readonly: Parser<_, unit> = keywordP "readonly" READONLY
    let _writeonly: Parser<_, unit> = keywordP "writeonly" WRITEONLY
    let _atomic_uint: Parser<_, unit> = keywordP "atomic_uint" ATOMICUINT
    let _layout: Parser<_, unit> = keywordP "layout" LAYOUT
    let _centroid: Parser<_, unit> = keywordP "centroid" CENTROID
    let _flat: Parser<_, unit> = keywordP "flat" FLAT
    let _smooth: Parser<_, unit> = keywordP "smooth" SMOOTH
    let _noperspective: Parser<_, unit> = keywordP "noperspective" NOPERSPECTIVE
    let _patch: Parser<_, unit> = keywordP "patch" PATCH
    let _sample: Parser<_, unit> = keywordP "sample" SAMPLE
    let _invariant: Parser<_, unit> = keywordP "invariant" INVARIANT
    let _precise: Parser<_, unit> = keywordP "precise" PRECISE
    let _break: Parser<_, unit> = keywordP "break" BREAK
    let _continue: Parser<_, unit> = keywordP "continue" CONTINUE
    let _do: Parser<_, unit> = keywordP "do" DO
    let _for: Parser<_, unit> = keywordP "for" FOR
    let _while: Parser<_, unit> = keywordP "while" WHILE
    let _switch: Parser<_, unit> = keywordP "switch" SWITCH
    let _case: Parser<_, unit> = keywordP "case" CASE
    let _default: Parser<_, unit> = keywordP "default" DEFAULT
    let _if: Parser<_, unit> = keywordP "if" IF
    let _else: Parser<_, unit> = keywordP "else" ELSE
    let _subroutine: Parser<_, unit> = keywordP "subroutine" SUBROUTINE
    let _in: Parser<_, unit> = keywordP "in" IN
    let _out: Parser<_, unit> = keywordP "out" OUT
    let _inout: Parser<_, unit> = keywordP "inout" INOUT
    let _int: Parser<_, unit> = keywordP "int" INT
    let _void: Parser<_, unit> = keywordP "void" VOID
    let _bool: Parser<_, unit> = keywordP "bool" BOOL
    let _true: Parser<_, unit> = keywordP "true" (BOOLCONSTANT true)
    let _false: Parser<_, unit> = keywordP "false" (BOOLCONSTANT false)
    let _float: Parser<_, unit> = keywordP "float" FLOAT
    let _double: Parser<_, unit> = keywordP "double" DOUBLE
    let _discard: Parser<_, unit> = keywordP "discard" DISCARD
    let _return: Parser<_, unit> = keywordP "return" RETURN
    let _vec2: Parser<_, unit> = keywordP "vec2" VEC2
    let _vec3: Parser<_, unit> = keywordP "vec3" VEC3
    let _vec4: Parser<_, unit> = keywordP "vec4" VEC4
    let _ivec2: Parser<_, unit> = keywordP "ivec2" IVEC2
    let _ivec3: Parser<_, unit> = keywordP "ivec3" IVEC3
    let _ivec4: Parser<_, unit> = keywordP "ivec4" IVEC4
    let _bvec2: Parser<_, unit> = keywordP "bvec2" BVEC2
    let _bvec3: Parser<_, unit> = keywordP "bvec3" BVEC3
    let _bvec4: Parser<_, unit> = keywordP "bvec4" BVEC4
    let _uint: Parser<_, unit> = keywordP "uint" UINT
    let _uvec2: Parser<_, unit> = keywordP "uvec2" UVEC2
    let _uvec3: Parser<_, unit> = keywordP "uvec3" UVEC3
    let _uvec4: Parser<_, unit> = keywordP "uvec4" UVEC4
    let _dvec2: Parser<_, unit> = keywordP "dvec2" DVEC2
    let _dvec3: Parser<_, unit> = keywordP "dvec3" DVEC3
    let _dvec4: Parser<_, unit> = keywordP "dvec4" DVEC4
    let _mat2: Parser<_, unit> = keywordP "mat2" MAT2
    let _mat3: Parser<_, unit> = keywordP "mat3" MAT3
    let _mat4: Parser<_, unit> = keywordP "mat4" MAT4
    let _mat2x2: Parser<_, unit> = keywordP "mat2x2" MAT2X2
    let _mat2x3: Parser<_, unit> = keywordP "mat2x3" MAT2X3
    let _mat2x4: Parser<_, unit> = keywordP "mat2x4" MAT2X4
    let _mat3x2: Parser<_, unit> = keywordP "mat3x2" MAT3X2
    let _mat3x3: Parser<_, unit> = keywordP "mat3x3" MAT3X3
    let _mat3x4: Parser<_, unit> = keywordP "mat3x4" MAT3X4
    let _mat4x2: Parser<_, unit> = keywordP "mat4x2" MAT4X2
    let _mat4x3: Parser<_, unit> = keywordP "mat4x3" MAT4X3
    let _mat4x4: Parser<_, unit> = keywordP "mat4x4" MAT4X4
    let _dmat2: Parser<_, unit> = keywordP "dmat2" DMAT2
    let _dmat3: Parser<_, unit> = keywordP "dmat3" DMAT3
    let _dmat4: Parser<_, unit> = keywordP "dmat4" DMAT4
    let _dmat2x2: Parser<_, unit> = keywordP "dmat2x2" DMAT2X2
    let _dmat2x3: Parser<_, unit> = keywordP "dmat2x3" DMAT2X3
    let _dmat2x4: Parser<_, unit> = keywordP "dmat2x4" DMAT2X4
    let _dmat3x2: Parser<_, unit> = keywordP "dmat3x2" DMAT3X2
    let _dmat3x3: Parser<_, unit> = keywordP "dmat3x3" DMAT3X3
    let _dmat3x4: Parser<_, unit> = keywordP "dmat3x4" DMAT3X4
    let _dmat4x2: Parser<_, unit> = keywordP "dmat4x2" DMAT4X2
    let _dmat4x3: Parser<_, unit> = keywordP "dmat4x3" DMAT4X3
    let _dmat4x4: Parser<_, unit> = keywordP "dmat4x4" DMAT4X4
    let _lowp: Parser<_, unit> = keywordP "lowp" LOWPRECISION
    let _mediump: Parser<_, unit> = keywordP "mediump" MEDIUMPRECISION
    let _highp: Parser<_, unit> = keywordP "highp" HIGHPRECISION
    let _precision: Parser<_, unit> = keywordP "precision" PRECISION
    let _sampler1D: Parser<_, unit> = keywordP "sampler1D" SAMPLER1D
    let _sampler1DShadow: Parser<_, unit> = keywordP "sampler1DShadow" SAMPLER1DSHADOW
    let _sampler1DArray: Parser<_, unit> = keywordP "sampler1DArray" SAMPLER1DARRAY
    let _sampler1DArrayShadow: Parser<_, unit> = keywordP "sampler1DArrayShadow" SAMPLER1DARRAYSHADOW
    let _isampler1D: Parser<_, unit> = keywordP "isampler1D" ISAMPLER1D
    let _isampler1DArray: Parser<_, unit> = keywordP "isampler1DArray" ISAMPLER1DARRAY
    let _usampler1D: Parser<_, unit> = keywordP "usampler1D" USAMPLER1D
    let _usampler1DArray: Parser<_, unit> = keywordP "usampler1DArray" USAMPLER1DARRAY
    let _sampler2D: Parser<_, unit> = keywordP "sampler2D" SAMPLER2D
    let _sampler2DShadow: Parser<_, unit> = keywordP "sampler2DShadow" SAMPLER2DSHADOW
    let _sampler2DArray: Parser<_, unit> = keywordP "sampler2DArray" SAMPLER2DARRAY
    let _sampler2DArrayShadow: Parser<_, unit> = keywordP "sampler2DArrayShadow" SAMPLER2DARRAYSHADOW
    let _isampler2D: Parser<_, unit> = keywordP "isampler2D" ISAMPLER2D
    let _isampler2DArray: Parser<_, unit> = keywordP "isampler2DArray" ISAMPLER2DARRAY
    let _usampler2D: Parser<_, unit> = keywordP "usampler2D" USAMPLER2D
    let _usampler2DArray: Parser<_, unit> = keywordP "usampler2DArray" USAMPLER2DARRAY
    let _sampler2DRect: Parser<_, unit> = keywordP "sampler2DRect" SAMPLER2DRECT
    let _sampler2DRectShadow: Parser<_, unit> = keywordP "sampler2DRectShadow" SAMPLER2DRECTSHADOW
    let _isampler2DRect: Parser<_, unit> = keywordP "isampler2DRect" ISAMPLER2DRECT
    let _usampler2DRect: Parser<_, unit> = keywordP "usampler2DRect" USAMPLER2DRECT
    let _sampler2DMS: Parser<_, unit> = keywordP "sampler2DMS" SAMPLER2DMS
    let _isampler2DMS: Parser<_, unit> = keywordP "isampler2DMS" ISAMPLER2DMS
    let _usampler2DMS: Parser<_, unit> = keywordP "usampler2DMS" USAMPLER2DMS
    let _sampler2DMSArray: Parser<_, unit> = keywordP "sampler2DMSArray" SAMPLER2DMSARRAY
    let _isampler2DMSArray: Parser<_, unit> = keywordP "isampler2DMSArray" ISAMPLER2DMSARRAY
    let _usampler2DMSArray: Parser<_, unit> = keywordP "usampler2DMSArray" USAMPLER2DMSARRAY
    let _sampler3D: Parser<_, unit> = keywordP "sampler3D" SAMPLER3D
    let _isampler3D: Parser<_, unit> = keywordP "isampler3D" ISAMPLER3D
    let _usampler3D: Parser<_, unit> = keywordP "usampler3D" USAMPLER3D
    let _samplerCube: Parser<_, unit> = keywordP "samplerCube" SAMPLERCUBE
    let _samplerCubeShadow: Parser<_, unit> = keywordP "samplerCubeShadow" SAMPLERCUBESHADOW
    let _isamplerCube: Parser<_, unit> = keywordP "isamplerCube" ISAMPLERCUBE
    let _usamplerCube: Parser<_, unit> = keywordP "usamplerCube" USAMPLERCUBE
    let _samplerCubeArray: Parser<_, unit> = keywordP "samplerCubeArray" SAMPLERCUBEARRAY
    let _samplerCubeArrayShadow: Parser<_, unit> = keywordP "samplerCubeArrayShadow" SAMPLERCUBEARRAYSHADOW
    let _isamplerCubeArray: Parser<_, unit> = keywordP "isamplerCubeArray" ISAMPLERCUBEARRAY
    let _usamplerCubeArray: Parser<_, unit> = keywordP "usamplerCubeArray" USAMPLERCUBEARRAY
    let _samplerBuffer: Parser<_, unit> = keywordP "samplerBuffer" SAMPLERBUFFER
    let _isamplerBuffer: Parser<_, unit> = keywordP "isamplerBuffer" ISAMPLERBUFFER
    let _usamplerBuffer: Parser<_, unit> = keywordP "usamplerBuffer" USAMPLERBUFFER
    let _image1D: Parser<_, unit> = keywordP "image1D" IMAGE1D
    let _iimage1D: Parser<_, unit> = keywordP "iimage1D" IIMAGE1D
    let _uimage1D: Parser<_, unit> = keywordP "uimage1D" UIMAGE1D
    let _image1DArray: Parser<_, unit> = keywordP "image1DArray" IMAGE1DARRAY
    let _iimage1DArray: Parser<_, unit> = keywordP "iimage1DArray" IIMAGE1DARRAY
    let _uimage1DArray: Parser<_, unit> = keywordP "uimage1DArray" UIMAGE1DARRAY
    let _image2D: Parser<_, unit> = keywordP "image2D" IMAGE2D
    let _iimage2D: Parser<_, unit> = keywordP "iimage2D" IIMAGE2D
    let _uimage2D: Parser<_, unit> = keywordP "uimage2D" UIMAGE2D
    let _image2DArray: Parser<_, unit> = keywordP "image2DArray" IMAGE2DARRAY
    let _iimage2DArray: Parser<_, unit> = keywordP "iimage2DArray" IIMAGE2DARRAY
    let _uimage2DArray: Parser<_, unit> = keywordP "uimage2DArray" UIMAGE2DARRAY
    let _image2DRect: Parser<_, unit> = keywordP "image2DRect" IMAGE2DRECT
    let _iimage2DRect: Parser<_, unit> = keywordP "iimage2DRect" IIMAGE2DRECT
    let _uimage2DRect: Parser<_, unit> = keywordP "uimage2DRect" UIMAGE2DRECT
    let _image2DMS: Parser<_, unit> = keywordP "image2DMS" IMAGE2DMS
    let _iimage2DMS: Parser<_, unit> = keywordP "iimage2DMS" IIMAGE2DMS
    let _uimage2DMS: Parser<_, unit> = keywordP "uimage2DMS" UIMAGE2DMS
    let _image2DMSArray: Parser<_, unit> = keywordP "image2DMSArray" IMAGE2DMSARRAY
    let _iimage2DMSArray: Parser<_, unit> = keywordP "iimage2DMSArray" IIMAGE2DMSARRAY
    let _uimage2DMSArray: Parser<_, unit> = keywordP "uimage2DMSArray" UIMAGE2DMSARRAY
    let _image3D: Parser<_, unit> = keywordP "image3D" IMAGE3D
    let _iimage3D: Parser<_, unit> = keywordP "iimage3D" IIMAGE3D
    let _uimage3D: Parser<_, unit> = keywordP "uimage3D" UIMAGE3D
    let _imageCube: Parser<_, unit> = keywordP "imageCube" IMAGECUBE
    let _iimageCube: Parser<_, unit> = keywordP "iimageCube" IIMAGECUBE
    let _uimageCube: Parser<_, unit> = keywordP "uimageCube" UIMAGECUBE
    let _imageCubeArray: Parser<_, unit> = keywordP "imageCubeArray" IMAGECUBEARRAY
    let _iimageCubeArray: Parser<_, unit> = keywordP "iimageCubeArray" IIMAGECUBEARRAY
    let _uimageCubeArray: Parser<_, unit> = keywordP "uimageCubeArray" UIMAGECUBEARRAY
    let _imageBuffer: Parser<_, unit> = keywordP "imageBuffer" IMAGEBUFFER
    let _iimageBuffer: Parser<_, unit> = keywordP "iimageBuffer" IIMAGEBUFFER
    let _uimageBuffer: Parser<_, unit> = keywordP "uimageBuffer" UIMAGEBUFFER
    let _struct: Parser<_, unit> = keywordP "struct" STRUCT

    let keywordParser =
        _const
        <|> _uniform
        <|> _buffer
        <|> _shared
        <|> _attribute
        <|> _varying
        <|> _coherent
        <|> _volatile
        <|> _restrict
        <|> _readonly
        <|> _writeonly
        <|> _atomic_uint
        <|> _layout
        <|> _centroid
        <|> _flat
        <|> _smooth
        <|> _noperspective
        <|> _patch
        <|> _sample
        <|> _invariant
        <|> _precise
        <|> _break
        <|> _continue
        <|> _do
        <|> _for
        <|> _while
        <|> _switch
        <|> _case
        <|> _default
        <|> _if
        <|> _else
        <|> _subroutine
        <|> _in
        <|> _out
        <|> _inout
        <|> _int
        <|> _void
        <|> _bool
        <|> _true
        <|> _false
        <|> _float
        <|> _double
        <|> _discard
        <|> _return
        <|> _vec2
        <|> _vec3
        <|> _vec4
        <|> _ivec2
        <|> _ivec3
        <|> _ivec4
        <|> _bvec2
        <|> _bvec3
        <|> _bvec4
        <|> _uint
        <|> _uvec2
        <|> _uvec3
        <|> _uvec4
        <|> _dvec2
        <|> _dvec3
        <|> _dvec4
        <|> _mat2
        <|> _mat3
        <|> _mat4
        <|> _mat2x2
        <|> _mat2x3
        <|> _mat2x4
        <|> _mat3x2
        <|> _mat3x3
        <|> _mat3x4
        <|> _mat4x2
        <|> _mat4x3
        <|> _mat4x4
        <|> _dmat2
        <|> _dmat3
        <|> _dmat4
        <|> _dmat2x2
        <|> _dmat2x3
        <|> _dmat2x4
        <|> _dmat3x2
        <|> _dmat3x3
        <|> _dmat3x4
        <|> _dmat4x2
        <|> _dmat4x3
        <|> _dmat4x4
        <|> _lowp
        <|> _mediump
        <|> _highp
        <|> _precision
        <|> _sampler1D
        <|> _sampler1DShadow
        <|> _sampler1DArray
        <|> _sampler1DArrayShadow
        <|> _isampler1D
        <|> _isampler1DArray
        <|> _usampler1D
        <|> _usampler1DArray
        <|> _sampler2D
        <|> _sampler2DShadow
        <|> _sampler2DArray
        <|> _sampler2DArrayShadow
        <|> _isampler2D
        <|> _isampler2DArray
        <|> _usampler2D
        <|> _usampler2DArray
        <|> _sampler2DRect
        <|> _sampler2DRectShadow
        <|> _isampler2DRect
        <|> _usampler2DRect
        <|> _sampler2DMS
        <|> _isampler2DMS
        <|> _usampler2DMS
        <|> _sampler2DMSArray
        <|> _isampler2DMSArray
        <|> _usampler2DMSArray
        <|> _sampler3D
        <|> _isampler3D
        <|> _usampler3D
        <|> _samplerCube
        <|> _samplerCubeShadow
        <|> _isamplerCube
        <|> _usamplerCube
        <|> _samplerCubeArray
        <|> _samplerCubeArrayShadow
        <|> _isamplerCubeArray
        <|> _usamplerCubeArray
        <|> _samplerBuffer
        <|> _isamplerBuffer
        <|> _usamplerBuffer
        <|> _image1D
        <|> _iimage1D
        <|> _uimage1D
        <|> _image1DArray
        <|> _iimage1DArray
        <|> _uimage1DArray
        <|> _image2D
        <|> _iimage2D
        <|> _uimage2D
        <|> _image2DArray
        <|> _iimage2DArray
        <|> _uimage2DArray
        <|> _image2DRect
        <|> _iimage2DRect
        <|> _uimage2DRect
        <|> _image2DMS
        <|> _iimage2DMS
        <|> _uimage2DMS
        <|> _image2DMSArray
        <|> _iimage2DMSArray
        <|> _uimage2DMSArray
        <|> _image3D
        <|> _iimage3D
        <|> _uimage3D
        <|> _imageCube
        <|> _iimageCube
        <|> _uimageCube
        <|> _imageCubeArray
        <|> _iimageCubeArray
        <|> _uimageCubeArray
        <|> _imageBuffer
        <|> _iimageBuffer
        <|> _uimageBuffer
        <|> _struct
        
module Identifier =
    let nonDigits =
        [|
            '_';'a';'b';'c';'d';'e';'f';'g';'h';'i';'j';'k';'l';'m';'n';'o';'p';'q';'r';'s';'t';'u';'v';'w';'x';'y';'z';'A';'B';'C';'D';'E';'F';'G';'H';'I';'J';'K';'L';'M';'N';'O';'P';'Q';'R';'S';'T';'U';'V';'W';'X';'Y';'Z';
        |]
        
    let digits =
        [|
            '0';'1';'2';'3';'4';'5';'6';'7';'8';'9'
        |]
    
    let identifier: Parser<_, unit> =
        let nonDigitP = anyOf nonDigits
        let digitP = anyOf digits
        nonDigitP .>>. many1 (nonDigitP <|> digitP) |>> (fun (c, carr) -> c::carr) |>> (fun chars -> System.String.Join("", chars) |> IDENTIFIER)