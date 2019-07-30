using Dot.Core.Pool;

namespace Dot.Core.Entity
{
    public class EntityControllerPool : ObjectPool<AEntityController>
    {
        public EntityControllerPool(int preloadCount) : base(preloadCount)
        {

        }
    }
}
