using Entitas;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    public IEntity entity;

    private void OnTriggerEnter(Collider other)
    {
        int targetEntityID = -1;
        GameObject targetGO = other.gameObject;
        EntityBehaviour targetEntity = targetGO.GetComponent<EntityBehaviour>();
        if(targetEntity)
        {
            targetEntityID = (targetEntity.entity as GameEntity).uniqueID.value;
        }
        (entity as GameEntity).ReplaceTriggerEnter(targetEntityID);
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
