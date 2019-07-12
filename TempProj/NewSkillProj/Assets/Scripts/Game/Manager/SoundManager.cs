using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Dot.Core.Utill.Singleton<SoundManager>
{
    private Stack<AudioSource> unusedCamereASStack = new Stack<AudioSource>();
    private Camera mainCamera = null;

    protected override void DoInit()
    {
        mainCamera = Camera.main;
    }

    public AudioSource GetAudioSource(SoundType soundType)
    {
        AudioSource audioSource = null;
        if(soundType == SoundType.Camera)
        {
            if(unusedCamereASStack.Count>0)
            {
                audioSource = unusedCamereASStack.Pop();
                audioSource.enabled = true;
            }else
            {
                audioSource = mainCamera.gameObject.AddComponent<AudioSource>();
            }
        }
        return audioSource;
    }

    public void ReleaseAudioSource(AudioSource audioSource,SoundType soundType)
    {
        if (audioSource == null) return;

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.enabled = false;
        if(soundType == SoundType.Camera)
        {
            unusedCamereASStack.Push(audioSource);
        }
    }

}
