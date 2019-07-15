using UnityEngine;
public class GameController : MonoBehaviour
{
    private Contexts contexts;
    private Services services;

    private UpdateSystems updateSystems;
    private LateUpdateSystems lateUpdateSystems;

    void Awake()
    {
        contexts = Contexts.sharedInstance;
        services = new Services
        {
            logService = new UnityLogService(contexts),
            timeService = new TimeService(contexts),
            idService = new IDService(contexts),
            dataService = new ConfigDataService(contexts),
        };
        services.entityFactroy = new EntityFactroy(contexts, services);

        updateSystems = new UpdateSystems(contexts, services);
        lateUpdateSystems = new LateUpdateSystems(contexts, services);

        updateSystems.Initialize();
        lateUpdateSystems.Initialize();

        services.entityFactroy.CreatePlayerEntity();
    }
    private void OnGUI()
    {
        if(GUILayout.Button("Skill 1"))
        {
            GameEntity mainPlayer = contexts.game.mainPlayerEntity;
            mainPlayer.ReplaceEmitSkill(1);
        }
        if (GUILayout.Button("Skill 2"))
        {
            GameEntity mainPlayer = contexts.game.mainPlayerEntity;
            mainPlayer.ReplaceEmitSkill(2);
        }
        if (GUILayout.Button("Skill 3"))
        {
            GameEntity mainPlayer = contexts.game.mainPlayerEntity;
            mainPlayer.ReplaceEmitSkill(3);
        }
    }

    void Update()
    {
        updateSystems.Execute();
        updateSystems.Cleanup();
        
    }

    private void LateUpdate()
    {
        lateUpdateSystems.Execute();
        lateUpdateSystems.Cleanup();
    }

    private void OnDestroy()
    {
        updateSystems.DeactivateReactiveSystems();
        updateSystems.ClearReactiveSystems();
        updateSystems.TearDown();
        updateSystems = null;

        lateUpdateSystems.DeactivateReactiveSystems();
        lateUpdateSystems.ClearReactiveSystems();
        lateUpdateSystems.TearDown();
        lateUpdateSystems = null;

        services.Dispose();
        services = null;
    }
}
