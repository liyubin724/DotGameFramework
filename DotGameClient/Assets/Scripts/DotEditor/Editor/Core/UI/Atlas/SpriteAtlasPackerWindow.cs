using DotEditor.Core.EGUI;
using DotEditor.Core.Util;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.UI.Atlas
{
    public class SpriteAtlasPackerWindow : EditorWindow
    {
        [MenuItem("Game/UI/Atlas/Sprite Atlas Setting")]
        public static void ShowWin()
        {
            SpriteAtlasPackerWindow win = GetWindow<SpriteAtlasPackerWindow>();
            win.titleContent = new GUIContent("Sprite Atlas Setting");
            win.autoRepaintOnSceneChange = true;
            win.Show();
        }

        public static void ShowUtilityWin(string inputDir)
        {
            SpriteAtlasPackerWindow win = GetWindow<SpriteAtlasPackerWindow>();
            win.titleContent = new GUIContent("Sprite Atlas Setting");
            win.autoRepaintOnSceneChange = true;
            win.isUtility = true;
            win.position = new Rect(Screen.currentResolution.width * 0.5f-200, Screen.currentResolution.height * 0.5f-150, 400, 430);
            win.spriteAssetInputDir = inputDir;
            win.Show();
        }

        private bool isUtility = false;
        private string spriteAssetInputDir = "";

        private int[] atlasMaxSizes = new int[]
        {
            128,256,512,1024,2048,4096,
        };
        private string[] atlasMaxSizeContents = new string[]
        {
            "128*128","256*256","512*512","1024*1024","2048*2048","4096*4096",
        };

        private int[] winAtlasFormats = new int[]
        {
            (int)TextureImporterFormat.RGBA32,
            (int)TextureImporterFormat.RGBA16,
            (int)TextureImporterFormat.DXT5,
            (int)TextureImporterFormat.DXT5Crunched,
        };
        private string[] winAtlasFormatContents = new string[]
        {
            TextureImporterFormat.RGBA32.ToString(),
            TextureImporterFormat.RGBA16.ToString(),
            TextureImporterFormat.DXT5.ToString(),
            TextureImporterFormat.DXT5Crunched.ToString(),
    };

        private int[] androidAtlasFormats = new int[]
        {
            (int)TextureImporterFormat.RGBA32,
            (int) TextureImporterFormat.RGBA16,
            (int) TextureImporterFormat.ETC2_RGBA8,
            (int) TextureImporterFormat.ETC2_RGBA8Crunched
        };
        private string[] androidAtlasFormatContents = new string[]
        {
            TextureImporterFormat.RGBA32.ToString(),
            TextureImporterFormat.RGBA16.ToString(),
            TextureImporterFormat.ETC2_RGBA8.ToString(),
            TextureImporterFormat.ETC2_RGBA8Crunched.ToString(),
        };

        private int[] iosAtlasFormats = new int[]
        {
            (int)TextureImporterFormat.RGBA32,
            (int) TextureImporterFormat.RGBA16,
            (int) TextureImporterFormat.ASTC_RGBA_4x4,
            (int) TextureImporterFormat.ASTC_RGBA_6x6,
            (int) TextureImporterFormat.ASTC_RGBA_8x8,
            (int) TextureImporterFormat.ASTC_RGB_12x12
        };
        private string[] iosAtlasFormatContents = new string[]
        {
            TextureImporterFormat.RGBA32.ToString(), 
            TextureImporterFormat.RGBA16.ToString(),
            TextureImporterFormat.ASTC_RGBA_4x4.ToString(),
            TextureImporterFormat.ASTC_RGBA_6x6.ToString(), 
            TextureImporterFormat.ASTC_RGBA_8x8.ToString(), 
            TextureImporterFormat.ASTC_RGB_12x12.ToString(), 
        };

        private SpriteAtlasSetting spriteAtlasSetting;
        private SpriteAtlasSetting displaySetting = null;
        private void OnEnable()
        {
            displaySetting = null;
            spriteAtlasSetting = null;

            string[] assetPaths = AssetDatabaseUtil.FindAssets<SpriteAtlasSetting>();
            if(assetPaths!=null && assetPaths.Length>0)
            {
                spriteAtlasSetting = AssetDatabase.LoadAssetAtPath<SpriteAtlasSetting>(assetPaths[0]);
                if(isUtility)
                {
                    displaySetting = UnityObject.Instantiate<SpriteAtlasSetting>(spriteAtlasSetting);
                }else
                {
                    displaySetting = spriteAtlasSetting;
                }
            }
        }

        private void OnGUI()
        {
            if(displaySetting == null)
            {
                EditorGUILayout.LabelField("Please create SpriteAtlasSetting at first");
                return;
            }
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            {
                if(isUtility)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        EditorGUILayout.TextField("Input Dir", spriteAssetInputDir);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                displaySetting.atlasDirPath = EditorGUILayoutUtil.DrawAssetFolderSelection("Atlas Save Dir", displaySetting.atlasDirPath, true);

                displaySetting.pixelsPerUnit = EditorGUILayout.IntField("Pixels Per Unit", displaySetting.pixelsPerUnit);
                EditorGUILayout.Space();

                displaySetting.isRotation = EditorGUILayout.Toggle("Is Rotation", displaySetting.isRotation);
                displaySetting.isTightPacking = EditorGUILayout.Toggle("Is Tight Packing", displaySetting.isTightPacking);
                displaySetting.padding = EditorGUILayout.IntField("Padding", displaySetting.padding);

                EditorGUILayout.Space();

                displaySetting.isReadOrWrite = EditorGUILayout.Toggle("Is Read Write", displaySetting.isReadOrWrite);
                displaySetting.isMipmap = EditorGUILayout.Toggle("is Mipmap", displaySetting.isMipmap);
                displaySetting.isSRGB = EditorGUILayout.Toggle("is SRGB", displaySetting.isSRGB);
                displaySetting.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Filter Mode", displaySetting.filterMode);
                EditorGUILayout.Space();

                int maxSizeIndex = Array.IndexOf(atlasMaxSizes, displaySetting.maxSize);
                maxSizeIndex = EditorGUILayout.Popup("Max Size", maxSizeIndex, atlasMaxSizeContents);
                if(displaySetting.maxSize!=atlasMaxSizes[maxSizeIndex])
                {
                    displaySetting.maxSize = atlasMaxSizes[maxSizeIndex];
                }
                displaySetting.winTextureFormat = EditorGUILayout.Popup("Win Format", displaySetting.winTextureFormat, winAtlasFormatContents);
                displaySetting.androidTextureFormat = EditorGUILayout.Popup("Android Format", displaySetting.androidTextureFormat, androidAtlasFormatContents);
                displaySetting.iosTextureFormat = EditorGUILayout.Popup("iOS Format", displaySetting.iosTextureFormat, iosAtlasFormatContents);


                if (isUtility)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("Save",GUILayout.Width(80)))
                        {
                            spriteAtlasSetting.CopyFrom(displaySetting);
                            EditorUtility.SetDirty(spriteAtlasSetting);
                        }
                        if(GUILayout.Button("Pack", GUILayout.Width(80)))
                        {
                            SpriteAtlas atlas = SpriteAtlasPackerUtil.PackSpriteAtlas(spriteAssetInputDir, displaySetting);
                            if(atlas!=null)
                            {
                                Selection.activeObject = atlas;
                            }
                            Close();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            if(GUI.changed && !isUtility)
            {
                EditorUtility.SetDirty(displaySetting);
            }
        }
        
        private void OnLostFocus()
        {
            if(isUtility)
            {
                Close();
            }
        }
    }
}
