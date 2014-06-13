namespace CjClutter.OpenGl.Noise
{
    public class DiamondSquare
    {
        public double[] Generate(double h0, double h1, double h2, double h3, int levels)
        {
            var old = new[] { h0, h1, h2, h3 };
            var result = old;

            for (var i = 0; i < levels; i++)
            {
                Subdivide(old, i);


            }

            return result;
        }

        private void Subdivide(double[] old, int level)
        {

        }
    }
}