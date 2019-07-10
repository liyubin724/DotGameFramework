using UnityEngine;

public class ConfigDataService : Service
{
    public ConfigDataService(Contexts contexts) : base(contexts)
    {
    }

    public override void DoDestroy()
    {

    }

    public override void DoReset()
    {

    }

    public T GetData<T>(string resPath) where T : ScriptableObject => Resources.Load<T>(resPath);

    private SkillConfig skillConfig = new SkillConfig();
    public SkillConfigData GetSkillData(int id) => skillConfig.GetData(id);

    SoundConfig soundConfig = new SoundConfig();
    public SoundConfigData GetSoundData(int id) => soundConfig.GetData(id);
    
    EffectConfig effectConfig = new EffectConfig();
    public EffectConfigData GetEffectData(int id) => effectConfig.GetData(id);

    BulletConfig bulletConfig = new BulletConfig();
    public BulletConfigData GetBulletData(int id) => bulletConfig.GetData(id);

}