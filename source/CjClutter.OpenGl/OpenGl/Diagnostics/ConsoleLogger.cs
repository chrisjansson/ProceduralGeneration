using System;
using System.Diagnostics;

namespace CjClutter.OpenGl.OpenGl.Diagnostics
{
    public interface ILogger
    {
        void Warn(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Warn(string message)
        {
            var time = DateTime.Now;
            var formattedMessage = string.Format("{0} Warning: {1}", time, message);
        
            Debug.WriteLine(formattedMessage);
        }
    }
}