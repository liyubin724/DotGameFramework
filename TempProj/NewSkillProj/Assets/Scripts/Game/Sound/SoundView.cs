using UnityEngine;

public class SoundView : VirtualView,IMarkDestroyListener
{
    public AudioSource AudioSource
    {
        get;
        private set;
    }

    private GameEntity ViewEntity
    {
        get
        {
            return (GameEntity)GetEntity();
        }
    }

    public override bool Active
    {
        get
        {
            return AudioSource.enabled;
        }
        set
        {
            AudioSource.enabled = value;
        }
    }

    private SoundType soundType;

    public void OnMarkDestroy(GameEntity entity)
    {
        SoundManager.GetInstance().ReleaseAudioSource(AudioSource, soundType);
        AudioSource = null;
    }

    public void SetData(SoundType soundType)
    {
        this.soundType = soundType;
        AudioSource = SoundManager.GetInstance().GetAudioSource(soundType);
    }

    public override void AddListeners()
    {
        ViewEntity.AddMarkDestroyListener(this);
    }

    public override void RemoveListeners()
    {
        ViewEntity.RemoveMarkDestroyListener(this);
    }
}
