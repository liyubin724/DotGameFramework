using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Data;
using LitJson;
using UnityEngine;

public class EntityFactroy : Service
{
    private Services services;
    private Transform viewRootTransfrom;
    public EntityFactroy(Contexts contexts,Services services):base(contexts)
    {
        this.services = services;

        viewRootTransfrom = new GameObject("View Root").transform;
    }

    public override void DoDestroy()
    {
        
    }

    public override void DoReset()
    {
        
    }

    public GameEntity CreateEntity()
    {
        GameEntity entity = CachedContexts.game.CreateEntity();
        entity.AddUniqueID(services.idService.GetNext());

        return entity;
    }

    public GameEntity CreateChildrenEntity(GameEntity entity)
    {
        GameEntity childEntity = CreateEntity();
        childEntity.AddChildOf(entity.uniqueID.value);

        if(entity.hasOwnerID)
        {
            childEntity.ReplaceOwnerID(entity.ownerID.value);
        }else
        {
            childEntity.ReplaceOwnerID(entity.uniqueID.value);
        }

        return childEntity;
    }

    public GameEntity CreatePlayerEntity()
    {
        GameEntity playerEntity = CreateEntity();
        playerEntity.isMainPlayer = true;
        playerEntity.isPlayer = true;
        playerEntity.AddConfigID(0);

        PlayerView playerView = new PlayerView($"Player_{playerEntity.uniqueID.value}",viewRootTransfrom);
        playerEntity.AddVirtualView(playerView);
        playerView.InitializeView(CachedContexts, services, playerEntity);
        playerEntity.AddAddSkeleton("Character/Capsule");

        playerEntity.AddPosition(Vector3.zero);

        return playerEntity;
    }

    public GameEntity CreateSkillEntity(GameEntity entity,int skillConfigID)
    {
        GameEntity skillEntity = CreateChildrenEntity(entity);
        skillEntity.isSkill = true;
        skillEntity.AddConfigID(skillConfigID);

        SkillConfigData data = services.dataService.GetSkillData(skillConfigID);
        JsonData jsonData = JsonMapper.ToObject(Resources.Load<TextAsset>(data.timeLineConfig).text);
        TimeLineController controller = TimeLineReader.ReadController(jsonData);
        controller.Initialize(CachedContexts, services, skillEntity);
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
        EffectView view = new EffectView($"Effect_{effectEntity.uniqueID.value}");
        effectEntity.AddVirtualView(view);
        view.InitializeView(CachedContexts, services, effectEntity);

        EffectConfigData data = services.dataService.GetEffectData(effectConfigID);
        effectEntity.AddAddSkeleton(data.assetPath);

        return effectEntity;
    }

    public GameEntity CreateEffectEntity(GameEntity entity,int effectConfigID,EffectType bindType,BindNodeType nodeType,int nodeIndex)
    {
        GameEntity effectEntity = CreateChildrenEntity(entity);
        effectEntity.isEffect = true;
        effectEntity.AddConfigID(effectConfigID);
        effectEntity.AddEffectBind(bindType, nodeType, nodeIndex);
        EffectView view = new EffectView("");
        effectEntity.AddVirtualView(view);
        view.InitializeView(CachedContexts, services, effectEntity);

        EffectConfigData data = services.dataService.GetEffectData(effectConfigID);
        effectEntity.AddAddSkeleton(data.assetPath);

        return effectEntity;
    }

    public GameEntity CreateSoundEntity(GameEntity entity,int soundConfigID)
    {
        GameEntity soundEntity = CreateChildrenEntity(entity);
        soundEntity.isSound = true;
        soundEntity.AddConfigID(soundConfigID);
        SoundView soundView = new SoundView();
        soundEntity.AddVirtualView(soundView);
        soundView.InitializeView(CachedContexts, services, soundEntity);

        return soundEntity;
    }

    public GameEntity CreateBulletEntity(GameEntity entity, int bulletConfigID)
    {
        GameEntity bulletEntity = CreateChildrenEntity(entity);
        bulletEntity.isBullet = true;
        bulletEntity.AddConfigID(bulletConfigID);

        BulletView view = new BulletView($"Bullet_{bulletEntity.uniqueID.value}",viewRootTransfrom);
        view.InitializeView(CachedContexts, services, bulletEntity);
        bulletEntity.AddVirtualView(view);

        BulletConfigData data = services.dataService.GetBulletData(bulletConfigID);
        bulletEntity.AddAddSkeleton(data.assetPath);

        JsonData jsonData = JsonMapper.ToObject(Resources.Load<TextAsset>(data.timeLineConfig).text);
        TimeLineController controller = TimeLineReader.ReadController(jsonData);
        controller.Initialize(CachedContexts, services, bulletEntity);
        bulletEntity.AddTimeLineController(data.timeLineConfig, controller);
        
        return bulletEntity;
    }
}
