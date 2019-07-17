public class SkillConfig : BaseConfig<SkillConfigData>
{
    public SkillConfig()
    {
        AddData(new SkillConfigData() { id = 10000, timeLineConfig = "Skill/Data/skill_10000" });
    }
}

public class SkillConfigData : BaseConfigData
{
    public int dtlID = 1;
    public string timeLineConfig;
}
