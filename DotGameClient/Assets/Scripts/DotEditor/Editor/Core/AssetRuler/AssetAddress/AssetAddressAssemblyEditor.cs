using Dot.Core.AssetRuler.AssetAddress;
using UnityEditor;

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

            serializedObject.ApplyModifiedProperties();
        }
    }
}
