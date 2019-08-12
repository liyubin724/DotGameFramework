using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysics : MonoBehaviour
{
    public GameObject ship;
    public GameObject bullet;

    void Start()
    {
        ship.layer = LayerMask.NameToLayer("SpacecraftOtherPlayer");
        var shipCC = ship.AddComponent<CapsuleCollider>();
        var shipR = ship.AddComponent<Rigidbody>();
        shipCC.radius = 0.035f;
        shipCC.height = 0.22f;
        shipCC.direction = 2;
        shipCC.isTrigger = false;
        shipR.useGravity = false;
        shipR.angularDrag = 0;
        shipR.drag = 0;
        shipR.constraints = RigidbodyConstraints.FreezeAll;
        shipR.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        bullet.layer = LayerMask.NameToLayer("SkillProjectile");
        bulletCC = bullet.AddComponent<CapsuleCollider>();
        var bulletR = bullet.AddComponent<Rigidbody>();
        bulletCC.radius = 0.001f;
        bulletCC.height = 0.1f;
        bulletCC.direction = 2;
        bulletCC.isTrigger = true;
        bulletR.useGravity = false;
        bulletR.angularDrag = 0;
        bulletR.drag = 0;
        bulletR.constraints = RigidbodyConstraints.FreezeAll;
        bulletR.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

    }

    private Vector3 prePosition = Vector3.zero;
    private CapsuleCollider bulletCC;
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if(GUILayout.Button("SetPos"))
        {
            prePosition = bullet.transform.position;
        }
        if(GUILayout.Button("Cast"))
        {
            Vector3 curPosition = bullet.transform.position;

            Vector3 dir = (curPosition - prePosition).normalized;
            float distance = (curPosition - prePosition).sqrMagnitude;
            Vector3 colDir = Vector3.zero;
            if (bulletCC.direction == 0)
            {
                colDir = bullet.transform.right;
            }
            else if (bulletCC.direction == 1)
            {
                colDir = bullet.transform.up;
            }
            else if (bulletCC.direction == 2)
            {
                colDir = bullet.transform.forward;
            }
            Vector3 offset = colDir * bulletCC.height * 0.5f;

            Vector3 centerPos = prePosition + bulletCC.center;
            Vector3 position1 = centerPos + offset;
            Vector3 position2 = centerPos - offset;


            if (UnityEngine.Physics.CapsuleCast(position2, position1, bulletCC.radius, dir, out RaycastHit hit, distance))
            {
                Debug.Log("FFFFFFFFFFFF");
            }
        }
    }
}
