using OpenTK;

namespace CjClutter.OpenGl.CoordinateSystems
{
    public class GuiToRelativeCoordinateTransformer : IGuiToRelativeCoordinateTransformer
    {
        public IInterfaceSize Interface { get; set; }

        public Vector2d TransformToRelative(Vector2d absoluteCoordinate)
        {
            var x = absoluteCoordinate.X / Interface.Width * 2 - 1;
            var y = (Interface.Height - absoluteCoordinate.Y) / Interface.Height * 2 - 1;

            return new Vector2d(x, y);
        }
    }
}