using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class PositionalLightComponent : IEntityComponent
    {
        public Vector3d Position { get; set; }
    }
}