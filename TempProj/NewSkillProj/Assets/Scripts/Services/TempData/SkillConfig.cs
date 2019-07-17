public class SkillConfig : BaseConfig<SkillConfigData>
{
    public SkillConfig()
    {
        AddData(new SkillConfigData() { id = 10000, timeLineConfig = "Skill/Data/skill_10000" });
        AddData(new SkillConfigData() { id = 10001, timeLineConfig = "Skill/Data/skill_10001" });
    }
}

public class SkillConfigData : BaseConfigData
{
    public int dtlID = 1;
    public string timeLineConfig;
}
