using System.Collections.Generic;
using UnityEngine;
using HeuristicMethod = Dot.Core.UI.Atlas.MaxRectsBinPack.FreeRectChoiceHeuristic;

namespace Dot.Core.UI.Atlas
{
    public class DynamicAtlasManager : Util.Singleton<DynamicAtlasManager>
    {
        private static readonly string DefaultAtlasName = "DefaultDynamicAtlas";
        private static readonly int DefaultAtlasSize = 1024;

        private Dictionary<string, DynamicAtlasAssembly> atlasDic = new Dictionary<string, DynamicAtlasAssembly>();
        public bool Contains(string atlasName, string rawImagePath)
        {
            atlasName = GetAtlasName(atlasName);
            if (atlasDic.TryGetValue(atlasName,out DynamicAtlasAssembly atlas))
            {
                return atlas.Contains(rawImagePath);
            }
            return false;
        }

        public Sprite GetSprite(string atlasName,string rawImagePath)
        {
            atlasName = GetAtlasName(atlasName);
            if (atlasDic.TryGetValue(atlasName, out DynamicAtlasAssembly atlas))
            {
                if(atlas.Contains(rawImagePath))
                {
                    return atlas.GetRawImageAsSprite(rawImagePath);
                }
            }
            return null;
        }

        public void AddTexture(string atlasName,string rawImagePath,Texture2D texture)
        {
            atlasName = GetAtlasName(atlasName);
            if (!Contains(atlasName,rawImagePath))
            {
                DynamicAtlasAssembly atlas = GetAtlas(atlasName);
                atlas.AddRawImage(rawImagePath, texture);
            }
        }

        public void ReleaseSprite(string atlasName, string rawImagePath)
        {
            atlasName = GetAtlasName(atlasName);
            if (atlasDic.TryGetValue(atlasName, out DynamicAtlasAssembly atlas))
            {
                if (atlas.Contains(rawImagePath))
                {
                    atlas.RemoveRawImageSprite(rawImagePath);
                }
            }
        }
        
        public DynamicAtlasAssembly CreateAtlas(string atlasName = "",int atlasSize = 0, TextureFormat texFormat = TextureFormat.RGBA32, HeuristicMethod method = HeuristicMethod.RectBestShortSideFit)
        {
            atlasName = GetAtlasName(atlasName);
            if(atlasDic.TryGetValue(atlasName,out DynamicAtlasAssembly atlas))
            {
                return atlas;
            }
            atlasSize = atlasSize <= 0 ? DefaultAtlasSize : atlasSize;
            atlas = new DynamicAtlasAssembly(atlasName, atlasSize, atlasSize, method, texFormat);
            atlasDic.Add(atlasName,atlas);
            return atlas;
        }

        public DynamicAtlasAssembly GetAtlas(string atlasName, bool isCreateIfNot = true)
        {
            atlasName = GetAtlasName(atlasName);
            if (atlasDic.TryGetValue(atlasName,out DynamicAtlasAssembly atlas))
            {
                return atlas;
            }else
            {
                if(isCreateIfNot)
                {
                    return CreateAtlas(atlasName);
                }else
                {
                    return null;
                }
            }
        }

        private string GetAtlasName(string atlasName) => string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
    }
}
