using CjClutter.OpenGl.Input.Keboard;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.EntityComponent
{
    public class InputComponent : IEntityComponent
    {
        public InputComponent(Key forward, Key backward, Key right, Key left, Key up, Key down)
        {
            Down = down;
            Up = up;
            Left = left;
            Right = right;
            Backward = backward;
            Forward = forward;
        }

        public Key Forward { get; private set; }
        public Key Backward { get; private set; }
        public Key Right { get; private set; }
        public Key Left { get; private set; }
        public Key Up { get; private set; }
        public Key Down { get; private set; }
    }

    public class InputSystem : IEntitySystem
    {
        private KeyboardInputProcessor _keyboardInputProcessor;
        private double _elapsedTime;

        public InputSystem(KeyboardInputProcessor keyboardInputProcessor)
        {
            _keyboardInputProcessor = keyboardInputProcessor;
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            var entities = entityManager.GetEntitiesWithComponent<InputComponent>();

            var dt = elapsedTime - _elapsedTime;
            foreach (var entity in entities)
            {
                var inputComponent = entityManager.GetComponent<InputComponent>(entity);
                var lightPosition = entityManager.GetComponent<PositionalLightComponent>(entity);

                if (_keyboardInputProcessor.IsButtonDown(inputComponent.Forward))
                {
                    lightPosition.Position += new Vector3d(1, 0, 0) * dt * 100;
                }

                if (_keyboardInputProcessor.IsButtonDown(inputComponent.Backward))
                {
                    lightPosition.Position -= new Vector3d(1, 0, 0) * dt * 100;
                }

                if (_keyboardInputProcessor.IsButtonDown(inputComponent.Right))
                {
                    lightPosition.Position += new Vector3d(0, 0, 1) * dt * 100;
                }

                if (_keyboardInputProcessor.IsButtonDown(inputComponent.Left))
                {
                    lightPosition.Position -= new Vector3d(0, 0, 1) * dt * 100;
                }

                if (_keyboardInputProcessor.IsButtonDown(inputComponent.Up))
                {
                    lightPosition.Position += new Vector3d(0, 1, 0) * dt * 10;
                }

                if (_keyboardInputProcessor.IsButtonDown(inputComponent.Down))
                {
                    lightPosition.Position -= new Vector3d(0, 1, 0) * dt * 10;
                }
                _elapsedTime = elapsedTime;
            }
        }
    }
}