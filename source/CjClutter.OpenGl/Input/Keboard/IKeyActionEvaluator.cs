using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Keboard
{
    public interface IKeyActionEvaluator
    {
        bool ShouldKeyActionBeFired(Key key);
    }
}