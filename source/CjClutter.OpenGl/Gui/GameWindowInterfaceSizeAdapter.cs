using CjClutter.ObjLoader.Viewer.CoordinateSystems;
using OpenTK;

namespace CjClutter.OpenGl.Gui
{
    public class GameWindowInterfaceSizeAdapter : IInterfaceSize
    {
        public GameWindow GameWindow { get; set; }

        public double Width
        {
            get { return GameWindow.Width; }
        }

        public double Height
        {
            get { return GameWindow.Height; }
        }
    }
}