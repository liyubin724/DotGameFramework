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
            if(frameRate.intValue<0)
            {
                frameRate.intValue = 0;
            }
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

        protected override void OnSpriteSelectedCallback(SerializedProperty spriteProperty, string spriteName)
        {
            spriteProperty.serializedObject.Update();
            if (string.IsNullOrEmpty(spriteName))
            {
                return;
            }
            
            int nDigitStartIndex = -1;
            for(int i = spriteName.Length-1;i>=0;i--)
            {
                if(!char.IsDigit(spriteName[i]))
                {
                    nDigitStartIndex = i;
                    break;
                }
            }
            if(nDigitStartIndex>0)
            {
                string prefix = spriteName.Substring(0, nDigitStartIndex+1);
                if (prefix != spriteNamePrefix.stringValue)
                {
                    spriteNamePrefix.stringValue = prefix;
                }
                string index = spriteName.Substring(nDigitStartIndex + 1, spriteName.Length - nDigitStartIndex - 1);
                spriteIndex.intValue = int.Parse(index);
            }
            spriteProperty.stringValue = spriteName;
            spriteProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}
