using System;
using System.Linq;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class LightMoverSystem : IEntitySystem
    {
        public void Update(double elapsedTime, EntityManager entityManager)
        {
            var light= entityManager.GetEntitiesWithComponent<PositionalLightComponent>()
                .Single();

            var component = entityManager.GetComponent<PositionalLightComponent>(light);
            //component.Position = new Vector3d(Math.Cos(elapsedTime) * 5, 2, Math.Sin(elapsedTime) * 5);
        }
    }
}