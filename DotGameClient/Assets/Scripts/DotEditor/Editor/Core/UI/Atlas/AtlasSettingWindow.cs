using Dot.Core.UI.Atlas;
using DotEditor.Core.Util;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace DotEditor.Core.UI.Atlas
{
    public class AtlasSettingWindow : OdinEditorWindow
    {
        private static readonly string AtlasSettingPath = "Assets/Tools/UI/Atlas/atlas_setting.asset";

        [MenuItem("Game/UI/Atlas/Atlas Pack Selected &p")]
        public static void PackSelectedAtlas()
        {
            AtlasSetting atlasSetting = AssetDatabase.LoadAssetAtPath<AtlasSetting>(AtlasSettingPath);
            if(atlasSetting==null || !IsAtlasSettingValid(atlasSetting))
            {
                if (EditorUtility.DisplayDialog("Warning", "Please Set the setting at first!", "OK", "Cancel"))
                {
                    AtlasSettingWindow.ShowWindow();
                }
                return;
            }

            string[] selectedDirs = SelectionUtil.GetSelectionDirs();
            if (selectedDirs == null || selectedDirs.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a directory" , "OK");
                return;
            }
            List<string> spriteDirs = new List<string>(atlasSetting.spriteFolders);
            foreach (string dir in selectedDirs)
            {
                if(spriteDirs.IndexOf(dir)<0)
                {
                    spriteDirs.Add(dir);
                }
                PackAtlas(atlasSetting, dir);
            }
            atlasSetting.spriteFolders = spriteDirs.ToArray();
            EditorUtility.SetDirty(atlasSetting);
        }


        [MenuItem("Game/UI/Atlas/Atlas Pack Window")]
        public static void ShowWindow()
        {
            AtlasSettingWindow win = GetWindow<AtlasSettingWindow>();
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }

        private AtlasSetting atlasSetting = null;
        void Awake()
        {
            atlasSetting = AssetDatabase.LoadAssetAtPath<AtlasSetting>(AtlasSettingPath);
            if(atlasSetting == null)
            {
                atlasSetting = CreateInstance<AtlasSetting>();
                AssetDatabase.CreateAsset(atlasSetting, AtlasSettingPath);
            }

        }

        protected override IEnumerable<object> GetTargets()
        {
            yield return atlasSetting;
            yield return this;
        }

        protected override void DrawEditor(int index)
        {
            base.DrawEditor(index);
        }

        [Button(ButtonHeight =(int)ButtonSizes.Gigantic)]
        [EnableIf("IsValid")]
        public void PackAll()
        {
            if(atlasSetting.spriteFolders!=null && atlasSetting.spriteFolders.Length>0)
            {
                foreach(var folder in atlasSetting.spriteFolders)
                {
                    PackAtlas(atlasSetting, folder);
                }
            }
        }

        private bool IsValid()
        {
            return IsAtlasSettingValid(atlasSetting);
        }

        public static bool IsAtlasSettingValid(AtlasSetting setting)
        {
            if (string.IsNullOrEmpty(setting.atlasDirPath))
            {
                return false;
            }
            if (!Directory.Exists(PathUtil.GetDiskPath(setting.atlasDirPath)))
            {
                return false;
            }

            if (setting.atlasType == UIAtlasType.TexturePacker)
            {
                if (string.IsNullOrEmpty(setting.tpAtlasSetting.tpExePath) || !File.Exists(setting.tpAtlasSetting.tpExePath))
                {
                    return false;
                }
            }

            return true;
        }

        public static void PackAtlas(AtlasSetting setting,string assetDir)
        {
            string spriteDiskPath = PathUtil.GetDiskPath(assetDir);
            string[] textureFiles = Directory.GetFiles(spriteDiskPath, "*.png", SearchOption.TopDirectoryOnly);

            string atlasName = assetDir.Substring(assetDir.LastIndexOf("/")).ToLower();

            List<string> files = new List<string>();
            if(textureFiles!=null && textureFiles.Length>0)
            {
                if (setting.atlasType == UIAtlasType.Unity)
                {
                    Array.ForEach<string>(textureFiles, (f) =>
                    {
                        files.Add(PathUtil.GetAssetPath(f));
                    });
                    PackToUnitySpriteAtlas(atlasName, setting, files.ToArray());
                }else if(setting.atlasType == UIAtlasType.TexturePacker)
                {
                    Array.ForEach<string>(textureFiles, (f) =>
                    {
                        files.Add(f.Replace("\\", "/"));
                    });
                    PackToTPAtlas(atlasName, setting, files.ToArray());
                }
            }
            
        }

        public static void PackToUnitySpriteAtlas(string atlasName,AtlasSetting setting,string[] spriteAssetsPaths)
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach(var p in spriteAssetsPaths)
            {
                SetTextureToSprite(p, setting);
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(p);
                sprites.Add(s);
            }

            string atlasAssetPath = string.Format("{0}/{1}_atlas.spriteatlas",setting.atlasDirPath,atlasName);
            SpriteAtlas packedAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasAssetPath);
            if(packedAtlas == null)
            {
                packedAtlas = new SpriteAtlas();
                AssetDatabase.CreateAsset(packedAtlas, atlasAssetPath);
            }
            packedAtlas.Remove(packedAtlas.GetPackables());
            EditorUtility.SetDirty(packedAtlas);
            AssetDatabase.SaveAssets();

            packedAtlas.Add(sprites.ToArray());

            SetSpriteAtlasPlatformSetting(setting, packedAtlas);
            EditorUtility.SetDirty(packedAtlas);
            AssetDatabase.SaveAssets();
        }

        private static void SetTextureToSprite(string textAssetPath,AtlasSetting setting)
        {
            TextureImporter texImp = AssetImporter.GetAtPath(textAssetPath) as TextureImporter;
            texImp.textureType = TextureImporterType.Sprite;
            texImp.spriteImportMode = SpriteImportMode.Single;
            texImp.spritePackingTag = "";
            texImp.spritePixelsPerUnit = setting.PixelsPerUnit;
            texImp.sRGBTexture = setting.isSRGB;
            texImp.alphaIsTransparency = true;
            texImp.alphaSource = TextureImporterAlphaSource.FromInput;
            texImp.isReadable = false;
            texImp.mipmapEnabled = false;
            texImp.SaveAndReimport();
        }

        private static void SetSpriteAtlasPlatformSetting(AtlasSetting setting,SpriteAtlas packAtlas)
        {
            SpriteAtlasTextureSettings sats = packAtlas.GetTextureSettings();
            sats.readable = setting.isReadOrWrite;
            sats.sRGB = setting.isSRGB;
            sats.generateMipMaps = setting.isMipmap;
            sats.filterMode = setting.filterMode;
            packAtlas.SetTextureSettings(sats);

            SpriteAtlasPackingSettings saps = packAtlas.GetPackingSettings();
            saps.enableRotation = setting.isRotation;
            saps.padding = setting.padding;
            saps.enableTightPacking = setting.uAtlasSetting.isTightPacking;
            packAtlas.SetPackingSettings(saps);

            TextureImporterPlatformSettings winTips = packAtlas.GetPlatformSettings("Standalone");
            winTips.overridden = true;
            winTips.maxTextureSize = setting.maxSize;
            winTips.format = (TextureImporterFormat)setting.winTextureFormat;
            packAtlas.SetPlatformSettings(winTips);

            TextureImporterPlatformSettings androidTips = packAtlas.GetPlatformSettings("Android");
            androidTips.maxTextureSize = setting.maxSize;
            androidTips.overridden = true;
            androidTips.format = (TextureImporterFormat)setting.androidTextureFormat;
            packAtlas.SetPlatformSettings(androidTips);

            TextureImporterPlatformSettings iOSTips = packAtlas.GetPlatformSettings("iPhone");
            iOSTips.maxTextureSize = setting.maxSize;
            iOSTips.overridden = true;
            iOSTips.format = (TextureImporterFormat)setting.iosTextureFormat;
            packAtlas.SetPlatformSettings(iOSTips);
        }

        public static void PackToTPAtlas(string atlasName, AtlasSetting setting,string[] textureDiskPaths)
        {
            string atlasAssetPath = string.Format("{0}/{1}.png", setting.atlasDirPath, atlasName);
            string atlasSheetPath = string.Format("{0}/{1}.tpsheet", setting.atlasDirPath, atlasName);

            string atlasDiskPath = PathUtil.GetDiskPath(atlasAssetPath);
            string atlasDiskSheetPath = PathUtil.GetDiskPath(atlasSheetPath);
            StringBuilder args = new StringBuilder();
            args.Append(setting.GetTPParam());
            args.AppendFormat("--sheet {0}  ", atlasDiskPath);
            args.AppendFormat("--data {0}  ", atlasDiskSheetPath);

            foreach (var p in textureDiskPaths)
            {
                args.Append(p + " ");
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = setting.tpAtlasSetting.tpExePath;
            process.StartInfo.Arguments = args.ToString();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;
            process.Start();
            process.WaitForExit();

            AssetDatabase.ImportAsset(atlasSheetPath);
            AssetDatabase.ImportAsset(atlasAssetPath);

            SetTPAtlasSetting(setting, atlasAssetPath);
            CreateTPAtlas(atlasAssetPath);
        }

        private static void CreateTPAtlas(string atlasAssetPath)
        {
            string tpAtlasAssetPath = atlasAssetPath.Replace(Path.GetExtension(atlasAssetPath), ".prefab");
            GameObject atlasGOPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tpAtlasAssetPath);
            GameObject atlasGOInst = null;
            if (atlasGOPrefab != null)
            {
                atlasGOInst = atlasGOPrefab;
            }
            else
            {
                atlasGOInst = new GameObject(Path.GetFileNameWithoutExtension(tpAtlasAssetPath));
            }
            TPAtlas atlas = atlasGOInst.GetComponent<TPAtlas>();
            if (atlas == null)
            {
                atlas = atlasGOInst.AddComponent<TPAtlas>();
            }

            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(atlasAssetPath);
            List<Sprite> sprites = new List<Sprite>();
            List<string> names = new List<string>();
            foreach (var obj in objs)
            {
                if (obj.GetType() == typeof(Sprite))
                {
                    sprites.Add(obj as Sprite);
                    names.Add(obj.name);
                }
            }
            atlas.sprites = sprites.ToArray();
            atlas.names = names.ToArray();
            

            if (atlasGOPrefab != null)
            {
                EditorUtility.SetDirty(atlasGOPrefab);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(atlasGOInst, tpAtlasAssetPath);
                GameObject.DestroyImmediate(atlasGOInst);
            }

            AssetDatabase.ImportAsset(tpAtlasAssetPath);
        }

        private static void SetTPAtlasSetting(AtlasSetting setting, string atlasAssetPath)
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(atlasAssetPath);
            TextureImporterSettings tis = new TextureImporterSettings();
            ti.ReadTextureSettings(tis);

            tis.textureType = TextureImporterType.Sprite;
            tis.spriteMode = 2;
            tis.spritePixelsPerUnit = setting.PixelsPerUnit;
            tis.spriteMeshType = SpriteMeshType.FullRect;
            tis.sRGBTexture = setting.isSRGB;
            tis.alphaSource = TextureImporterAlphaSource.FromInput;
            tis.alphaIsTransparency = true;
            tis.readable = setting.isReadOrWrite;
            tis.mipmapEnabled = setting.isMipmap;

            ti.SetTextureSettings(tis);

            TextureImporterPlatformSettings winTips = ti.GetPlatformTextureSettings("Standalone");
            winTips.overridden = true;
            winTips.maxTextureSize = setting.maxSize;
            winTips.format = (TextureImporterFormat)setting.winTextureFormat;
            ti.SetPlatformTextureSettings(winTips);

            TextureImporterPlatformSettings androidTips = ti.GetPlatformTextureSettings("Android");
            androidTips.maxTextureSize = setting.maxSize;
            androidTips.overridden = true;
            androidTips.format = (TextureImporterFormat)setting.androidTextureFormat;
            ti.SetPlatformTextureSettings(androidTips);

            TextureImporterPlatformSettings iOSTips = ti.GetPlatformTextureSettings("iPhone");
            iOSTips.maxTextureSize = setting.maxSize;
            iOSTips.overridden = true;
            iOSTips.format = (TextureImporterFormat)setting.iosTextureFormat;
            ti.SetPlatformTextureSettings(iOSTips);

            ti.SaveAndReimport();
        }
    }
}
