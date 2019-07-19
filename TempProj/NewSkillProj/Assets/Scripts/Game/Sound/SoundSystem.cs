using Entitas;
using System.Collections.Generic;
using UnityEngine;


public class SoundSystem : ReactiveSystem<GameEntity> 
{
	private readonly Contexts contexts;
	private readonly Services services;

	public SoundSystem (Contexts contexts,Services services) : base(contexts.game) 
	{
		this.contexts = contexts;
		this.services = services;
	}
		
	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) 
	{
		return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Sound,GameMatcher.ConfigID,GameMatcher.View));
	}
		
	protected override bool Filter(GameEntity entity) 
	{
		return true;
	}

	protected override void Execute(List<GameEntity> entities) 
	{
		foreach (var e in entities) 
		{
            int configID = e.configID.value;
            SoundConfigData data = services.dataService.GetSoundData(configID);

            SoundView soundView = e.view.view as SoundView;
            AudioClip clip = Resources.Load<AudioClip>(data.assetPath);
            soundView.SetData(data.soundType,clip);
        }
    }
}
