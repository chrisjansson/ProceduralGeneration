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

let drawMesh (m:Rendering.AllocatedMesh) (primitiveType:PrimitiveType) =
    m.bind()
    GL.DrawElements(BeginMode.Triangles, m.faces * 3, DrawElementsType.UnsignedInt, 0);

let clamp min max v =
    match v with
    | _ when v < min -> min
    | _ when v > max -> max
    | _ -> v

type ShaderProgram =
    | SimpleShaderProgram of SimpleShaderProgram.SimpleProgram
    | NormalDebugShaderProgram of NormalDebugShaderProgram.SimpleProgram
    | BlinnShaderProgram of BlinnShaderProgram.BlinnPhongProgram

let render program renderJob =
    match program with
    | SimpleShaderProgram p ->
        GL.UseProgram p.ProgramId
        p.ProjectionMatrixUniform.set renderJob.StaticContext.ProjectionMatrix
        p.ViewMatrix.set renderJob.StaticContext.ViewMatrix
    | BlinnShaderProgram p ->
        GL.UseProgram p.ProgramId
        p.ProjectionMatrixUniform.set renderJob.StaticContext.ProjectionMatrix
        p.ViewMatrix.set renderJob.StaticContext.ViewMatrix
        match renderJob.Material with
        | Blinn m -> 
            p.AmbientColor.set m.AmbientColor
            p.DiffuseColor.set m.DiffuseColor
            p.SpecularColor.set m.SpecularColor
            for j in renderJob.RenderJobs do
                p.ModelMatrix.set j.Mesh.renderContext.ModelMatrix
                p.NormalMatrix.set j.Mesh.renderContext.NormalMatrix
                drawMesh j.Mesh PrimitiveType.Triangles
        | _ -> ()
    | NormalDebugShaderProgram p ->
        GL.UseProgram p.ProgramId
        p.ProjectionMatrixUniform.set renderJob.StaticContext.ProjectionMatrix
        p.ViewMatrix.set renderJob.StaticContext.ViewMatrix

let configureTweakBar c defaultValue =
    let bar = new Bar(c)
    bar.Size <- new Size(300, bar.Size.Height)
    bar.Label <- "Stuff"
    bar.Contained <- true
    makeViewModel bar defaultValue
        
let makeRenderJob mesh cameraMatrix =
    let translation = Matrix4.Identity
    let (modelToProjection:Matrix4) = translation * cameraMatrix;
    let normalMatrix = new Matrix3(Matrix4.Transpose(modelToProjection.Inverted()))
    {
        Mesh = mesh
        IndividualContext = {
                            ModelMatrix = translation
                            NormalMatrix = normalMatrix
            }
        }

let fsaaSamples = 8
let windowGraphicsMode =
    new Graphics.GraphicsMode(
            Graphics.GraphicsMode.Default.ColorFormat, 
            Graphics.GraphicsMode.Default.Depth, 
            Graphics.GraphicsMode.Default.Stencil,
            fsaaSamples)
