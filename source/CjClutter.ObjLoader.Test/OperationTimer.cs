using System;
using System.Diagnostics;

namespace ObjLoader.Test
{
    public class OperationTimer
    {
        public TimeSpan TimeOperation(Action operation)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            operation();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}