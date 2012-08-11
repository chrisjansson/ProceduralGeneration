using System;
using System.Diagnostics;
using CjClutter.OpenGl;
using CjClutter.OpenGl.Noise;

namespace ObjLoader.Test
{
    class OpenGlTestProgram
    {
        static void Main(string[] args)
        {
            const string title = "OpenGl test program";
            Console.WriteLine(title);

            var improvedPerlinNoise = new ImprovedPerlinNoise();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                var noise = improvedPerlinNoise.Noise(i, i, i);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

            stopwatch.Reset();

            var simplexNoise = new SimplexNoise();
            stopwatch.Start();

            for (int i = 0; i < 10000000; i++)
            {
                var noise = simplexNoise.Noise(i, i);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);

            //var window = new OpenGlWindow(800, 600, title, OpenGlVersion.OpenGl31);

            //window.Run();
        }
    }
}
