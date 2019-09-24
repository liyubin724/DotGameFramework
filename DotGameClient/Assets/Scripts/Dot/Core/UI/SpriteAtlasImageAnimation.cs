using UnityEngine;

namespace Dot.Core.UI
{

    [AddComponentMenu("UI/Atlas Image Animation", 13)]
    [ExecuteInEditMode]
    public class SpriteAtlasImageAnimation : SpriteAtlasImage
    {
        public int frameRate;
        public bool autoPlayOnAwake = true;
        public string spriteNamePrefix = "";
        public int spriteIndex = 0;
        public int spriteStartIndex = 0;
        public int spriteEndIndex = 0;

        private float frameTime = 0.0f;
        private float elapseTime = 0.0f;
        private bool isPlaying = false;
        protected override void Awake()
        {
            base.Awake();
            frameTime = 1.0f / frameRate;

            if(Application.isPlaying && autoPlayOnAwake)
            {
                isPlaying = true;
                ChangeAnimation();
            }

        }

        private void Update()
        {
            if(isPlaying)
            {
                elapseTime += Time.deltaTime;
                if(elapseTime >= frameTime)
                {
                    ++spriteIndex;
                    ChangeAnimation();

                    elapseTime -= frameTime;
                }
            }
        }

        public void ChangeAnimation()
        {
            if (spriteIndex > spriteEndIndex || spriteIndex < spriteStartIndex)
            {
                spriteIndex = spriteStartIndex;
            }
            string spriteName = $"{spriteNamePrefix}{spriteIndex}";
            if(SpriteName!=spriteName)
            {
                SpriteName = spriteName;
            }
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void PlayAt(int index)
        {
            isPlaying = true;
            spriteIndex = index;
            ChangeAnimation();
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public void StopAt(int index)
        {
            isPlaying = false;
            spriteIndex = index;
            ChangeAnimation();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            ChangeAnimation();
            base.OnValidate();
        }
#endif
    }
}
