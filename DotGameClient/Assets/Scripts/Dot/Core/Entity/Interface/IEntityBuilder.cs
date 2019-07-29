namespace Dot.Core.Entity
{
    public interface IEntityBuilder
    {
        EntityObject CreateEntityObject(long uniqueID,int entityType);
        void DestroyEntityObject(EntityObject entity);
    }
}
