
using System.Windows.Forms;

namespace CjClutter.ObjLoader.Viewer.CoordinateSystems
{
    public class ControlInterfaceSizeAdapter : IInterfaceSize
    {
        public Control Control { get; set; }

        public double Width
        {
            get { return Control.Width; }
        }

        public double Height
        {
            get { return Control.Height; }
        }
    }
}