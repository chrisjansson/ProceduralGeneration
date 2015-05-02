module Input
open OpenTK
open OpenTK.Input
open CjClutter.OpenGl.Input.Keboard

type Keyboard = KeyboardInputProcessor

let forwardKey = Key.W
let backwardKey = Key.S
let leftKey = Key.D
let rightKey = Key.A
let zero = Vector3d.Zero

let translateOnKey (keyboard:Keyboard) key direction =
    match keyboard.IsButtonDown key with
    | true -> direction
    | false -> zero

let forward keyboard = translateOnKey keyboard forwardKey -Vector3d.UnitZ
let backward keyboard = translateOnKey keyboard backwardKey Vector3d.UnitZ
let left keyboard = translateOnKey keyboard leftKey Vector3d.UnitX
let right keyboard = translateOnKey keyboard rightKey -Vector3d.UnitX

let convert (keyboard:Keyboard) (dt:float) =
    let translation = 
        (forward keyboard + backward keyboard + left keyboard + right keyboard) * dt
    Matrix4d.CreateTranslation(translation)

type Camera = CjClutter.OpenGl.Camera.ICamera

let applyTransform (camera:Camera) (transform:Matrix4d) =
    let inverseCameraMatrix = camera.ComputeCameraMatrix().Inverted()
    let newCameraMatrix = transform * inverseCameraMatrix
    let cameraMatrix = camera.ComputeCameraMatrix()
    camera.Position <- Vector3d.Transform(Vector3d.Transform(camera.Position, cameraMatrix), newCameraMatrix)
    camera.Target <- Vector3d.Transform(Vector3d.Transform(camera.Target, cameraMatrix), newCameraMatrix)
