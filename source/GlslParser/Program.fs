open GlslParser.ParserLibrary

module Parser =
    open GlslParser.ParserLibrary

    open GlslParser.ParserLibrary.ParserWithPositionalErrors
        
    let tokenP token =
        satisfy (fun t -> t = token) (sprintf "%A" token)

    type Token = GlslParser.Tokenizer.Token


    let semicolonP = tokenP GlslParser.Tokenizer.Token.SEMICOLON
    let commaP = tokenP GlslParser.Tokenizer.Token.COMMA
    let leftBracketP = tokenP GlslParser.Tokenizer.Token.LEFTBRACKET
    let rightBracketP = tokenP GlslParser.Tokenizer.Token.RIGHTBRACKET
    let leftBraceP = tokenP GlslParser.Tokenizer.Token.LEFTBRACE
    let rightBraceP = tokenP GlslParser.Tokenizer.Token.RIGHTBRACE
    let leftParenP = tokenP Token.LEFTPAREN
    let rightParenP = tokenP Token.RIGHTPAREN
    
    let constantExpressionP = failwith "todo"
    
    
    let identifierP =
        let matcher token =
            match token with
            | GlslParser.Tokenizer.Token.IDENTIFIER _ -> true
            | _ -> false
        satisfy matcher ("IDENTIFIER")

        
    let typeSpecifierPRef = ParserWithPositionalErrors.createRefParser ()
    let statementPRef = ParserWithPositionalErrors.createRefParser ()
    let expressionPRef = ParserWithPositionalErrors.createRefParser ()
    let postfixExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    let assignmentExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    let functionCallHeaderWithParametersPRef = ParserWithPositionalErrors.createRefParser ()
    let unaryExpressionPRef = ParserWithPositionalErrors.createRefParser ()
        
    let rec structDeclaratorP =
        (identifierP |>> ignore) <|> (identifierP .>>. arraySpecifierP |>> ignore)

    and structDeclaratorListP =
        (structDeclaratorP .>>. (many (commaP .>>. structDeclaratorP)) |>> ignore)
        
    and structDeclarationP =
        (typeSpecifierPRef.Parser .>>. structDeclaratorListP .>>. semicolonP |>> ignore)
        <|> (typeQualifierP .>>. typeSpecifierPRef.Parser .>>. structDeclaratorListP .>>. semicolonP |>> ignore)
    
    and structDeclarationListP = (many1 structDeclarationP) |>> ignore

    and structSpecifierP =
        (tokenP Token.STRUCT .>>. identifierP .>>. leftBraceP .>>. structDeclarationListP .>>. rightBraceP |>> ignore)
        <|> (tokenP Token.STRUCT .>>. leftBraceP .>>. structDeclarationListP .>>. rightBraceP |>> ignore)
    
    and typeSpecifierNonArrayP =
        let tokens = [
                Token.VOID
                Token.FLOAT
                Token.DOUBLE
                Token.INT
                Token.UINT
                Token.BOOL
                Token.VEC2
                Token.VEC3
                Token.VEC4
                Token.DVEC2
                Token.DVEC3
                Token.DVEC4
                Token.BVEC2
                Token.BVEC3
                Token.BVEC4
                Token.IVEC2
                Token.IVEC3
                Token.IVEC4
                Token.UVEC2
                Token.UVEC3
                Token.UVEC4
                Token.MAT2
                Token.MAT3
                Token.MAT4
                Token.MAT2X2
                Token.MAT2X3
                Token.MAT2X4
                Token.MAT3X2
                Token.MAT3X3
                Token.MAT3X4
                Token.MAT4X2
                Token.MAT4X3
                Token.MAT4X4
                Token.DMAT2
                Token.DMAT3
                Token.DMAT4
                Token.DMAT2X2
                Token.DMAT2X3
                Token.DMAT2X4
                Token.DMAT3X2
                Token.DMAT3X3
                Token.DMAT3X4
                Token.DMAT4X2
                Token.DMAT4X3
                Token.DMAT4X4
                Token.ATOMICUINT
                Token.SAMPLER2D
                Token.SAMPLER3D
                Token.SAMPLERCUBE
                Token.SAMPLER2DSHADOW
                Token.SAMPLERCUBESHADOW
                Token.SAMPLER2DARRAY
                Token.SAMPLER2DARRAYSHADOW
                Token.SAMPLERCUBEARRAY
                Token.SAMPLERCUBEARRAYSHADOW
                Token.ISAMPLER2D
                Token.ISAMPLER3D
                Token.ISAMPLERCUBE
                Token.ISAMPLER2DARRAY
                Token.ISAMPLERCUBEARRAY
                Token.USAMPLER2D
                Token.USAMPLER3D
                Token.USAMPLERCUBE
                Token.USAMPLER2DARRAY
                Token.USAMPLERCUBEARRAY
                Token.SAMPLER1D
                Token.SAMPLER1DSHADOW
                Token.SAMPLER1DARRAY
                Token.SAMPLER1DARRAYSHADOW
                Token.ISAMPLER1D
                Token.ISAMPLER1DARRAY
                Token.USAMPLER1D
                Token.USAMPLER1DARRAY
                Token.SAMPLER2DRECT
                Token.SAMPLER2DRECTSHADOW
                Token.ISAMPLER2DRECT
                Token.USAMPLER2DRECT
                Token.SAMPLERBUFFER
                Token.ISAMPLERBUFFER
                Token.USAMPLERBUFFER
                Token.SAMPLER2DMS
                Token.ISAMPLER2DMS
                Token.USAMPLER2DMS
                Token.SAMPLER2DMSARRAY
                Token.ISAMPLER2DMSARRAY
                Token.USAMPLER2DMSARRAY
                Token.IMAGE2D
                Token.IIMAGE2D
                Token.UIMAGE2D
                Token.IMAGE3D
                Token.IIMAGE3D
                Token.UIMAGE3D
                Token.IMAGECUBE
                Token.IIMAGECUBE
                Token.UIMAGECUBE
                Token.IMAGEBUFFER
                Token.IIMAGEBUFFER
                Token.UIMAGEBUFFER
                Token.IMAGE1D
                Token.IIMAGE1D
                Token.UIMAGE1D
                Token.IMAGE1DARRAY
                Token.IIMAGE1DARRAY
                Token.UIMAGE1DARRAY
                Token.IMAGE2DRECT
                Token.IIMAGE2DRECT
                Token.UIMAGE2DRECT
                Token.IMAGE2DARRAY
                Token.IIMAGE2DARRAY
                Token.UIMAGE2DARRAY
                Token.IMAGECUBEARRAY
                Token.IIMAGECUBEARRAY
                Token.UIMAGECUBEARRAY
                Token.IMAGE2DMS
                Token.IIMAGE2DMS
                Token.UIMAGE2DMS
                Token.IMAGE2DMSARRAY
                Token.IIMAGE2DMSARRAY
                Token.UIMAGE2DMSARRAY
            ]
        let tokensP = List.map (fun t -> tokenP t) tokens |> choice |>> ignore
        
        tokensP <|> (identifierP |>> ignore) (*TYPE_NAME*) <|> structSpecifierP

    and arraySpecifierP =
        let singular =
            (leftBracketP .>>. rightBracketP |>> ignore) 
            <|> (leftBracketP .>>. constantExpressionP .>>. rightBracketP |>> ignore) 
        
        many1 singular |>> ignore
    
    and typeSpecifierP =
        (typeSpecifierNonArrayP |>> ignore)
        <|> (typeSpecifierNonArrayP .>>. arraySpecifierP |>> ignore)
    
    and typeNameListP =
        (identifierP |>> ignore) .>>. many (commaP .>>. identifierP |>> ignore) 
    
    and storageQualifierP =
        let tokens = [
            Token.CONST
            Token.IN
            Token.OUT
            Token.INOUT
            Token.CENTROID
            Token.PATCH
            Token.SAMPLE
            Token.UNIFORM
            Token.BUFFER
            Token.SHARED
            Token.COHERENT
            Token.VOLATILE
            Token.RESTRICT
            Token.READONLY
            Token.WRITEONLY
            Token.SUBROUTINE
        ]
        let tokensP = List.map tokenP tokens |> choice |>> ignore
        tokensP <|> (tokenP Token.SUBROUTINE .>>. leftParenP .>>. typeNameListP .>>. rightParenP |>> ignore)
    
    and layoutQualifierIdP =
        (identifierP |>> ignore)
        <|> (identifierP .>>. tokenP Token.EQUAL .>>. constantExpressionP |>> ignore)
        <|> (tokenP Token.SHARED |>> ignore)
    
    and layoutQualifierIdListP =
        layoutQualifierIdP .>>. (many (commaP .>>. layoutQualifierIdP)) |>> ignore
    
    and layoutQualifierP =
        (tokenP Token.LAYOUT) .>>. leftParenP .>>. layoutQualifierIdListP .>>. rightParenP |>> ignore
    
    and interpolationQualifierP =
        (tokenP Token.SMOOTH |>> ignore)
        <|> (tokenP Token.FLAT |>> ignore)
        <|> (tokenP Token.NOPERSPECTIVE |>> ignore)
    
    and invariantQualifierP = tokenP Token.INVARIANT |>> ignore
    
    and preciseQualifierP = tokenP Token.PRECISE |>> ignore
    
    and singleTypeQualifierP =
        storageQualifierP
        <|> layoutQualifierP
        <|> precisionQualifierP
        <|> interpolationQualifierP
        <|> invariantQualifierP
        <|> preciseQualifierP
    
    and typeQualifierP =
        many1 singleTypeQualifierP |>> ignore

    
    and fullySpecifiedTypeP =
        (typeSpecifierP |>> ignore)
        <|> (typeQualifierP .>>. typeSpecifierP |>> ignore)
    
    and functionHeaderP =
        fullySpecifiedTypeP .>>. identifierP .>>. leftParenP
    
    and parameterDeclarationP = failwith "TODO"
    
    and functionHeaderWithParametersP =
        (functionHeaderP .>>. parameterDeclarationP) .>>. many (commaP .>>. parameterDeclarationP)
    
    and functionDeclaratorP =
        (functionHeaderP |>> ignore) <|> (functionHeaderWithParametersP |>> ignore)
    
    and functionPrototypeP =
        functionDeclaratorP .>>. rightParenP
    
    and statementListP = many1 statementPRef.Parser |>> ignore
    
    and compoundStatementP =
        (leftBraceP .>>. rightBraceP |>> ignore)
        <|> (leftBraceP .>>. statementListP .>>. rightBraceP |>> ignore)
    
    and initDeclaratorListP = failwith "TODO"
    
    and precisionP = tokenP GlslParser.Tokenizer.Token.PRECISION
    
    and precisionQualifierP = failwith "todo"
    
    and arraySpecifierP = failwith "todo"
    
    and identifierListP = many1 (commaP .>>. identifierP) |>> ignore
    
    and declarationP =
        (functionPrototypeP .>>. semicolonP |>> ignore)
        <|> (initDeclaratorListP .>>. semicolonP |>> ignore)
        <|> (precisionP .>>. precisionQualifierP .>>. typeSpecifierP .>>. semicolonP |>> ignore)
        <|> (typeQualifierP .>>. identifierP .>>. leftBraceP .>>. structDeclarationListP .>>. rightBraceP .>>. semicolonP |>> ignore)
        <|> (typeQualifierP .>>. identifierP .>>. leftBraceP .>>. structDeclarationListP .>>. rightBraceP .>>. identifierP .>>. semicolonP |>> ignore)
        <|> (typeQualifierP .>>. identifierP .>>. leftBraceP .>>. structDeclarationListP .>>. rightBraceP .>>. identifierP |>> ignore)
        <|> (arraySpecifierP .>>. semicolonP |>> ignore)    
        <|> (typeQualifierP .>>. semicolonP |>> ignore)    
        <|> (typeQualifierP .>>. identifierP .>>. semicolonP |>> ignore)    
        <|> (typeQualifierP .>>. identifierP .>>. identifierListP .>>. semicolonP |>> ignore)    
    
    and declarationStatementP = declarationP
    
    and variableIdentifierP = identifierP
    
    and intConstantP =
        let pred token =
            match token with
            | Token.INTCONSTANT _ -> true
            | _ -> false
        satisfy pred "INTCONSTANT"
    
    
    and uintConstantP =
        let pred token =
            match token with
            | Token.UINTCONSTANT _ -> true
            | _ -> false
        satisfy pred "UINTCONSTANT"

    
    and floatConstant =
        let pred token =
            match token with
            | Token.FLOATCONSTANT _ -> true
            | _ -> false
        satisfy pred "FLOATCONSTANT"
        
        
    and doubleConstant =
        let pred token =
            match token with
            | Token.DOUBLECONSTANT _ -> true
            | _ -> false
        satisfy pred "DOUBLECONSTANT"

    and primaryExpressionP =
        (variableIdentifierP |>> ignore)
        <|> (intConstantP |>> ignore)
        <|> (uintConstantP |>> ignore)
        <|> (floatConstant |>> ignore)
        <|> (doubleConstant |>> ignore)
        <|> (leftParenP .>>. expressionPRef.Parser .>>. rightParenP |>> ignore)
    
    and functionIdentifierP =
        typeSpecifierP
        <|> postfixExpressionPRef.Parser
    
    and functionCallHeaderP =
        functionIdentifierP .>>. leftParenP |>> ignore
    
    and functionCallHeaderNoParametersP =
        functionCallHeaderP .>>. tokenP Token.VOID |>>  ignore
        <|> functionCallHeaderP
        
    and functionCallHeaderWithParametersP = 
        functionCallHeaderP .>>. assignmentExpressionPRef.Parser |>> ignore
        <|> functionCallHeaderWithParametersPRef.Parser .>>. commaP .>>. assignmentExpressionPRef.Parser |>> ignore
    
    and functionCallGenericP =
        functionCallHeaderWithParametersPRef.Parser .>>. rightParenP |>> ignore
        <|> functionCallHeaderNoParametersP .>>. rightParenP |>> ignore
        
    and functionCallOrMethodP = functionCallGenericP
    
    and functionCallP = functionCallOrMethodP 
    
    and integerExpressionP = expressionPRef.Parser
    
    and postfixExpressionP =
        primaryExpressionP
        <|> (postfixExpressionPRef.Parser .>>. leftBracketP .>>. integerExpressionP .>>. rightBracketP |>> ignore)
        <|> functionCallP
        <|> (postfixExpressionPRef.Parser .>>. (tokenP Token.DOT |>> ignore) .>>. (tokenP Token.FIELDSELECTION) |>> ignore)
        <|> (postfixExpressionPRef.Parser .>>. tokenP Token.INCOP |>> ignore)
        <|> (postfixExpressionPRef.Parser .>>. tokenP Token.DECOP |>> ignore)
    
    and unaryOperatorP =
        tokenP Token.PLUS
        <|> tokenP Token.DASH
        <|> tokenP Token.BANG
        <|> tokenP Token.TILDE |>> ignore
    
    and unaryExpressionP =
        (postfixExpressionP |>> ignore)
        <|> (tokenP Token.INCOP .>>. unaryExpressionPRef.Parser |>> ignore)
        <|> (tokenP Token.DECOP .>>. unaryExpressionPRef.Parser |>> ignore)
        <|> (unaryOperatorP .>>. unaryExpressionPRef.Parser |>> ignore)
        
    and multiplicativeExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and multiplicativeExpressionP =
        unaryExpressionP
        <|> (multiplicativeExpressionPRef.Parser .>>. tokenP Token.STAR .>>. unaryExpressionP |>> ignore)
        <|> (multiplicativeExpressionPRef.Parser .>>. tokenP Token.SLASH .>>. unaryExpressionP |>> ignore)
        <|> (multiplicativeExpressionPRef.Parser .>>. tokenP Token.PERCENT .>>. unaryExpressionP |>> ignore)
        
    and additiveExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and additiveExpressionP =
        multiplicativeExpressionP
        <|> (additiveExpressionPRef.Parser .>>. tokenP Token.PLUS .>>. multiplicativeExpressionP |>> ignore)
        <|> (additiveExpressionPRef.Parser .>>. tokenP Token.DASH .>>. multiplicativeExpressionP |>> ignore)

    and shiftExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and shiftExpressionP =
        additiveExpressionP
        <|> (shiftExpressionPRef.Parser .>>. tokenP Token.LEFTOP .>>. additiveExpressionP |>> ignore)
        <|> (shiftExpressionPRef.Parser .>>. tokenP Token.RIGHTOP .>>. additiveExpressionP |>> ignore)
    
    and relationalExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and relationalExpressionP =
        shiftExpressionP
        <|> (relationalExpressionPRef.Parser .>>. tokenP Token.LEFTANGLE .>>. shiftExpressionP |>> ignore)
        <|> (relationalExpressionPRef.Parser .>>. tokenP Token.RIGHTANGLE .>>. shiftExpressionP |>> ignore)
        <|> (relationalExpressionPRef.Parser .>>. tokenP Token.LEOP .>>. shiftExpressionP |>> ignore)
        <|> (relationalExpressionPRef.Parser .>>. tokenP Token.GEOP .>>. shiftExpressionP |>> ignore)

    and equalityExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and equalityExpressionP =
        relationalExpressionP
        <|> (equalityExpressionPRef.Parser .>>. tokenP Token.EQOP .>>. relationalExpressionP |>> ignore)
        <|> (equalityExpressionPRef.Parser .>>. tokenP Token.NEOP .>>. relationalExpressionP |>> ignore)
    
    and andExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and andExpressionP =
        equalityExpressionP
        <|> (andExpressionPRef.Parser .>>. tokenP Token.AMPERSAND .>>. equalityExpressionP |>> ignore) 

    and exclusiveOrExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and exclusiveOrExpressionP =    
        andExpressionP
        <|> (exclusiveOrExpressionPRef.Parser .>>. tokenP Token.CARET .>>. andExpressionP |>> ignore)
        
    and inclusiveOrExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and inclusiveOrExpressionP =
        exclusiveOrExpressionP
        <|> (inclusiveOrExpressionPRef.Parser .>>. tokenP Token.VERTICALBAR .>>. exclusiveOrExpressionP |>> ignore)

    and logicalAndExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and logicalAndExpressionP =
        inclusiveOrExpressionP
        <|> (logicalAndExpressionPRef.Parser .>>. tokenP Token.ANDOP .>>. inclusiveOrExpressionP |>> ignore) 

    and logicalXorExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and logicalXorExpressionP =
        logicalAndExpressionP
        <|> (logicalXorExpressionPRef.Parser .>>. tokenP Token.XOROP .>>. logicalAndExpressionP |>> ignore)

    and logicalOrExpressionPRef = ParserWithPositionalErrors.createRefParser ()
    and logicalOrExpressionP =
        logicalXorExpressionP
        <|> (logicalOrExpressionPRef.Parser .>>. tokenP Token.OROP .>>. logicalXorExpressionP |>> ignore)
        
    and conditionalExpressionP =
        logicalOrExpressionP
        <|> (logicalOrExpressionP .>>. tokenP Token.QUESTION .>>. expressionPRef.Parser .>>. tokenP Token.COLON .>>. assignmentExpressionPRef.Parser |>> ignore)

    and assignmentExpressionP =
        conditionalExpressionP
        <|> (unaryExpressionP .>>. assignmentOpP .>>. assignmentExpressionP |>> ignore)
    
    and expressionP =
        assignmentExpressionP .>>. (many (commaP .>>. assignmentExpressionP)) |>> ignore
        
    and expressionStatementP =
        (semicolonP |>> ignore)
        <|> (expressionP .>>. semicolonP |>> ignore)

    and selectionRestStatementP =
        (statementP .>>. tokenP Token.ELSE .>>. statementP |>> ignore)
        <|> statementP
            
    and selectionStatementP =
        tokenP Token.IF .>>. leftParenP .>>. expressionP .>>. rightParenP .>>. selectionRestStatementP |>> ignore
    
    and switchStatementListP = statementListP

    and switchStatementP =
        tokenP Token.SWITCH .>>. leftParenP .>>. expressionP .>>. rightParenP .>>. leftBraceP .>>. switchStatementListP .>>. rightBraceP |>> ignore
        
    and caseLabelP =
        (tokenP Token.CASE .>>. expressionP .>>. tokenP Token.COLON) |>> ignore
        <|> (tokenP Token.DEFAULT .>>. tokenP Token.COLON |>> ignore)
    
    and iterationStatementP =
        tokenP Token.WHILE .>>. leftParenP .>>. conditionP .>>. rightParenP .>>. statementNoNewScopeP
        DO statement WHILE LEFT_PAREN expression RIGHT_PAREN SEMICOLON
        FOR LEFT_PAREN for_init_statement for_rest_statement RIGHT_PAREN statement_no_new_scope
        
    
    and jumpStatementP = failwith "todo"
    
    and simpleStatementP =    
        declarationStatementP
        <|> expressionStatementP
        <|> selectionStatementP
        <|> switchStatementP
        <|> caseLabelP
        <|> iterationStatementP
        <|> jumpStatementP
        
    and statementP =
        compoundStatementP <|> simpleStatementP
    
    and statementListP =
        many1 statementP
    
    and compoundStatementNoNewScopeP =
        (tokenP GlslParser.Tokenizer.Token.LEFTBRACE .>>. tokenP GlslParser.Tokenizer.Token.RIGHTBRACE |>> ignore)
        <|>
        (tokenP GlslParser.Tokenizer.Token.LEFTBRACE .>>. statementListP .>>. tokenP GlslParser.Tokenizer.Token.RIGHTBRACE |>> ignore)

    
    and functionDefinitionP =
        functionPrototypeP .>>. compoundStatementNoNewScopeP

    and externalDeclarationP =
        //functionDefinitionP
        //declarationP
        semicolonP
        
    typeSpecifierPRef.Set typeSpecifierP
    statementPRef.Set statementP
    expressionPRef.Set expressionP



