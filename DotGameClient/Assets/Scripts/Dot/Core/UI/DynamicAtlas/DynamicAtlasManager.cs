using Dot.Core.Asset;
using System;
using System.Collections.Generic;
using UnityEngine;
using HeuristicMethod = Dot.Core.UI.Atlas.MaxRectsBinPack.FreeRectChoiceHeuristic;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.UI.Atlas
{
    public class RawImageLoaderData
    {
        public string atlasName;
        public string imagePath;
        public Action<Sprite> callback;
        public AssetHandle assetHandle = null;
    }

    public class DynamicAtlasManager : Util.Singleton<DynamicAtlasManager>
    {
        private static readonly string DefaultAtlasName = "DefaultDynamicAtlas";
        private static readonly int DefaultAtlasSize = 1024;

        private Dictionary<string, DynamicAtlasAssembly> atlasDic = new Dictionary<string, DynamicAtlasAssembly>();
        private List<RawImageLoaderData> loaderDataList = new List<RawImageLoaderData>();

        public bool Contains(string atlasName, string rawImagePath)
        {
            atlasName = string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
            if(atlasDic.TryGetValue(atlasName,out DynamicAtlasAssembly atlas))
            {
                return atlas.Contains(rawImagePath);
            }
            return false;
        }

        public Sprite GetSprite(string atlasName,string rawImagePath)
        {
            atlasName = string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
            if (atlasDic.TryGetValue(atlasName, out DynamicAtlasAssembly atlas))
            {
                if(atlas.Contains(rawImagePath))
                {
                    return atlas.GetRawImageAsSprite(rawImagePath);
                }
            }
            return null;
        }

        public void ReleaseSprite(string atlasName, string rawImagePath)
        {
            atlasName = string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
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
            atlasName = string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
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
            atlasName = string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
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

        public void LoadRawImage(string atlasName, string rawImagePath, Action<Sprite> callback)
        {
            atlasName = string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
            RawImageLoaderData loaderData = new RawImageLoaderData();
            loaderData.atlasName = atlasName;
            loaderData.imagePath = rawImagePath;
            loaderData.callback = callback;

            AssetHandle assetHandle = AssetLoader.GetInstance().LoadAssetAsync(rawImagePath, OnRawImageLoadComplete, null, loaderData);
            loaderData.assetHandle = assetHandle;
            loaderDataList.Add(loaderData);
        }

        private void OnRawImageLoadComplete(string address,UnityObject uObj,SystemObject userData)
        {
            RawImageLoaderData loaderData = userData as RawImageLoaderData;
            if(loaderDataList.IndexOf(loaderData)>=0)
            {
                Texture2D texture = uObj as Texture2D;
                DynamicAtlasAssembly atlas = GetAtlas(loaderData.atlasName);
                atlas.AddRawImage(loaderData.imagePath, texture);

                loaderDataList.Remove(loaderData);
                loaderData.callback(atlas.GetRawImageAsSprite(loaderData.imagePath));
            }
            loaderData.assetHandle.Release();
        }

        public void CancelLoadRawImage(string atlasName, string rawImagePath, Action<Sprite> callback)
        {
            for(int i = loaderDataList.Count-1;i>=0;--i)
            {
                RawImageLoaderData loaderData = loaderDataList[i];
                if(loaderData.atlasName == atlasName && loaderData.imagePath == rawImagePath && loaderData.callback == callback)
                {
                    loaderData.assetHandle.Release();
                    loaderDataList.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
