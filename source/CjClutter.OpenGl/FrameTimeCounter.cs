namespace CjClutter.OpenGl
{
    public class FrameTimeCounter
    {
        private double _frameTime = 0;

        public void UpdateFrameTime(double newFrameTime)
        {
            _frameTime = (_frameTime + newFrameTime) / 2;
        }

        public void Reset()
        {
            _frameTime = 0;
        }

        public double FrameTime
        {
            get { return _frameTime; }
        }

        public double Fps
        {
            get { return 1/_frameTime; }
        }
    }

    public static class FrameTimeCounterExtensions
    {
        public static string ToOutputString(this FrameTimeCounter frameTimeCounter)
        {
            return string.Format(
                "FPS: {0:#}\r\nFrame time: {1:#.###}ms",
                frameTimeCounter.Fps,
                frameTimeCounter.FrameTime * 1000);
        }
    }
}