using Dot.Core.Loader;
using Dot.Core.UI.Atlas;
using UnityEngine;
using UnityEngine.UI;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.UI
{
    public class DynamicAtlasImage : Image
    {
        private AssetLoaderHandle imageLoaderHandle = null;

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
                if(imageLoaderHandle!=null)
                {
                    AssetManager.GetInstance().UnloadAssetLoader(imageLoaderHandle);
                    imageLoaderHandle = null;
                }else if(sprite!=null)
                {
                    DynamicAtlasManager.GetInstance().ReleaseSprite(AtlasName, RawImagePath);
                }
            }
        }

        private void ChangeImage()
        {
            if (!string.IsNullOrEmpty(RawImagePath))
            {
                if(DynamicAtlasManager.GetInstance().Contains(AtlasName, RawImagePath))
                {
                    SetSprite();
                }
                else
                {
                    imageLoaderHandle = AssetManager.GetInstance().LoadAssetAsync(m_RawImagePath, OnLoadImageComplete);
                }
            }
        }

        private void OnLoadImageComplete(string pathOrAddress, UnityObject uObj, SystemObject userData)
        {
            imageLoaderHandle = null;
            if(pathOrAddress == RawImagePath)
            {
                Texture2D texture = uObj as Texture2D;
                DynamicAtlasManager.GetInstance().AddTexture(AtlasName, RawImagePath, texture);

                SetSprite();
            }
        }

        private void SetSprite()
        {
            Sprite sprite = DynamicAtlasManager.GetInstance().GetSprite(AtlasName, RawImagePath);
            this.sprite = sprite;

            if (IsSetNativeSize)
                SetNativeSize();
        }

        protected override void OnDestroy()
        {
            if (Application.isPlaying)
            {
                ReleaseImage();
            }
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

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if(sprite != null)
            {
                base.OnPopulateMesh(vh);
            }else
            {
                vh.Clear();
            }
        }

    }
}
