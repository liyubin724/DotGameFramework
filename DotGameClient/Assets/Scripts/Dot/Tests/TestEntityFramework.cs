using Dot.Core.Asset;
using Dot.Core.Entity;
using Dot.Core.Entity.Controller;
using Game.Battle.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TestEntityFramework : MonoBehaviour
{
    private EntityContext entityContext;
    private void Awake()
    {
        entityContext = new EntityContext();

        BulletEntityBuilder bulletBuilder = new BulletEntityBuilder();
        bulletBuilder.Context = entityContext;
        entityContext.RegisterEntityCreator(EntityCategroyConst.BULLET, bulletBuilder);

    }

    private void Update()
    {
        AssetManager.GetInstance().DoUpdate();
        entityContext.DoUpdate(Time.deltaTime);
    }

    private void OnGUI()
    {
        if(GUILayout.Button("CreateBullet"))
        {
            EntityObject entity = entityContext.CreateEntity(EntityCategroyConst.BULLET);
            entity.GetController<EntitySkeletonController>(EntityControllerConst.SKELETON_INDEX).AddSkeleton("prefab");
        }
    }
}