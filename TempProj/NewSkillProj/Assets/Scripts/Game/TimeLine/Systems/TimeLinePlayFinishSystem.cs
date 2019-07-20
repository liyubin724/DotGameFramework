using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitas;
using Game.TimeLine;

public class TimeLinePlayFinishSystem : AGameEntityReactiveSystem
{
    public TimeLinePlayFinishSystem(Contexts contexts, Services services) : base(contexts, services)
    {
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
            string nextGroupName = "";
            if(e.isNewSkill)
            {
                nextGroupName = SkillTimeLineConst.GetNextGroupName(e.timeLinePlayFinish.groupName);
            }else if(e.isBullet)
            {
                nextGroupName = BulletTimeLineConst.GetNextGroupName(e.timeLinePlayFinish.groupName);
            }
            if(string.IsNullOrEmpty(nextGroupName))
            {
                e.isTimeLineStop = true;
            }else
            {
                e.ReplaceTimeLinePlay(nextGroupName);
            }
            e.RemoveTimeLinePlayFinish();
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasTimeLine && entity.hasTimeLinePlayFinish;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.TimeLine, GameMatcher.TimeLinePlayFinish));
    }
}
