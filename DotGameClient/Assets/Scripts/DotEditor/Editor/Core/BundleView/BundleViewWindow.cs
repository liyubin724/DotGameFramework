using Dot.Core.Loader;
using ReflectionMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.BundleView
{
    public class BundleViewWindow : EditorWindow
    {
        [MenuItem("Game/Asset Bundle/Bundle View Window")]
        public static void ShowWin()
        {
            BundleViewWindow win = EditorWindow.GetWindow<BundleViewWindow>();
            win.titleContent = new GUIContent("Bundle Packer");
            win.Show();
        }

        private AssetPathMode pathMode;
        private Dictionary<string, AssetNode> assetNodeDic = null;
        private Dictionary<string, BundleNode> bundleNodeDic = null;
        private void OnEnable()
        {
            if (EditorApplication.isPlaying)
            {
                AssetManager assetManager = AssetManager.GetInstance();
                AssetBundleLoader bundleLoader = (AssetBundleLoader)assetManager.AsDynamic().assetLoader;

                dynamic bundleLoaderDynamic = bundleLoader.AsDynamic();
                if (bundleLoader != null)
                {
                    pathMode = (AssetPathMode)bundleLoaderDynamic.pathMode;
                    assetNodeDic = (Dictionary<string, AssetNode>)bundleLoaderDynamic.assetNodeDic;
                    bundleNodeDic = (Dictionary<string, BundleNode>)bundleLoaderDynamic.bundleNodeDic;
                }
            }
        }

        private void OnGUI()
        {
            if(!EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("It can run in playmode");
                return;
            }
            bool isInitSuccess = (bool)AssetManager.GetInstance().AsDynamic().isInit;
            if(!isInitSuccess)
            {
                EditorGUILayout.LabelField("AssetManager has not inited success!!");
                return;
            }
            
            EditorGUILayout.LabelField("Path Mode:", "" + pathMode.ToString());
        }
    }
}
