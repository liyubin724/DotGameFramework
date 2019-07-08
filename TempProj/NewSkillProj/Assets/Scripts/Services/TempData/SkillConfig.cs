public class SkillConfig : BaseConfig<SkillConfigData>
{
    public SkillConfig()
    {
        AddData(new SkillConfigData() { id = 1,timeLineConfig = "Skill/skill_1001" });
        AddData(new SkillConfigData() { id = 2, timeLineConfig = "Skill/skill_1002" });
        AddData(new SkillConfigData() { id = 3, timeLineConfig = "Skill/skill_1001" });
        AddData(new SkillConfigData() { id = 4, timeLineConfig = "Skill/skill_1001" });
    }
}

public class SkillConfigData : BaseConfigData
{
    public int dtlID = 1;
    public string timeLineConfig;
}
