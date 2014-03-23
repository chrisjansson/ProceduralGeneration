using System;
using CjClutter.OpenGl.Gui;

namespace ObjLoader.Test
{
    class OpenGlTestProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            const string title = "OpenGl test program";
            Console.WriteLine(title);

            var window = new OpenGlWindow(800, 600, title, OpenGlVersion.OpenGl31);

            window.Run();
        }

    }
}
