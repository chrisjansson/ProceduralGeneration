using System;
using System.Threading.Tasks;
using CjClutter.OpenGl.Noise;

namespace ObjLoader.Test
{
    class TimingTestProgram
    {
        private const int NumberOfCalculations = 1000000;
        private const int Iterations = 10;

        static void Main(string[] args)
        {
            var noise = new SimplexNoise();

            var operationTimer = new OperationTimer();
            var elapsedTime = operationTimer.TimeOperation(() =>
                                                               {
                                                                   const int upperLimit = NumberOfCalculations * Iterations;
                                                                   for (int i = 0; i < upperLimit; i++)
                                                                   {
                                                                       var d = noise.Noise(i, i, i);
                                                                   }
                                                               });

            Console.WriteLine(elapsedTime.TotalSeconds);

            elapsedTime = operationTimer.TimeOperation(() => Parallel.For(0, Iterations, CalculateNoise));
            Console.WriteLine(elapsedTime.TotalSeconds);
        }


        private static void CalculateNoise(int i)
        {
            var simplexNoise = new SimplexNoise();

            for (int j = 0; j < NumberOfCalculations; j++)
            {
                var noise = simplexNoise.Noise(i, i, i);
            }
        }
    }
}
