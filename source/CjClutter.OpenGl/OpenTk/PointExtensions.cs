using System.Drawing;
using OpenTK;

namespace CjClutter.OpenGl.OpenTk
{
    public static class PointExtensions
    {
         public static Vector2d ToVector(this Point point)
         {
             return new Vector2d(point.X, point.Y);
         }
    }
}