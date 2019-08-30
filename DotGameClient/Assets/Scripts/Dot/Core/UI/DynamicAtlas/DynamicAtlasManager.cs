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
        public List<Action<Sprite>> callbacks = new List<Action<Sprite>>();
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
            if (atlasDic.TryGetValue(GetAtlasName(atlasName),out DynamicAtlasAssembly atlas))
            {
                return atlas;
            }else
            {
                if(isCreateIfNot)
                {
                    return CreateAtlas(GetAtlasName(atlasName));
                }else
                {
                    return null;
                }
            }
        }

        public void LoadRawImage(string atlasName, string rawImagePath, Action<Sprite> callback)
        {
            Sprite sprite = GetSprite(atlasName, rawImagePath);
            if(sprite!=null)
            {
                callback?.Invoke(sprite);
                return;
            }

            RawImageLoaderData loaderData = GetLoadingData(GetAtlasName(atlasName), rawImagePath);
            if(loaderData != null)
            {
                loaderData.callbacks.Add(callback);
                return;
            }
            loaderData = new RawImageLoaderData();
            loaderData.atlasName = GetAtlasName(atlasName);
            loaderData.imagePath = rawImagePath;
            loaderData.callbacks.Add(callback);
            
            AssetHandle assetHandle = AssetLoader.GetInstance().LoadAssetAsync(rawImagePath, OnRawImageLoadComplete, null, loaderData);
            loaderData.assetHandle = assetHandle;
            loaderDataList.Add(loaderData);
        }

        public RawImageLoaderData GetLoadingData(string atlasName, string rawImagePath)
        {
            foreach(var data in loaderDataList)
            {
                if(data.atlasName == atlasName && data.imagePath == rawImagePath)
                {
                    return data;
                }
            }
            return null;
        }

        private void OnRawImageLoadComplete(string address,UnityObject uObj,SystemObject userData)
        {
            RawImageLoaderData loaderData = userData as RawImageLoaderData;
            if(loaderDataList.IndexOf(loaderData)>=0)
            {
                Texture2D texture = uObj as Texture2D;
                DynamicAtlasAssembly atlas = GetAtlas(loaderData.atlasName);
                atlas.AddRawImage(loaderData.imagePath, texture);
                
                loaderData.callbacks.ForEach((callback) =>
                {
                    Sprite sprite = atlas.GetRawImageAsSprite(loaderData.imagePath);
                    callback?.Invoke(sprite);
                });

                loaderDataList.Remove(loaderData);
            }
            loaderData.assetHandle.Release();
         }

        public void CancelLoadRawImage(string atlasName, string rawImagePath, Action<Sprite> callback)
        {
            RawImageLoaderData loaderData = GetLoadingData(GetAtlasName(atlasName), rawImagePath);
            if(loaderData!=null)
            {
                for(int i =0;i< loaderData.callbacks.Count;++i)
                {
                    if(loaderData.callbacks[i] == callback)
                    {
                        loaderData.callbacks.RemoveAt(i);
                        break;
                    }
                }
                if(loaderData.callbacks.Count == 0)
                {
                    loaderData.assetHandle.Release();
                    loaderDataList.Remove(loaderData);
                }
            }
        }

        private string GetAtlasName(string atlasName) => string.IsNullOrEmpty(atlasName) ? DefaultAtlasName : atlasName;
    }
}
