using Dot.Core.UI.Atlas;
using UnityEngine;
using UnityEngine.UI;

namespace Dot.Core.UI
{
    public class DynamicAtlasImage : Image
    {
        private bool m_isLoading = false;
        private bool m_IsLoadFinish = false;

        [SerializeField]
        private string m_AtlasName = "";
        public string AtlasName
        {
            get
            {
                return m_AtlasName;
            }
            set
            {
                if (m_AtlasName != value)
                {
                    ReleaseImage();
                    m_AtlasName = value;
                    ChangeImage();
                }
            }
        }

        [SerializeField]
        private string m_RawImagePath = "";
        public string RawImagePath
        {
            get
            {
                return m_RawImagePath;
            }
            set
            {
                if(m_RawImagePath !=value)
                {
                    ReleaseImage();
                    m_RawImagePath = value;
                    ChangeImage();
                }
            }
        }
        [SerializeField]
        private bool m_IsSetNativeSize = true;
        public bool IsSetNativeSize
        {
            get { return m_IsSetNativeSize; }
            set
            {
                m_IsSetNativeSize = value;
                if(m_IsSetNativeSize && sprite!=null)
                {
                    SetNativeSize();
                }
            }
        }

        private void ReleaseImage()
        {
            if(!string.IsNullOrEmpty(m_RawImagePath))
            {
                if(m_isLoading)
                {
                    DynamicAtlasManager.GetInstance().CancelLoadRawImage(AtlasName, RawImagePath,OnLoadImageComplete);
                }else if(m_IsLoadFinish)
                {
                    DynamicAtlasManager.GetInstance().ReleaseSprite(AtlasName, RawImagePath);
                }
            }
        }

        private void ChangeImage()
        {
            if (!string.IsNullOrEmpty(m_RawImagePath))
            {
                m_isLoading = true;
                m_IsLoadFinish = false;
                DynamicAtlasManager.GetInstance().LoadRawImage(AtlasName, RawImagePath, OnLoadImageComplete);
            }
        }

        private void OnLoadImageComplete(Sprite sprite)
        {
            m_isLoading = false;
            m_IsLoadFinish = true;
            this.sprite = sprite;

            if(IsSetNativeSize)
                SetNativeSize();
        }

        protected override void OnDestroy()
        {
            ReleaseImage();
            base.OnDestroy();
        }

        protected override void Awake()
        {
            base.Awake();

            if(Application.isPlaying)
            {
                ChangeImage();
            }
        }

    }
}
