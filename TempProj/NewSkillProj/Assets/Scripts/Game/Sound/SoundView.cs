using UnityEngine;

public class SoundView : ABaseView,IMarkDestroyListener
{
    private SoundType soundType;
    public AudioSource AudioSource
    {
        get;
        private set;
    }

    private GameEntity ViewEntity => (GameEntity)entity;

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


    public void OnMarkDestroy(GameEntity entity)
    {
        AudioSource.Stop();
        SoundManager.GetInstance().ReleaseAudioSource(AudioSource, soundType);
        AudioSource = null;

        base.DestroyView();
    }

    public void SetData(SoundType soundType)
    {
        this.soundType = soundType;

        AudioSource = SoundManager.GetInstance().GetAudioSource(soundType);
        AudioSource.Play();
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
