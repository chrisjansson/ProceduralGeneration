using System.Collections.Generic;

namespace CjClutter.OpenGl.EntityComponent
{
    public class Entity
    {
        private string _name;
        private readonly List<IEntityComponent> _entityComponents;

        public Entity(string name)
        {
            _name = name;
            _entityComponents = new List<IEntityComponent>();
        }

        public void AddComponent(IEntityComponent entityComponent)
        {
            _entityComponents.Add(entityComponent);
        }

        public IEnumerable<IEntityComponent> Components { get { return _entityComponents; } }
    }
}