using OpenTK;

namespace CjClutter.OpenGl.CoordinateSystems
{
    public interface IGuiToRelativeCoordinateTransformer
    {
        Vector2d TransformToRelative(Vector2d absoluteCoordinate);
        IInterfaceSize Interface { get; set; }
    }
}