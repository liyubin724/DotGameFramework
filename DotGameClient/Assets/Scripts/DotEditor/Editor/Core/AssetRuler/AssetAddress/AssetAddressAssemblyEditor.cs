using UnityEditor;
using UnityEngine;

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
                (target as AssetAddressAssembly).Execute();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
