open primitives
open shader
open Rendering
open OpenTK
open OpenTK.Input
open OpenTK.Graphics.OpenGL
open System.Drawing
open AntTweakBar
open TweakBar
open TweakBarGui
open BlinnMaterialTweakBar
open TweakBarGuiViewModel
open System.Reactive.Linq
open System.Linq
open LOD
open LODCache
open Terrain
open Input
open CDLodOpenGl
open System.Runtime.InteropServices

let drawMesh (m:Rendering.RenderableMesh) (primitiveType:PrimitiveType) =
    m.Bind()
    match primitiveType with
    | PrimitiveType.Points -> GL.DrawArrays(PrimitiveType.Points, 0, m.Faces * 3)
    | PrimitiveType.Triangles -> GL.DrawElements(BeginMode.Triangles, m.Faces * 3, DrawElementsType.UnsignedShort, 0)
    | _ -> ()

type ShaderProgram =
    | SimpleShaderProgram of SimpleShaderProgram.SimpleProgram
    | NormalDebugShaderProgram of NormalDebugShaderProgram.SimpleProgram
    | BlinnShaderProgram of BlinnShaderProgram.BlinnPhongProgram

let renderTerrain (p:BlinnShaderProgram.BlinnPhongProgram) staticContext (renderJobs:CDLodRenderJob seq) =
    GL.UseProgram p.ProgramId
    p.ProjectionMatrixUniform.set staticContext.ProjectionMatrix
    p.ViewMatrix.set staticContext.ViewMatrix
    for job in renderJobs do
        p.ModelMatrix.set job.ModelMatrix
        p.NormalMatrix.set job.NormalMatrix
        p.MorphStart.set job.MorphStart
        p.MorphEnd.set job.MorphEnd
        p.CameraPositon.set job.CameraPosition
        drawMesh job.Mesh PrimitiveType.Triangles
        

//
//let render program renderJob =
//    match program with
////    | SimpleShaderProgram p ->
////        GL.UseProgram p.ProgramId
////        p.ProjectionMatrixUniform.set renderJob.StaticContext.ProjectionMatrix
////        p.ViewMatrix.set renderJob.StaticContext.ViewMatrix
//    | BlinnShaderProgram p ->
//        GL.UseProgram p.ProgramId
//        p.ProjectionMatrixUniform.set renderJob.StaticContext.ProjectionMatrix
//        p.ViewMatrix.set renderJob.StaticContext.ViewMatrix
//
//    | _ -> failwith "Unrecognized program"
////        match renderJob.Material with
////        | Blinn m -> 
////            p.AmbientColor.set m.AmbientColor
////            p.DiffuseColor.set m.DiffuseColor
////            p.SpecularColor.set m.SpecularColor
////            for j in renderJob.RenderJobs do
////                p.ModelMatrix.set j.Mesh.renderContext.ModelMatrix
////                p.NormalMatrix.set j.Mesh.renderContext.NormalMatrix
////                drawMesh j.Mesh PrimitiveType.Triangles
////        | _ -> ()
////    | NormalDebugShaderProgram p ->
////        GL.UseProgram p.ProgramId
////        p.ProjectionMatrixUniform.set renderJob.StaticContext.ProjectionMatrix
////        p.ViewMatrix.set renderJob.StaticContext.ViewMatrix

let configureTweakBar c defaultValue =
    let bar = new Bar(c)
    bar.Size <- new Size(300, bar.Size.Height)
    bar.Label <- "Stuff"
    bar.Contained <- true
    makeViewModel bar defaultValue
        
let clamp (min) (max) (value) =
    match value with 
    | x when x < min -> min
    | x when x > max -> max
    | _ -> value

let fsaaSamples = 8
let windowGraphicsMode =
    new Graphics.GraphicsMode(
            Graphics.GraphicsMode.Default.ColorFormat,
            24, 
            Graphics.GraphicsMode.Default.Stencil,
            fsaaSamples)
