using Dot.Core.Logger;
using Dot.Core.Pool;
using Dot.Core.Util;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Dot.Core.UI.Atlas.DynamicAtlas;
using HeuristicMethod = Dot.Core.UI.Atlas.MaxRectsBinPack.FreeRectChoiceHeuristic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.UI.Atlas
{
    public class DynamicAtlasInfo
    {
        private string name;
        private int width = 512;
        private int height = 512;
        private HeuristicMethod method = HeuristicMethod.RectBestShortSideFit;
        private TextureFormat format = TextureFormat.RGBA32;

        internal List<DynamicAtlas> atlasList = new List<DynamicAtlas>();
        private Dictionary<string, int> spriteUsedCountDic = new Dictionary<string, int>();
        public DynamicAtlasInfo(string name,int size):this(name,size,size)
        {
        }

        public DynamicAtlasInfo(string name,int width,int height,HeuristicMethod method = HeuristicMethod.RectBestShortSideFit,TextureFormat format = TextureFormat.RGBA32)
        {
            this.name = name;
            this.width = width;
            this.height = height;
            this.method = method;
            this.format = format;

            DynamicAtlas atlas = new DynamicAtlas(this.width, this.height, this.name, this.method, this.format);
            atlasList.Add(atlas);
        }

        public Sprite GetSprite(string texPath)
        {
            Sprite sprite = null;
            if(spriteUsedCountDic.ContainsKey(texPath))
            {
                for (int i = 0; i < atlasList.Count; i++)
                {
                    SourceInfo sInfo = atlasList[i].Get(texPath);
                    if(sInfo!=null)
                    {
                        sprite = sInfo.GetSprite();
                        spriteUsedCountDic[texPath]++;
                        break;
                    }
                }
            }
            return sprite;
        }

        public void ReleaseSprite(string texPath)
        {
            if (spriteUsedCountDic.ContainsKey(texPath))
            {
                spriteUsedCountDic[texPath]--;
                if(spriteUsedCountDic[texPath] == 0)
                {
                    spriteUsedCountDic.Remove(texPath);
                    for(int i =0;i<atlasList.Count;i++)
                    {
                        if(atlasList[i].Remove(texPath))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void Insert(string texPath,Texture2D texture)
        {
            if (texture == null)
                return;
            if(texture.width>width || texture.height>height)
            {
                DebugLogger.LogError("DynamicAtlasManager::Insert->texture is too large,path = " + texPath);
                return;
            }

            if(spriteUsedCountDic.ContainsKey(texPath))
            {
                return;
            }

            bool isSuccess = false;
            for(int i = 0;i<atlasList.Count;i++)
            {
                DynamicAtlas atlas = atlasList[i];
                if(atlas.Insert(texture,texPath))
                {
                    isSuccess = true;
                    break;
                }
            }
            if(!isSuccess)
            {
                DynamicAtlas atlas = new DynamicAtlas(this.width, this.height, this.name, this.method, this.format);
                atlasList.Add(atlas);

                if(atlas.Insert(texture,texPath))
                {
                    isSuccess = true;
                }
            }
            if(isSuccess)
            {
                spriteUsedCountDic.Add(texPath, 0);
            }else
            {
                DebugLogger.LogError("DynamicAtlasManager::Insert->texture add failed,path = " + texPath);
            }
        }
    }

    public delegate void OnLoadSpriteFromDynamicAtlasFinish(Sprite sprite);

    public class DynamicAtlasManager : Singleton<DynamicAtlasManager>
    {
        class RawImageLoadingData  : IObjectPoolItem
        {
            public string atlasName = null;
            public int loadingIndex = -1;
            public OnLoadSpriteFromDynamicAtlasFinish callback;

            public void OnNew()
            {
                
            }

            public void OnRelease()
            {
                atlasName = null;
                loadingIndex = -1;
                callback = null;
            }
        }

        private Dictionary<string, DynamicAtlasInfo> atlasInfoDic = new Dictionary<string, DynamicAtlasInfo>();
        private ObjectPool<RawImageLoadingData> loadingDataPool;
        private Dictionary<int, RawImageLoadingData> loadingDataDic = new Dictionary<int, RawImageLoadingData>();
        public DynamicAtlasManager()
        {
            loadingDataPool = new ObjectPool<RawImageLoadingData>(5);
        }

        public bool Contains(string atlasName)
        {
            return atlasInfoDic.ContainsKey(atlasName);
        }

        public void CreateDynamicAtlas(string atlasName,int width,int height, HeuristicMethod method = HeuristicMethod.RectBestShortSideFit, TextureFormat texFormat = TextureFormat.RGBA32)
        {
            if(!atlasInfoDic.ContainsKey(atlasName))
            {
                atlasInfoDic.Add(atlasName, new DynamicAtlasInfo(atlasName, width, height, method, texFormat));
            }
        }

        public void CreateDynamicAtlas(string atlasName,int size)
        {
            CreateDynamicAtlas(atlasName, size, size);
        }

        public int LoadRawImage(string atlasName,string texPath,OnLoadSpriteFromDynamicAtlasFinish callback)
        {
            Sprite sprite = GetSpriteFromAtlas(atlasName, texPath);
            if(sprite)
            {
                callback(sprite);
                return -1;
            }

            RawImageLoadingData data = loadingDataPool.Get();
            data.atlasName = atlasName;
            data.callback = callback;
            //data.loadingIndex = GameAsset.LoadAssetAsync(texPath, OnLoadRawImageFinish);
            loadingDataDic.Add(data.loadingIndex, data);
            return data.loadingIndex;
        }

        public void CancelRawImage(int assetIndex)
        {
            if(assetIndex >=0 && loadingDataDic.ContainsKey(assetIndex))
            {
                RawImageLoadingData data = loadingDataDic[assetIndex];
                loadingDataDic.Remove(assetIndex);
                loadingDataPool.Release(data);

                //GameAsset.CancelLoad(assetIndex);
            }
        }

        private void OnLoadRawImageFinish(int assetIndex, string assetPath, UnityObject obj)
        {
            RawImageLoadingData data = loadingDataDic[assetIndex];
            if(data!=null)
            {
                Sprite sprite = GetSpriteFromAtlas(data.atlasName, assetPath);
                if (sprite)
                {
                    data.callback(sprite);
                }
                else
                {
                    Texture2D texture = (obj as Texture2D);
                    if(texture)
                    {
                        if (atlasInfoDic.TryGetValue(data.atlasName, out DynamicAtlasInfo atlasInfo))
                        {
                            atlasInfo.Insert(assetPath, texture);
                            data.callback?.Invoke(atlasInfo.GetSprite(assetPath));
                        }
                    }
                }
            }
            
            loadingDataDic.Remove(assetIndex);
            loadingDataPool.Release(data);
        }

        private Sprite GetSpriteFromAtlas(string atlasName, string texPath)
        {
            Sprite sprite = null;
            
            if(atlasInfoDic.TryGetValue(atlasName,out DynamicAtlasInfo atlasInfo))
            {
                sprite = atlasInfo.GetSprite(texPath);
            }

            return sprite;
        }

        public void ReleaseSpriteFromAtlas(string atlasName,string texPath)
        {
            if (atlasInfoDic.TryGetValue(atlasName, out DynamicAtlasInfo atlasInfo))
            {
                atlasInfo.ReleaseSprite(texPath);
            }
        }

        public void ReleaseAtlas(string atlasName)
        {
            if(atlasInfoDic.ContainsKey(atlasName))
            {
                atlasInfoDic.Remove(atlasName);
            }
            Dictionary<int, RawImageLoadingData>.KeyCollection keys = loadingDataDic.Keys;
            foreach (var k in keys)
            {
                if(loadingDataDic[k].atlasName == atlasName)
                {
                    CancelRawImage(k);
                }
            }
        }

        public void ReleaseAll()
        {
            atlasInfoDic.Clear();
            if(loadingDataDic.Count > 0)
            {
                Dictionary<int, RawImageLoadingData>.KeyCollection keys = loadingDataDic.Keys;
                foreach(var k in keys)
                {
                    CancelRawImage(k);
                }
            }
        }

        public static void SaveDynamicAtlasToDisk()
        {
            string diskDir = Application.dataPath.Substring(0,Application.dataPath.IndexOf("/Assets"))+"/DynamicAtlas";
            if(!Directory.Exists(diskDir))
            {
                Directory.CreateDirectory(diskDir);
            }
            Dictionary<string, DynamicAtlasInfo> atlasDic = DynamicAtlasManager.GetInstance().atlasInfoDic;
            foreach(var kvp in atlasDic)
            {
                List<DynamicAtlas> atlasList = kvp.Value.atlasList;
                for(int i =0;i<atlasList.Count;i++)
                {
                    atlasList[i].Apply();
                    byte[] bytes = atlasList[i].Texture.EncodeToPNG();
                    File.WriteAllBytes(diskDir+"/"+kvp.Key+"_"+i.ToString()+".png", bytes);
                }
            }
        }

    }
}
