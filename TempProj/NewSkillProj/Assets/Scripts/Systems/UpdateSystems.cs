using Game.Physics;
using Game.Player;
using Game.TimeLine.Systems;

public class UpdateSystems : AServiceFeature
{
    public UpdateSystems(Contexts contexts, Services services) : base("Update Systems", contexts, services)
    {
        Add(new MarkDestroySystem(contexts, services));
        Add(new SoundSystem(contexts, services));
        Add(new SkeletonSystem(contexts, services));
        Add(new EmitSkillSystem(contexts, services));
        Add(new TimeLineDataSystem(contexts, services));
        Add(new TimeLinePlaySystem(contexts, services));
        Add(new TimeLineStopSystem(contexts, services));
        Add(new TimeLinePlayFinishSystem(contexts, services));
        Add(new TimeLineUpdateSystem(contexts, services));

        Add(new MoverSystem(contexts, services));

        Add(new ColliderSystem(contexts, services));
        Add(new RigibodySystem(contexts, services));
        Add(new PhysicsSystem(contexts, services));
        Add(new RaycastHitSystem(contexts, services));

        Add(new GameEventSystems(contexts));


    }
}