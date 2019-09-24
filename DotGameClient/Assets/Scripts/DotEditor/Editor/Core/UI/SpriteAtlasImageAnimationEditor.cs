using Dot.Core.UI;
using UnityEditor;

namespace DotEditor.Core.UI
{
    [CustomEditor(typeof(SpriteAtlasImageAnimation), false)]
    public class SpriteAtlasImageAnimationEditor : SpriteAtlasImageEditor
    {
        private SerializedProperty frameRate;
        private SerializedProperty autoPlayOnAwake;
        private SerializedProperty spriteNamePrefix;
        private SerializedProperty spriteIndex;
        private SerializedProperty spriteStartIndex;
        private SerializedProperty spriteEndIndex;
        protected override void OnEnable()
        {
            base.OnEnable();
            frameRate = serializedObject.FindProperty("frameRate");
            autoPlayOnAwake = serializedObject.FindProperty("autoPlayOnAwake");
            spriteNamePrefix = serializedObject.FindProperty("spriteNamePrefix");
            spriteIndex = serializedObject.FindProperty("spriteIndex");
            spriteStartIndex = serializedObject.FindProperty("spriteStartIndex");
            spriteEndIndex = serializedObject.FindProperty("spriteEndIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSpriteAtlas();

            EditorGUILayout.PropertyField(frameRate);
            EditorGUILayout.PropertyField(autoPlayOnAwake);
            EditorGUILayout.PropertyField(spriteNamePrefix);
            EditorGUILayout.PropertyField(spriteIndex);
            EditorGUILayout.PropertyField(spriteStartIndex);
            EditorGUILayout.PropertyField(spriteEndIndex);

            if (spriteIndex.intValue > spriteEndIndex.intValue || spriteIndex.intValue < spriteStartIndex.intValue)
            {
                spriteIndex.intValue = spriteStartIndex.intValue;
            }
            
            AppearanceControlsGUI();
            RaycastControlsGUI();
            DrawTypeGUI();
            DrawImageType();
            DrawNativeSize();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
