﻿using Entitas;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    private Contexts contexts;
    private Services services;

    private UpdateSystems updateSystems;
    private LateUpdateSystems lateUpdateSystems;

    void Awake()
    {
        contexts = Contexts.sharedInstance;
        services = new Services(contexts);
        updateSystems = new UpdateSystems(contexts, services);
        lateUpdateSystems = new LateUpdateSystems(contexts, services);

        updateSystems.Initialize();
        lateUpdateSystems.Initialize();

        services.entityFactroy.CreatePlayerEntity(true);
        GameEntity entity = services.entityFactroy.CreatePlayerEntity(false);
        entity.ReplacePosition(new Vector3(5, 0, 0));
        entity = services.entityFactroy.CreatePlayerEntity(false);
        entity.ReplacePosition(new Vector3(-5, 0, 0));
        entity = services.entityFactroy.CreatePlayerEntity(false);
        entity.ReplacePosition(new Vector3(3, 0, 4));
        entity = services.entityFactroy.CreatePlayerEntity(false);
        entity.ReplacePosition(new Vector3(-3, 0, 4));
    }
    private void OnGUI()
    {
        if(GUILayout.Button("Change Target"))
        {
            GameEntity mainPlayer = contexts.game.mainPlayerEntity;
            IGroup<GameEntity> entityGroup = contexts.game.GetGroup(GameMatcher.Player);
            List<GameEntity> entityList = new List<GameEntity>();
            foreach(var e in entityGroup.GetEntities())
            {
                if(!e.isMainPlayer)
                {
                    entityList.Add(e);
                }
            }
            entityList.Sort((e1,e2) =>
            {
                if(e1.uniqueID.value>e2.uniqueID.value)
                {
                    return 1;
                }else if(e1.uniqueID.value < e2.uniqueID.value)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            if(!mainPlayer.hasSelectedTarget)
            {
                mainPlayer.ReplaceSelectedTarget(entityList[0].uniqueID.value);
            }else
            {
                int index = 0;
                for(int i =0;i<entityList.Count;i++)
                {
                    if(entityList[i].uniqueID.value == mainPlayer.selectedTarget.entityID)
                    {
                        index = i;
                        break;
                    }
                }
                index++;
                if(index>=entityList.Count)
                {
                    index = 0;
                }
                mainPlayer.ReplaceSelectedTarget(entityList[index].uniqueID.value);
            }
        }
        if(GUILayout.Button("Skill 10000"))
        {
            GameEntity mainPlayer = contexts.game.mainPlayerEntity;
            mainPlayer.ReplaceEmitSkill(10000);
        }
        if (GUILayout.Button("Skill 10001"))
        {
            GameEntity mainPlayer = contexts.game.mainPlayerEntity;
            mainPlayer.ReplaceEmitSkill(10001);
        }
        //if (GUILayout.Button("Skill 10002"))
        //{
        //    GameEntity mainPlayer = contexts.game.mainPlayerEntity;
        //    mainPlayer.ReplaceEmitSkill(10002);
        //}

    }

    void Update()
    {
        updateSystems.Execute();
        updateSystems.Cleanup();
        
    }

    private void FixedUpdate()
    {
        lateUpdateSystems.Execute();
        lateUpdateSystems.Cleanup();
    }

    private void OnDestroy()
    {
        updateSystems.DeactivateReactiveSystems();
        updateSystems.ClearReactiveSystems();
        updateSystems.TearDown();
        updateSystems = null;

        lateUpdateSystems.DeactivateReactiveSystems();
        lateUpdateSystems.ClearReactiveSystems();
        lateUpdateSystems.TearDown();
        lateUpdateSystems = null;

        services.DoDispose();
        services = null;
    }
}
