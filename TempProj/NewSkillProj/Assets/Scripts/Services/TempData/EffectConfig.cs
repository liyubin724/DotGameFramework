public class EffectConfig : BaseConfig<EffectConfigData>
{
    public EffectConfig()
    {
        AddData(new EffectConfigData()
        {
            id = 1,
            assetPath = "Effect/Prefab/missile_jizhong_02_fire",
            lifeTime = 5.0f,
        });
        AddData(new EffectConfigData()
        {
            id = 2,
            assetPath = "Effect/Prefab/Line_01_jizhong",
            lifeTime = 5.0f,
        });
        AddData(new EffectConfigData()
        {
            id = 3,
            assetPath = "Effect/Prefab/missile_kaipao_02_fire",
            lifeTime = 5.0f,
        });
        AddData(new EffectConfigData()
        {
            id = 4,
            assetPath = "Effect/Prefab/Effect_Shield_001",
            lifeTime = 99.0f,
        });
    }
}

public class EffectConfigData: BaseConfigData
{
    public string assetPath;
    public float lifeTime;
}
