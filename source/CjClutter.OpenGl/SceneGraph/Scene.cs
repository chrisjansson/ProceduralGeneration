using System;
using System.Collections.Generic;
using CjClutter.OpenGl.Noise;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Scene
    {
        public Scene()
        {
            SceneObjects = new List<SceneObject>();
        }

        public Matrix4d ViewMatrix { get; set; }
        public Matrix4d ProjectionMatrix { get; set; }

        public List<SceneObject> SceneObjects { get; set; }

        public void Reload(FractalBrownianMotionSettings fractalBrownianMotionSettings)
        {
            SceneObjects.Clear();

            //var terrainGenerator = new TerrainGenerator(fractalBrownianMotionSettings);
            //var generate = terrainGenerator.Generate();

            //SceneObjects.AddRange(generate);

            SceneObjects.Add(new WaterSceneObject());
        }

        public void Update(double elapsedTime)
        {
            //var blue = (Math.Sin(elapsedTime) + 1) / 4;
            //var color = new Vector4(0.5f, 0.5f, (float)blue + 0.5f, 0.0f);
            //foreach (var mesh in Meshes)
            //{
            //    mesh.Color = color;    
            //}

            foreach (var sceneObject in SceneObjects)
            {
                sceneObject.Update(elapsedTime);
            }
        }

        public void Unload()
        {
            //_meshResources.RenderProgram.Delete();
            //_meshResources.VerticesVbo.Delete();
        }

    }
}
