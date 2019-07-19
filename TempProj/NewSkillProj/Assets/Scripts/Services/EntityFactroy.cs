using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Data;
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
        playerView.InitializeView(contexts, services, playerEntity);
        playerEntity.AddView(playerView);

        playerEntity.AddSkeleton("Character/Prefab/PS_AR_Aurora_final");
        playerEntity.AddPosition(Vector3.zero);

        return playerEntity;
    }

    public GameEntity CreateSkillEntity(GameEntity playerEntity,int skillConfigID)
    {
        GameEntity skillEntity = CreateChildrenEntity(playerEntity);
        skillEntity.isNewSkill = true;
        skillEntity.AddConfigID(skillConfigID);
        skillEntity.AddOwnerID(playerEntity.uniqueID.value);

        SkillConfigData data = services.dataService.GetSkillData(skillConfigID);
        JsonData jsonData = JsonMapper.ToObject(Resources.Load<TextAsset>(data.timeLineConfig).text);
        TimeLineController controller = JsonDataReader.ReadController(jsonData);
        controller.Initialize(contexts, services, skillEntity);
        skillEntity.AddTimeLineController(data.timeLineConfig, controller);

        return skillEntity;
    }

    public GameEntity CreateTimeEntity(GameEntity entity,float leftTime)
    {
        GameEntity timeEntity = CreateChildrenEntity(entity);

        timeEntity.isTime = true;
        timeEntity.AddTimeDecrease(leftTime);

        return timeEntity;
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
        view.InitializeView(contexts, services, bulletEntity);
        bulletEntity.AddView(view);

        BulletConfigData data = services.dataService.GetBulletData(bulletConfigID);
        bulletEntity.AddSkeleton(data.assetPath);
        if(data.maxSpeed>0)
        {
            bulletEntity.AddMaxSpeed(data.maxSpeed);
        }

        JsonData jsonData = JsonMapper.ToObject(Resources.Load<TextAsset>(data.timeLineConfig).text);
        TimeLineController controller = JsonDataReader.ReadController(jsonData);
        controller.Initialize(contexts, services, bulletEntity);
        bulletEntity.AddTimeLineController(data.timeLineConfig, controller);
        
        return bulletEntity;
    }
}