type FysicsWindow() = 
    inherit GameWindow(800, 600, windowGraphicsMode) 

    [<DefaultValue>] val mutable tweakbarContext : Context
    [<DefaultValue>] val mutable program : ShaderProgram
    [<DefaultValue>] val mutable program2 : ShaderProgram
    [<DefaultValue>] val mutable blinn : System.IObservable<BlinnMaterial>
    let defaultBlinnMaterial = { 
        AmbientColor = new Vector3(0.1f, 0.1f, 0.1f); 
        DiffuseColor = new Vector3(0.4f, 0.7f, 0.4f); 
        SpecularColor = new Vector3(1.0f, 1.0f, 1.0f); 
        SpecularExp = 150.0 }
    let defaultVm = { IntegrationSpeed = 1.0; BlinnMaterial = defaultBlinnMaterial }
    let tree = makeTerrainLodTree
    let nodeCache = makeCache allocate

    let mutable vm : ViewModel = defaultVm
    let terrain = new CjClutter.OpenGl.Terrain(new LOD.ChunkedLod())
    let camera = new CjClutter.OpenGl.Camera.LookAtCamera()
    let lodCamera = new CjClutter.OpenGl.Camera.LookAtCamera()
    let factory = new CjClutter.OpenGl.TerrainChunkFactory()
    let mutable synchronizeCameras = true
    let mutable nodesInCache = 0
    let mutable cacheSize = 10000000

    override this.OnLoad(e) =
        this.program <- BlinnShaderProgram BlinnShaderProgram.makeBlinnShaderProgram
        this.program2 <- NormalDebugShaderProgram NormalDebugShaderProgram.makeSimpleShaderProgram
        this.tweakbarContext <- new Context(Tw.GraphicsAPI.OpenGL)
        let vmObs = configureTweakBar this.tweakbarContext defaultVm
        vmObs.Subscribe(fun m -> vm <- m) |> ignore
        GL.LineWidth(1.0f)
        GL.ClearColor(Color.WhiteSmoke)
        GL.Enable(EnableCap.DepthTest)
        this.VSync <- VSyncMode.On

        let startWorkerThread _ =
            this.Context.MakeCurrent(null)
            let contextReady = new System.Threading.AutoResetEvent(false)
            let t = new System.Threading.ParameterizedThreadStart(fun o -> 
                    let window = new OpenTK.NativeWindow()
                    let context = new OpenTK.Graphics.GraphicsContext(this.Context.GraphicsMode, window.WindowInfo)
                    context.MakeCurrent(window.WindowInfo)
                    contextReady.Set() |> ignore

                    while true do
                        let work = CjClutter.OpenGl.Gui.JobDispatcher.Instance.Dequeue()
                        work.Invoke()
                )

            let thread = new System.Threading.Thread(t)
            thread.IsBackground <- true
            thread.Start()
            contextReady.WaitOne() |> ignore
            this.MakeCurrent() |> ignore

        startWorkerThread () |> ignore
        startWorkerThread () |> ignore
        startWorkerThread () |> ignore
        startWorkerThread () |> ignore

    override this.OnClosing(e) =
        this.tweakbarContext.Dispose()

    override this.OnUpdateFrame(e) =
        if this.Keyboard.[Key.Escape] then do
            this.Exit()
        if this.Keyboard.[Key.A] then do
            camera.Position <- Vector3d.Transform(camera.Position, Matrix4d.CreateRotationY(-e.Time))
        if this.Keyboard.[Key.D] then do
            camera.Position <- Vector3d.Transform(camera.Position, Matrix4d.CreateRotationY(e.Time))
        if this.Keyboard.[Key.W] then do
            camera.Position <- camera.Position - camera.Position * (e.Time)
        if this.Keyboard.[Key.S] then do
            camera.Position <- camera.Position + camera.Position * (e.Time)

    override this.OnKeyUp(e) =
        match e.Key with
        | Key.N -> synchronizeCameras <- false
        | Key.M -> synchronizeCameras <- true
        | Key.Space -> cacheSize <- cacheSize + 1
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

        let frustum = CjClutter.OpenGl.Camera.FrustumPlaneExtractor.ExtractRowMajor(lodCamera)
        let visibleNodes = findVisibleNodes tree frustum (float this.Width) lodCamera.HorizontalFieldOfView lodCamera.Position 20.0
        let (nodesToDraw, nodesToCache) = getNodesToDrawAndCache nodeCache visibleNodes

        let takeMax n array =
            let elementsToTake = min (Array.length array) n
            Array.take elementsToTake array

        for n in nodesToCache |> Array.sortBy(fun n -> n.GeometricError) |> Array.rev |> takeMax (cacheSize - nodesInCache) do
            nodesInCache <- nodesInCache + 1
            nodeCache.beginCache n

        let blinnMaterial = vm.BlinnMaterial

        let staticRenderContext = {
                ProjectionMatrix = projectionMatrix
                ViewMatrix = cameraMatrix
            }

        let renderJob = {
                StaticContext = staticRenderContext
                RenderJobs = nodesToDraw |> Array.map (fun n -> nodeCache.get n) |> Array.map (fun m -> makeRenderJob m cameraMatrix) |> Array.toList
                Material = Blinn({ Rendering.BlinnMaterial.AmbientColor = blinnMaterial.AmbientColor; DiffuseColor = blinnMaterial.DiffuseColor; SpecularColor = blinnMaterial.SpecularColor})
            }

        render this.program renderJob

        this.tweakbarContext.Draw()

        this.SwapBuffers();

[<EntryPoint>]
let main argv =
    let window = new FysicsWindow()
    window.VSync <- VSyncMode.On
    window.Run()
    0 // return an integer exit code
