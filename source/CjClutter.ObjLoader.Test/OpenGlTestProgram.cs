using System;
using CjClutter.OpenGl;

namespace ObjLoader.Test
{
    class OpenGlTestProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("OpenGl test program");

            var window = new OpenGlWindow();

            window.Run();
        }
    }
}
