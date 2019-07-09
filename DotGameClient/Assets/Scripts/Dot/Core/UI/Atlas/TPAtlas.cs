using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.UI.Atlas
{
    public class TPAtlas : MonoBehaviour,ISerializationCallbackReceiver
    {
        [ReadOnly]
        public Sprite[] sprites = new Sprite[0];
        [ReadOnly]
        public string[] names = new string[0];

        private Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();

        public Sprite GetSprite(string name)
        {
            if(cachedSprites.TryGetValue(name,out Sprite sprite))
            {
                return sprite;
            }
            return null;
        }

        public void OnAfterDeserialize()
        {
            cachedSprites.Clear();
            for(int i =0;i<sprites.Length;i++)
            {
                cachedSprites.Add(names[i], sprites[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            
        }
    }
}
