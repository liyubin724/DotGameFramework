public class SkillConfig : BaseConfig<SkillConfigData>
{
    public SkillConfig()
    {
        AddData(new SkillConfigData()
        {
            id = 10000,
            timeLineConfig = "Skill/Data/skill_10000"
        });
        AddData(new SkillConfigData()
        {
            id = 10001,
            timeLineConfig = "Skill/Data/skill_10001"
        });

        AddData(new SkillConfigData()
        {
            id = 10002,
            timeLineConfig = "Skill/Data/skill_10001",
            targetType = SkillTargetType.Entity,
        });
    }
}

public enum SkillTargetType
{
    None,
    Position,
    Entity,
}

public class SkillConfigData : BaseConfigData
{
    public int dtlID = 1;
    public string timeLineConfig;
    public SkillTargetType targetType = SkillTargetType.None;
}
