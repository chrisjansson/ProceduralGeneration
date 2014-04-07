namespace CjClutter.OpenGl.EntityComponent
{
    public interface IEntitySystem
    {
        void Update(double elapsedTime, EntityManager entityManager);
    }
}