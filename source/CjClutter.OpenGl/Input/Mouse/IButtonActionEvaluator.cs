using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Mouse
{
    public interface IButtonActionEvaluator
    {
        bool ShouldButtonActionBeFired(MouseButton button);
    }
}