[<EntryPoint>]
let main argv =
    let src = """
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform float morphStart;
uniform float morphEnd;
uniform vec3 cameraPosition;
uniform vec2 quadScale;
uniform sampler2D heightMap;

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;

out vec3 vNormal;
out vec3 vPosition;

vec2 g_gridDim = vec2(64.0, 64.0);

vec2 morphVertex(vec2 gridPos, vec2 vertex, float morphK)
{
	vec2 fracPart = fract(gridPos.xy * g_gridDim.xy * 0.5) * 2.0 / g_gridDim.xy;
	return vertex.xy - fracPart * quadScale * morphK;
} 

void main()
{
	vec4 worldPosition = modelMatrix * vec4(position, 1.0);
	float height = texture2D(heightMap, (worldPosition.xz / 2048.0) + 0.5).x;
	worldPosition.y = height * 100;
	float l = length(worldPosition.xyz - cameraPosition);
	float clamped = clamp(l - morphStart, morphStart, morphEnd);
	float a = (clamped - morphStart) /  (morphEnd - morphStart);
 	vec2 morphedPos = morphVertex(position.xz + vec2(0.5),  worldPosition.xz, a);
	gl_Position = projectionMatrix * viewMatrix * vec4(morphedPos.x, texture2D(heightMap, (morphedPos / 2048.0) + 0.5).x * 100, morphedPos.y, 1.0);
    vPosition = position;
    vNormal = normal;
}


"""
    
//    CharParsers.run (GlslParser.Tokenizer.tokenP) "123" |> printfn "%A"
//    CharParsers.run (GlslParser.Tokenizer.tokenP) "123u" |> printfn "%A"
//    CharParsers.run (GlslParser.Tokenizer.tokenP) "0123" |> printfn "%A"
//    CharParsers.run (GlslParser.Tokenizer.tokenP) "1.5" |> printfn "%A"
//    CharParsers.run (GlslParser.Tokenizer.tokenP) "2.0LF" |> printfn "%A"
//    CharParsers.run (GlslParser.Tokenizer.tokensP) "1" |> printfn "%A"
    GlslParser.Tokenizer.parse src |> printfn "%A"
    
    printfn "Hello World from F#!"
    0 // return an integer exit code
