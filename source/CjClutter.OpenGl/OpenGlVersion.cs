namespace CjClutter.OpenGl
{
    public class OpenGlVersion
    {
        public static OpenGlVersion OpenGl31 = new OpenGlVersion(3, 1);
        
        public OpenGlVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public int Major { get; private set; }
        public int Minor { get; private set; }
    }
}