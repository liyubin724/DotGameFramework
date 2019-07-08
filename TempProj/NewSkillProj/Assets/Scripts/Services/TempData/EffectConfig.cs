public class EffectConfig : BaseConfig<EffectConfigData>
{
    public EffectConfig()
    {
        AddData(new EffectConfigData()
        {
            id = 1,
            assetPath = "Effect/effect_01",
        });
        AddData(new EffectConfigData()
        {
            id = 2,
            assetPath = "Effect/effect_02",
        });
        AddData(new EffectConfigData()
        {
            id = 3,
            assetPath = "Effect/effect_03",
        });
    }
}

public class EffectConfigData: BaseConfigData
{
    public string assetPath;
}
