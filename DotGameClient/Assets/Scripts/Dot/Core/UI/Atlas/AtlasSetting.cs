using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;
using System;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dot.Core.UI.Atlas
{
    public enum UIAtlasType
    {
        Unity,
        TexturePacker,
    }

    public enum TPTextureSizeConstraint
    {
        POT,
        AnySize,
        NPOT,
    }

    public enum TPTexturePaddingTrimMode
    {
        None,
        Trim,
        Crop,
        CropKeepPos,
    }

    public enum TPTextureDither
    {
        None,
        FloydSteinberg,
        FloydSteinbergAlpha,
    }
    
    [Serializable]
    [BoxGroup(GroupName = "Unity Atlas Setting", ShowLabel = true)]
    [HideLabel]
    public class UnitySpriteAtlasSetting
    {
        [ReadOnly]
        public bool isTightPacking = false;
    }

    [Serializable]
    [BoxGroup(GroupName ="TP Atlas Setting",ShowLabel =true)]
    [HideLabel]
    public class TPAtlasSetting
    {
        [BoxGroup("TP Setting", ShowLabel = true)]
        [FilePath(Extensions = ".exe", RequireExistingPath = true, AbsolutePath = true)]
        public string tpExePath = "";
        [BoxGroup("TP Setting", ShowLabel = true)]
        [ReadOnly]
        public string sheetFormat = "unity-texture2d";
        [BoxGroup("TP Setting", ShowLabel = true)]
        public bool isTrimSpriteName = true;
        [BoxGroup("TP Setting", ShowLabel = true)]
        public bool forceSquared = true;
        [BoxGroup("TP Setting", ShowLabel = true)]
        [EnumToggleButtons]
        public TPTextureSizeConstraint sizeConstraint = TPTextureSizeConstraint.POT;
        [BoxGroup("TP Setting", ShowLabel = true)]
        public string packMode = "Best";
        [BoxGroup("TP Setting", ShowLabel = true)]
        [EnumToggleButtons]
        public TPTexturePaddingTrimMode paddingTrimMode = TPTexturePaddingTrimMode.None;

        [BoxGroup("Texture Setting", ShowLabel = true)]
        [ReadOnly]
        public SpriteMeshType meshType = SpriteMeshType.FullRect;
        
        [BoxGroup("Texture Setting", ShowLabel = true)]
        [EnumToggleButtons]
        public WrapMode wrapMode = WrapMode.Clamp;
    }

    public class AtlasSetting : ScriptableObject
    {
        [FolderPath(RequireExistingPath =true)]
        [PropertyTooltip("The Folder which the atlas will be packed to")]
        public string atlasDirPath = "Assets/Resources/UI/Atlas";

        [EnumToggleButtons]
        public UIAtlasType atlasType = UIAtlasType.TexturePacker;

        public bool isRotation = false;
        [ValueDropdown("GetAtlasTextureMaxSize")]
        public int maxSize = 2048;
        [ReadOnly]
        public int PixelsPerUnit = 100;
        public int padding = 4;

        public bool isSRGB = true;
        [ReadOnly]
        public bool isReadOrWrite = false;
        [ReadOnly]
        public bool isMipmap = false;
        [EnumToggleButtons]
        public FilterMode filterMode = FilterMode.Bilinear;

        [ShowIf("IsShowUnitySetting")]
        public UnitySpriteAtlasSetting uAtlasSetting = new UnitySpriteAtlasSetting();
        [ShowIf("IsShowTPSetting")]
        public TPAtlasSetting tpAtlasSetting = new TPAtlasSetting();
        
        [BoxGroup(GroupName ="Platform Setting",ShowLabel =true)]
        [ValueDropdown("GetWinTextureFormat")]
        public int winTextureFormat = -1;

        [BoxGroup(GroupName = "Platform Setting", ShowLabel = true)]
        [ValueDropdown("GetAndroidTextureFormat")]
        public int androidTextureFormat = -1;

        [BoxGroup(GroupName = "Platform Setting", ShowLabel = true)]
        [ValueDropdown("GetIOSTextureFormat")]
        public int iosTextureFormat = -1;


        [FolderPath(RequireExistingPath =true)]
        [ListDrawerSettings(Expanded =true)]
        public string[] spriteFolders = new string[] { };

#if UNITY_EDITOR
        private ValueDropdownList<int> GetWinTextureFormat()
        {
            
            ValueDropdownList<int> list = new ValueDropdownList<int>();
            list.Add(TextureImporterFormat.RGBA32.ToString(), (int)TextureImporterFormat.RGBA32);
            list.Add(TextureImporterFormat.RGBA16.ToString(), (int)TextureImporterFormat.RGBA16);
            list.Add(TextureImporterFormat.DXT5.ToString(), (int)TextureImporterFormat.DXT5);
            list.Add(TextureImporterFormat.DXT5Crunched.ToString(), (int)TextureImporterFormat.DXT5Crunched);
            return list;
        }

        private ValueDropdownList<int> GetAndroidTextureFormat()
        {
            ValueDropdownList<int> list = new ValueDropdownList<int>();
            list.Add(TextureImporterFormat.RGBA32.ToString(), (int)TextureImporterFormat.RGBA32);
            list.Add(TextureImporterFormat.RGBA16.ToString(), (int)TextureImporterFormat.RGBA16);
            list.Add(TextureImporterFormat.ETC2_RGBA8.ToString(), (int)TextureImporterFormat.ETC2_RGBA8);
            list.Add(TextureImporterFormat.ETC2_RGBA8Crunched.ToString(), (int)TextureImporterFormat.ETC2_RGBA8Crunched);
            return list;
        }

        private ValueDropdownList<int> GetIOSTextureFormat()
        {
            ValueDropdownList<int> list = new ValueDropdownList<int>();
            list.Add(TextureImporterFormat.RGBA32.ToString(), (int)TextureImporterFormat.RGBA32);
            list.Add(TextureImporterFormat.RGBA16.ToString(), (int)TextureImporterFormat.RGBA16);
            list.Add(TextureImporterFormat.ASTC_RGBA_4x4.ToString(), (int)TextureImporterFormat.ASTC_RGBA_4x4);
            list.Add(TextureImporterFormat.ASTC_RGBA_6x6.ToString(), (int)TextureImporterFormat.ASTC_RGBA_6x6);
            list.Add(TextureImporterFormat.ASTC_RGBA_8x8.ToString(), (int)TextureImporterFormat.ASTC_RGBA_8x8);
            list.Add(TextureImporterFormat.ASTC_RGB_12x12.ToString(), (int)TextureImporterFormat.ASTC_RGB_12x12);
            return list;
        }

        private ValueDropdownList<int> GetAtlasTextureMaxSize()
        {
            ValueDropdownList<int> list = new ValueDropdownList<int>();
            list.Add("256*256", 256);
            list.Add("512*512", 512);
            list.Add("1024*1024", 1024);
            list.Add("2048*2048", 2048);
            return list;
        }

        private bool IsShowUnitySetting()
        {
            return atlasType == UIAtlasType.Unity;
        }

        private bool IsShowTPSetting()
        {
            return atlasType == UIAtlasType.TexturePacker;
        }

        public string GetTPParam()
        {
            StringBuilder paramSB = new StringBuilder();
            paramSB.AppendFormat("--pack-mode {0}  ", tpAtlasSetting.packMode);
            paramSB.Append("--texture-format png ");
            paramSB.AppendFormat("--format {0}  ", tpAtlasSetting.sheetFormat);
            if (tpAtlasSetting.isTrimSpriteName)
                paramSB.Append("--trim-sprite-names  ");
            paramSB.AppendFormat("--max-size {0}  ", maxSize);
            paramSB.AppendFormat("--size-constraints {0}  ", tpAtlasSetting.sizeConstraint.ToString());
            if (tpAtlasSetting.forceSquared)
                paramSB.Append("--force-squared  ");
            paramSB.AppendFormat("--padding {0}  ", padding);
            if (isRotation)
                paramSB.Append("--enable-rotation  ");
            else
                paramSB.Append("--disable-rotation  ");
            paramSB.AppendFormat("--trim-mode {0}  ", tpAtlasSetting.paddingTrimMode.ToString());
            paramSB.Append("--opt RGBA8888  ");

            return paramSB.ToString();
        }
#endif
    }
}
