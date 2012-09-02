using System;
using System.Threading.Tasks;
using CjClutter.OpenGl;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.Noise;

namespace ObjLoader.Test
{
    class OpenGlTestProgram
    {
        static void Main(string[] args)
        {
            const string title = "OpenGl test program";
            Console.WriteLine(title);

            var window = new OpenGlWindow(800, 600, title, OpenGlVersion.OpenGl31);

            window.Run();
        }

    }
}
