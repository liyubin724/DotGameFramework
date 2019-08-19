using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Dot.Core.UI
{
    [AddComponentMenu("UI/Atlas Image", 10)]
    public class SpriteAtlasImage : Image
    {
        [SerializeField]
        private SpriteAtlas m_SpriteAtlas;
        public SpriteAtlas Atlas
        {
            get
            {
                return m_SpriteAtlas;
            }
            set
            {
                if(m_SpriteAtlas !=value)
                {
                    m_SpriteAtlas = value;
                    SetAllDirty();
                }
            }
        }

        [SerializeField]
        private string m_SpriteName = "";
        public string SpriteName
        {
            get
            {
                return m_SpriteName;
            }
            set
            {
                if(m_SpriteName !=value)
                {
                    m_SpriteName = value;
                    SetAllDirty();
                }
            }
        }

        private string m_LastSpriteName = "";
        public override void SetMaterialDirty()
        {
            if(m_LastSpriteName != m_SpriteName)
            {
                m_LastSpriteName = m_SpriteName;

                sprite = m_SpriteAtlas ? m_SpriteAtlas.GetSprite(m_SpriteName) : null;
            }

            base.SetMaterialDirty();
        }
    }
}


