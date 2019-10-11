using UnityEditor;
using UnityEngine;
using DotEditor.Core.Packer;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CustomEditor(typeof(AssetAddressAssembly))]
    public class AssetAddressAssemblyEditor : AssetAssemblyEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGroup();
            DrawOperation();

            if(GUILayout.Button("Execute",GUILayout.Height(40)))
            {
                BundlePackUtil.UpdateTagConfig();
                if (BundlePackUtil.IsAddressRepeat())
                {
                    EditorUtility.DisplayDialog("Error", "Address Repeat!", "OK");
                }else
                {
                    BundlePackUtil.UpdateAddressConfig();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
