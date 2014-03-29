using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenGl.VertexTypes;

namespace CjClutter.OpenGl.SceneGraph
{
    public class MeshResources
    {
        public SimpleRenderProgram RenderProgram { get; set; }
        public RenderableMesh RenderableMesh { get; set; }
        public RenderableMesh Back { get; set; }
        public NormalDebugProgram NormalDebugProgram { get; set; }
    }
}