type FysicsWindow() = 
    inherit GameWindow(800, 600, windowGraphicsMode, "Window", GameWindowFlags.Default, DisplayDevice.Default, 4, 3, Graphics.GraphicsContextFlags.Debug) 

    [<DefaultValue>] val mutable tweakbarContext : Context
    [<DefaultValue>] val mutable program : ShaderProgram
    [<DefaultValue>] val mutable program2 : ShaderProgram
    [<DefaultValue>] val mutable cdlodMesh : RenderableCDLodMesh
    let defaultBlinnMaterial = { 
        AmbientColor = new Vector3(0.1f, 0.1f, 0.1f); 
        DiffuseColor = new Vector3(0.4f, 0.7f, 0.4f); 
        SpecularColor = new Vector3(1.0f, 1.0f, 1.0f); 
        SpecularExp = 150.0 }
    let defaultVm = { IntegrationSpeed = 1.0; BlinnMaterial = defaultBlinnMaterial }
    let mutable vm : ViewModel = defaultVm
    let camera = new CjClutter.OpenGl.Camera.LookAtCamera()
    let lodCamera = new CjClutter.OpenGl.Camera.LookAtCamera()
    let factory = new CjClutter.OpenGl.TerrainChunkFactory()
    let keyboard = new CjClutter.OpenGl.Input.Keboard.KeyboardInputProcessor()
    let lodTree = QuadTree.makeXZQuadTree 5 { Min = { X = -4096.0; Y = 0.0; Z = -4096.0 }; Max = { X = 4096.0; Y = 0.0; Z = 4096.0; } }
    let lodRanges = LodRanges.makeLodRanges { LodRanges.LodSettings.LodDistanceRatio = 2.0; LodLevels = 6; VisibilityDistance = 4000.0; MorphStartRatio = 0.667 }

    let mutable synchronizeCameras = true

    override this.OnLoad(e) =
        this.program <- BlinnShaderProgram BlinnShaderProgram.makeBlinnShaderProgram
        this.program2 <- NormalDebugShaderProgram NormalDebugShaderProgram.makeSimpleShaderProgram
        this.tweakbarContext <- new Context(Tw.GraphicsAPI.OpenGLCore)
        let vmObs = configureTweakBar this.tweakbarContext defaultVm
        vmObs.Subscribe(fun m -> vm <- m) |> ignore
        GL.LineWidth(1.0f)
        GL.ClearColor(Color.WhiteSmoke)
        GL.Enable(EnableCap.DepthTest)
        
        this.VSync <- VSyncMode.On

        let version = GL.GetString(StringName.Version)

        this.cdlodMesh <- makeRenderableSquareXZMesh 64 0 1

        for i = 1 to 1 do
            BackgroundWorker.startWorkerThread this |> ignore

    override this.OnClosing(e) =
        this.tweakbarContext.Dispose()

    override this.OnUpdateFrame(e) =
        keyboard.Update(OpenTK.Input.Keyboard.GetState())
        if this.Keyboard.[Key.Escape] then do
            this.Exit()
        let transform = convert keyboard (e.Time / 200.0)
        applyTransform camera transform

    override this.OnKeyUp(e) =
        match e.Key with
        | Key.N -> synchronizeCameras <- false
        | Key.M -> synchronizeCameras <- true
        | _ -> ()
        match convertKeyEvent e with
        | Some keys -> 
            this.tweakbarContext.HandleKeyUp(keys.Key, keys.Modifiers) |> ignore
        | _ -> ()

    override this.OnKeyDown(e) =
        match convertKeyEvent e with
        | Some keys -> 
            this.tweakbarContext.HandleKeyDown(keys.Key, keys.Modifiers) |> ignore
        | None -> ()

    override this.OnKeyPress(e) =
        this.tweakbarContext.HandleKeyPress(e.KeyChar) |> ignore

    override this.OnMouseMove(e) =
        this.tweakbarContext.HandleMouseMove(new Point(e.X, e.Y)) |> ignore

    override this.OnMouseDown(e) =
        this.tweakbarContext.HandleMouseClick(Tw.MouseAction.Pressed, Tw.MouseButton.Left) |> ignore

    override this.OnMouseUp(e) =
        this.tweakbarContext.HandleMouseClick(Tw.MouseAction.Released, Tw.MouseButton.Left) |> ignore

    override this.OnResize(e) =
        GL.Viewport(0, 0, this.Width, this.Height)
        this.tweakbarContext.HandleResize(this.ClientSize)
        camera.Width <- float this.Width
        camera.Height <- float this.Height

    override this.OnRenderFrame(e) =
        if synchronizeCameras then do
            lodCamera.Position <- camera.Position
            lodCamera.Target <- camera.Target
            lodCamera.Up <- camera.Up
            lodCamera.Width <- camera.Width
            lodCamera.Height <- camera.Height

        GL.Clear(ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)

        let projectionMatrix = CjClutter.OpenGl.OpenTk.Matrix4dExtensions.ToMatrix4(camera.ComputeProjectionMatrix())
        let cameraMatrix = CjClutter.OpenGl.OpenTk.Matrix4dExtensions.ToMatrix4(camera.ComputeCameraMatrix())

        let detailTester (lodRanges:LodRanges.LodRange array) (cameraPosition:Vector3d) (node:LodSelect.Node) detailLevel = 
            match detailLevel with
            | level when detailLevel < 0 -> false 
            | _ ->    
                let range = lodRanges.[detailLevel]
                let sphere = { Math.Sphere.Center = { X = cameraPosition.X; Y = cameraPosition.Y; Z = cameraPosition.Z }; Radius = range.VisibilityRange }
                aabbSphereIntersects node.Bounds sphere

        let frustumTester _ = true

        let nodes = LodSelect.lodSelect frustumTester (detailTester lodRanges camera.Position) lodTree

        let camera = Vector3(float32 lodCamera.Position.X, float32 lodCamera.Position.Y, float32 lodCamera.Position.Z)
        let makeRenderJob node =
            match node with
            | LodSelect.Full node -> 
                let center = getBoundsCenter node.Bounds
                let size = getBoundsSize node.Bounds
                let scale = Matrix4.CreateScale(float32 size.X, float32 size.Y, float32 size.Z)
                let translation = Matrix4.CreateTranslation(float32 center.X, float32 center.Y, float32 center.Z)
                let mesh = { RenderableMesh.Bind = this.cdlodMesh.Bind; RenderableMesh.Faces = this.cdlodMesh.ElementCount / 3; }
                let lodRange = lodRanges.[node.LodLevel]
                let renderJob = { 
                        MorphStart = float32 lodRange.MorphStart
                        MorphEnd = float32 lodRange.MorphEnd
                        ModelMatrix = scale * translation
                        NormalMatrix = Matrix3.Identity
                        Mesh = mesh
                        CameraPosition = camera
                    }
                Some renderJob
            | _ -> None 

        let renderJobs = nodes |> List.choose (fun n -> makeRenderJob n)

        let frustum = CjClutter.OpenGl.Camera.FrustumPlaneExtractor.ExtractRowMajor(lodCamera)
        
        let blinnMaterial = vm.BlinnMaterial

        let staticRenderContext = {
                ProjectionMatrix = projectionMatrix
                ViewMatrix = cameraMatrix
            }

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line)


        match this.program with
        | BlinnShaderProgram p -> renderTerrain p staticRenderContext renderJobs
        | _ -> ()

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)

        this.tweakbarContext.Draw()
        this.SwapBuffers();

[<EntryPoint>]
let main argv =
    let window = new FysicsWindow()
    window.VSync <- VSyncMode.On
    window.Run()
    0 // return an integer exit code
