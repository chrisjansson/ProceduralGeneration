module BackgroundWorker
open OpenTK
open OpenTK.Graphics
open System.Threading

let private createNewContext (owner:GameWindow) =
    let newWindow = new NativeWindow()
    let context = new GraphicsContext(owner.Context.GraphicsMode, newWindow.WindowInfo)
    (newWindow, context)

let work _ =
    let work = CjClutter.OpenGl.Gui.JobDispatcher.Instance.Dequeue()
    work.Invoke()
        
let startWorkerThread (owner:GameWindow) =
        let contextReady = new AutoResetEvent(false)
        owner.Context.MakeCurrent(null)


        let t = ParameterizedThreadStart(fun o -> 

                let (w, c) = createNewContext owner
                c.MakeCurrent(w.WindowInfo)
                contextReady.Set() |> ignore

                while w.Exists do
                    w.ProcessEvents()
                    work () |> ignore
            )

        let thread = Thread(t)
        thread.IsBackground <- true
        thread.Start()
        contextReady.WaitOne() |> ignore
        owner.MakeCurrent()

