using Dot.Core.Event;
using SystemObject = System.Object;

namespace Dot.Core.Entity
{
    public partial class EntityContext
    {
        private EventDispatcher eventDispatcher = new EventDispatcher();

        public void SendEvent(long receiverUniqueID, int eventID, params SystemObject[] datas)
        {
            if (entityDic.TryGetValue(receiverUniqueID, out EntityObject entityObject))
            {
                entityObject.SendEvent(eventID, datas);
            }
        }
    }
}
