using Dot.Core.Logger;
using Dot.Core.Pool;
using System.Collections.Generic;
using UnityEngine;
using static Dot.Core.UI.Atlas.DynamicAtlas;
using HeuristicMethod = Dot.Core.UI.Atlas.MaxRectsBinPack.FreeRectChoiceHeuristic;

namespace Dot.Core.UI.Atlas
{
    public class RawImageAtlasData : IObjectPoolItem
    {
        public string ImagePath { get; set; } = "";
        public DynamicAtlas Atlas { get; set; } = null;
        private int retainCount =0;

        public RawImageAtlasData()
        {
        }

        public void Retain() => ++retainCount;
        public void Release() => --retainCount;
        public bool IsInUsing() => retainCount > 0;

        public void OnNew()
        {
        }

        public void OnRelease()
        {
            ImagePath = "";
            retainCount = 0;
        }
    }

    public class DynamicAtlasAssembly
    {
        private string name;
        private int width = 0;
        private int height = 0;
        private HeuristicMethod method;
        private TextureFormat format;

        private static ObjectPool<RawImageAtlasData> dataPool = new ObjectPool<RawImageAtlasData>(20);

        private List<DynamicAtlas> atlasList = new List<DynamicAtlas>();
        private Dictionary<string, RawImageAtlasData> rawImageDic = new Dictionary<string, RawImageAtlasData>();

        public DynamicAtlasAssembly(string name, int width, int height, HeuristicMethod method, TextureFormat format )
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.method = method;
            this.format = format;
        }

        public bool Contains(string rawImagePath)
        {
            return rawImageDic.ContainsKey(rawImagePath);
        }

        public void AddRawImage(string rawImagePath,Texture2D texture)
        {
            if (texture == null) return;
            if(!texture.isReadable)
            {
                DebugLogger.LogError("DynamicAtlasAssembly::AddRawImageSprite->texture is not readable.path ="+rawImagePath);
                return;
            }

            if (rawImageDic.ContainsKey(rawImagePath)) return;

            if (texture.width > width || texture.height > height)
            {
                DebugLogger.LogError("DynamicAtlasAssembly::AddRawImageSprite->texture is too large,path = " + rawImagePath);
                return;
            }

            RawImageAtlasData imageAtlasData = dataPool.Get();
            imageAtlasData.ImagePath = rawImagePath;

            for(int i =atlasList.Count-1;i>=0;--i)
            {
                DynamicAtlas atlas = atlasList[i];
                if (atlas.Insert(texture, rawImagePath))
                {
                    imageAtlasData.Atlas = atlas;
                    break;
                }
            }

            if(imageAtlasData.Atlas ==null)
            {
                DynamicAtlas atlas = new DynamicAtlas(this.width, this.height, this.name, this.method, this.format);
                atlasList.Add(atlas);

                if (atlas.Insert(texture, rawImagePath))
                {
                    imageAtlasData.Atlas = atlas;
                }
            }
            if (imageAtlasData.Atlas == null)
            {
                DebugLogger.LogError("DynamicAtlasAssembly::AddRawImageSprite->texture add failed,path = " + rawImagePath);
                dataPool.Release(imageAtlasData);
                return;
            }

            rawImageDic.Add(rawImagePath, imageAtlasData);
        }
        
        public Sprite GetRawImageAsSprite(string rawImagePath)
        {
            if(rawImageDic.TryGetValue(rawImagePath,out RawImageAtlasData data))
            {
                SourceInfo sInfo = data.Atlas.Get(rawImagePath);
                if(sInfo!=null)
                {
                    data.Retain();
                    return sInfo.GetSprite();
                }
            }
            return null;
        }

        public void RemoveRawImageSprite(string rawImagePath)
        {
            if (rawImageDic.TryGetValue(rawImagePath, out RawImageAtlasData data))
            {
                data.Release();
                if(!data.IsInUsing())
                {
                    DynamicAtlas atlas = data.Atlas;
                    atlas.Remove(rawImagePath);
                    rawImageDic.Remove(rawImagePath);
                    dataPool.Release(data);

                    if(atlas.Lenght==0)
                    {
                        atlasList.Remove(atlas);
                    }
                }
            }
        }
    }
}
