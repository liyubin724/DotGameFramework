using UnityEngine;

namespace Dot.Core.Entity
{
    public class PhysicsBehaviour : MonoBehaviour
    {
        public EntityObject Entity { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            GameObject targetGO = other.gameObject;
            EntityObject targetEntityObj = null;
            PhysicsBehaviour targetPhyBeh = targetGO.GetComponent<PhysicsBehaviour>();
            if(targetPhyBeh!=null)
            {
                targetEntityObj = targetPhyBeh.Entity;
            }
            if(targetEntityObj == null)
            {
                Entity.SendEvent(EntityEventConst.TRIGGER_ENTER_SENDER_ID, targetGO);
            }else
            {
                Entity.SendEvent(EntityEventConst.TRIGGER_ENTER_SENDER_ID, targetGO, targetEntityObj);
                EntityContext.GetInstance().SendEvent(Entity.UniqueID, EntityEventConst.TRIGGER_ENTER_RECEIVER_ID,Entity);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        private void OnCollisionEnter(Collision collision)
        {
            
        }
        private void OnCollisionStay(Collision collision)
        {
            
        }

        private void OnCollisionExit(Collision collision)
        {
            
        }
    }
}
