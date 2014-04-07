using System.Collections.Generic;
using System.Linq;

namespace CjClutter.OpenGl.EntityComponent
{
    public class EntityManager
    {
        private readonly List<Entity> _entities;

        public EntityManager()
        {
            _entities = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity);
        }

        public void AddComponentToEntity(Entity entity, IEntityComponent entityComponent)
        {
            entity.AddComponent(entityComponent);
        }

        public IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : IEntityComponent
        {
            return _entities
                .Where(x => x.Components.Any(y => y.GetType() == typeof (T)))
                .ToList();
        }

        public T GetComponent<T>(Entity entity) where T : IEntityComponent
        {
            return (T) entity.Components.Single(x => x.GetType() == typeof (T));
        }
    }
}