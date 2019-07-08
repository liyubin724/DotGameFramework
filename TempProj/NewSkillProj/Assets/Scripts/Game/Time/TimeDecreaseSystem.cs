using Entitas;

public class TimeDecreaseSystem : IExecuteSystem  
{
	private readonly Contexts contexts;
	private readonly Services services;

    private IGroup<GameEntity> gameEntityGroup = null;
	public TimeDecreaseSystem(Contexts contexts,Services services)
	{
		this.contexts = contexts;
		this.services = services;

        gameEntityGroup = contexts.game.GetGroup(GameMatcher.TimeDecrease);
	}

	public void Execute() 
	{
		foreach(var e in gameEntityGroup.GetEntities())
        {
            e.ReplaceTimeDecrease(e.timeDecrease.value - services.timeService.DeltaTime());
        }
	}
}
