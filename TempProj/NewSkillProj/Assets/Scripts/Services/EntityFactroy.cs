﻿using Dot.Core.TimeLine;
using Dot.Core.TimeLine.Data;
using Game.TimeLine;
using LitJson;
using UnityEngine;

public class EntityFactroy : AService
{
    private Transform viewRootTransfrom;
    private Services services;
    public EntityFactroy(Contexts contexts,Services services):base(contexts)
    {
        this.services = services;

        GameObject viewRootGO = new GameObject("View Root");
        Object.DontDestroyOnLoad(viewRootGO);
        viewRootTransfrom = viewRootGO.transform;
    }
    
    public GameEntity CreateEntity()
    {
        GameEntity entity = contexts.game.CreateEntity();
        entity.AddUniqueID(services.idService.GetNext());

        return entity;
    }

    public GameEntity CreateChildrenEntity(GameEntity entity)
    {
        GameEntity childEntity = CreateEntity();
        childEntity.AddParent(entity.uniqueID.value);

        return childEntity;
    }

    public GameEntity CreatePlayerEntity(bool isMainPlayer = false)
    {
        GameEntity playerEntity = CreateEntity();
        if(isMainPlayer)
        {
            playerEntity.isMainPlayer = isMainPlayer;
        }
        playerEntity.isPlayer = true;
        playerEntity.AddConfigID(0);

        PlayerView playerView = new PlayerView($"Player_{playerEntity.uniqueID.value}",viewRootTransfrom);
        playerView.SetLayer(LayerMask.NameToLayer(isMainPlayer? "MainPlayer":"SpacecraftOtherPlayer"));
                                                                                
        playerView.InitializeView(contexts, services, playerEntity);
        playerEntity.AddView(playerView);

        playerEntity.AddSkeleton("Character/Prefab/PS_AR_Aurora_final");
        playerEntity.AddPosition(Vector3.zero);

        playerEntity.AddCollider(ColliderType.Capsule);
        playerEntity.AddCapsuleCollider(Vector3.zero, 0.035f, 0.22f, 2, false);
        playerEntity.AddRigidbody(false, 0, 0, false,CollisionDetectionMode.ContinuousDynamic, RigidbodyConstraints.None, Vector3.zero);

        return playerEntity;
    }

    public GameEntity CreateSkillEntity(GameEntity playerEntity,int skillConfigID)
    {
        GameEntity skillEntity = CreateChildrenEntity(playerEntity);
        skillEntity.isNewSkill = true;
        skillEntity.AddConfigID(skillConfigID);
        skillEntity.AddOwnerID(playerEntity.uniqueID.value);

        SkillConfigData configData = services.dataService.GetSkillData(skillConfigID);
        JsonData jsonData = JsonMapper.ToObject(Resources.Load<TextAsset>(configData.timeLineConfig).text);
        TimeLineData tlData = JsonDataReader.ReadData(jsonData);
        skillEntity.AddTimeLine(tlData);
        skillEntity.AddTimeLinePlay(SkillTimeLineConst.TIMELINE_BEGIN);



        return skillEntity;
    }

    public GameEntity CreateEffectEntity(GameEntity entity, int effectConfigID)
    {
        GameEntity effectEntity = CreateChildrenEntity(entity);
        effectEntity.isEffect = true;
        effectEntity.AddConfigID(effectConfigID);
        effectEntity.AddOwnerID(entity.ownerID.value);

        EffectView view = new EffectView($"Effect_{effectEntity.uniqueID.value}", viewRootTransfrom);
        effectEntity.AddView(view);
        view.InitializeView(contexts, services, effectEntity);

        EffectConfigData data = services.dataService.GetEffectData(effectConfigID);
        effectEntity.AddSkeleton(data.assetPath);
        effectEntity.AddLifeTime(data.lifeTime);

        return effectEntity;
    }

    public GameEntity CreateSoundEntity(GameEntity entity,int soundConfigID)
    {
        GameEntity soundEntity = CreateChildrenEntity(entity);
        soundEntity.isSound = true;
        soundEntity.AddConfigID(soundConfigID);
        SoundView soundView = new SoundView();
        soundEntity.AddView(soundView);
        soundView.InitializeView(contexts, services, soundEntity);

        return soundEntity;
    }

    public GameEntity CreateBulletEntity(GameEntity entity, int bulletConfigID)
    {
        GameEntity bulletEntity = CreateChildrenEntity(entity);
        bulletEntity.isBullet = true;
        bulletEntity.AddConfigID(bulletConfigID);
        bulletEntity.AddOwnerID(entity.ownerID.value);

        BulletView view = new BulletView($"Bullet_{bulletEntity.uniqueID.value}",viewRootTransfrom);
        view.SetLayer(LayerMask.NameToLayer("SkillProjectile"));
        view.InitializeView(contexts, services, bulletEntity);
        bulletEntity.AddView(view);

        BulletConfigData data = services.dataService.GetBulletData(bulletConfigID);
        bulletEntity.AddSkeleton(data.assetPath);
        if(data.maxSpeed>0)
        {
            bulletEntity.AddMaxSpeed(data.maxSpeed);
        }

        JsonData jsonData = JsonMapper.ToObject(Resources.Load<TextAsset>(data.timeLineConfig).text);
        TimeLineData controller = JsonDataReader.ReadData(jsonData);
        bulletEntity.AddTimeLine(controller);
        bulletEntity.AddTimeLinePlay(BulletTimeLineConst.TIMELINE_BEGIN);

        bulletEntity.AddCollider(ColliderType.Capsule);
        bulletEntity.AddCapsuleCollider(Vector3.zero, 0.001f, 0.1f, 2, true);
        bulletEntity.AddRigidbody(false, 0, 0, true,CollisionDetectionMode.ContinuousDynamic,RigidbodyConstraints.None, Vector3.zero);
        
        return bulletEntity;
    }
}
