namespace Dot.Core.Entity
{
    public interface IEntityBuilder
    {
        EntityObject CreateEntityObject(int entityType);
        void DestroyEntityObject(EntityObject entity);
    }
}
