using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    internal class BundlePackConfigGUI
    {
        GUIContent[] compressionContents =
        {
            new GUIContent("No Compression"),
            new GUIContent("Standard Compression (LZMA)"),
            new GUIContent("Chunk Based Compression (LZ4)")
        };
        int[] compressionValues = { 0, 1, 2 };

        BundlePackConfig packConfig = null;

        GUIContent targetContent;
        GUIContent compressionContent;
        GUIContent forceRebuildContent;
        GUIContent appendHashContent;
        GUIContent cleanBeforeBuildContent;

        private bool advancedSettings;
        private bool isForceRebuild = false;
        private bool isAppendHash = false;

        private Vector2 scrollPos = Vector2.zero;

        internal BundlePackConfigGUI()
        {
            targetContent = new GUIContent("Build Target", "Choose target platform to build for."); 
            compressionContent = new GUIContent("Compression", "Choose no compress, standard (LZMA), or chunk based (LZ4)");
            forceRebuildContent = new GUIContent("Force Rebuild", "Force rebuild the asset bundles");
            appendHashContent = new GUIContent("Append Hash", "Append the hash to the assetBundle name.");
            cleanBeforeBuildContent = new GUIContent("Clean Output Dir", "Delete the output dir before build");

            packConfig = Util.FileUtil.ReadFromBinary<BundlePackConfig>(BundlePackUtil.GetPackConfigPath());
            isForceRebuild = packConfig.bundleOptions.HasFlag(BuildAssetBundleOptions.ForceRebuildAssetBundle);
            isAppendHash = packConfig.bundleOptions.HasFlag(BuildAssetBundleOptions.AppendHashToAssetBundleName);
        }

        private string GetDefaultOutputDir()
        {
            string outputABPath = new System.IO.DirectoryInfo(".").Parent.FullName.Replace('\\', '/');
            return $"{outputABPath}/eternity_assetbunles";
        }

        internal void LayoutGUI()
        {
            var centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
            centeredStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.Label(new GUIContent("Bundle Pack Config"), centeredStyle);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            {
                packConfig.outputDirPath = EditorGUILayoutUtil.DrawDiskFolderSelection("Bundle Output", packConfig.outputDirPath);
                if(string.IsNullOrEmpty(packConfig.outputDirPath))
                {
                    packConfig.outputDirPath = GetDefaultOutputDir();
                }
                packConfig.buildTarget = (ValidBuildTarget)EditorGUILayout.EnumPopup(targetContent, packConfig.buildTarget);

                advancedSettings = EditorGUILayout.Foldout(advancedSettings, "Advanced Settings");
                if (advancedSettings)
                {
                    EditorGUIUtil.BeginIndent();
                    {
                        packConfig.cleanupBeforeBuild = EditorGUILayout.Toggle(cleanBeforeBuildContent, packConfig.cleanupBeforeBuild);
                        packConfig.compression = (CompressOptions)EditorGUILayout.IntPopup(compressionContent, (int)packConfig.compression, compressionContents, compressionValues);

                        EditorGUILayout.Space();

                        EditorGUI.BeginChangeCheck();
                        {
                            isForceRebuild = EditorGUILayout.Toggle(forceRebuildContent, isForceRebuild);
                            isAppendHash = EditorGUILayout.Toggle(appendHashContent, isAppendHash);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (isForceRebuild)
                            {
                                packConfig.bundleOptions |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
                            }
                            else
                            {
                                packConfig.bundleOptions &= ~BuildAssetBundleOptions.ForceRebuildAssetBundle;
                            }
                            if (isAppendHash)
                            {
                                packConfig.bundleOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
                            }
                            else
                            {
                                packConfig.bundleOptions &= ~BuildAssetBundleOptions.AppendHashToAssetBundleName;
                            }
                        }
                    }
                    EditorGUIUtil.EndIndent();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Pack Bundle"))
                {
                    EditorApplication.delayCall += () =>
                    {
                        BundlePackUtil.PackAssetBundle(packConfig);
                    };
                }
            }
            EditorGUILayout.EndVertical();


            if (GUI.changed)
            {
                Util.FileUtil.SaveToBinary<BundlePackConfig>(BundlePackUtil.GetPackConfigPath(), packConfig);
            }
        }
    }
}
