using DotEditor.Core.Util;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace DotEditor.Core.UI.Atlas
{
    public static class SpriteAtlasPackerUtil
    {
        [MenuItem("Game/UI/Atlas/Sprite Atlas Auto Pack &p")]
        public static void AutoPackSelectedAtlas()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssets<SpriteAtlasSetting>();
            if (assetPaths == null || assetPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "Please Set the setting at first!", "OK");
                return;
            }

            string[] selectedDirs = SelectionUtil.GetSelectionDirs();
            if (selectedDirs == null || selectedDirs.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a directory", "OK");
                return;
            }

            SpriteAtlasSetting spriteAtlasSetting = AssetDatabase.LoadAssetAtPath<SpriteAtlasSetting>(assetPaths[0]);
            List<SpriteAtlas> atlasList = new List<SpriteAtlas>();
            foreach (string dir in selectedDirs)
            {
                SpriteAtlas atlas = PackSpriteAtlas(dir, spriteAtlasSetting);
                if(atlas!=null)
                {
                    atlasList.Add(atlas);
                }
            }
            Selection.objects = atlasList.ToArray();
        }

        [MenuItem("Assets/Pack Sprite Atlas")]
        public static void PackSelectedAtlas()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssets<SpriteAtlasSetting>();
            if (assetPaths == null || assetPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "Please Set the setting at first!", "OK");
                return;
            }

            string[] selectedDirs = SelectionUtil.GetSelectionDirs();
            if (selectedDirs == null || selectedDirs.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a directory", "OK");
                return;
            }

            SpriteAtlasPackerWindow.ShowUtilityWin(selectedDirs[0]);
        }

        public static SpriteAtlas PackSpriteAtlas(string spriteAssetInputDir,SpriteAtlasSetting setting)
        {
            if (AssetDatabase.IsValidFolder(setting.atlasDirPath))
            {
                string atlasName = spriteAssetInputDir.Substring(spriteAssetInputDir.LastIndexOf("/")+1).ToLower();
                string[] assetPaths = DirectoryUtil.GetAssetsByFileNameFilter(spriteAssetInputDir, true, null, new string[] { ".meta" });

                List<Sprite> sprites = new List<Sprite>();
                foreach (var assetPath in assetPaths)
                {
                    Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                    if (texture != null)
                    {
                        SetTextureToSprite(assetPath, setting);
                        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        sprites.Add(s);
                    }
                }

                if(sprites.Count == 0)
                {
                    return null;
                }

                string atlasAssetPath = string.Format("{0}/{1}_atlas.spriteatlas", setting.atlasDirPath, atlasName);
                SpriteAtlas packedAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasAssetPath);
                if (packedAtlas == null)
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

                return packedAtlas;
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", $"Pack failed.\n{setting.atlasDirPath} is not valid!", "OK");
            }
            return null;
        }

        private static void SetTextureToSprite(string textAssetPath, SpriteAtlasSetting setting)
        {
            TextureImporter texImp = AssetImporter.GetAtPath(textAssetPath) as TextureImporter;
            texImp.textureType = TextureImporterType.Sprite;
            texImp.spriteImportMode = SpriteImportMode.Single;
            texImp.spritePackingTag = "";
            texImp.spritePixelsPerUnit = setting.pixelsPerUnit;
            texImp.sRGBTexture = setting.isSRGB;
            texImp.alphaIsTransparency = true;
            texImp.alphaSource = TextureImporterAlphaSource.FromInput;
            texImp.isReadable = false;
            texImp.mipmapEnabled = false;
            texImp.SaveAndReimport();
        }

        private static void SetSpriteAtlasPlatformSetting(SpriteAtlasSetting setting, SpriteAtlas packAtlas)
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
            saps.enableTightPacking = setting.isTightPacking;
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
    }
}
