using Dot.Config;
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
using UnityEngine.AddressableAssets;

public class TestEntityFramework : MonoBehaviour
{
    private EntityContext entityContext;
    private void Awake()
    {
        entityContext = new EntityContext();

        ConfigManager.GetInstance().InitConfig(() =>
        {
            Debug.Log("Config Data Init Finish!");
        });
    }

    private void Update()
    {
        AssetLoader.GetInstance().DoUpdate();
        entityContext.DoUpdate(Time.deltaTime);
    }

    private void OnGUI()
    {
        if(GUILayout.Button("CreateBullet"))
        {
            EntityObject entity = entityContext.CreateEntity(EntityCategroyConst.BULLET,
                new int[] { EntityControllerConst.SKELETON_INDEX,
                EntityControllerConst.VIEW_INDEX,EntityControllerConst.MOVE_INDEX});

            entity.GetController<EntitySkeletonController>(EntityControllerConst.SKELETON_INDEX).AddSkeleton("prefab");
        }
    }
}