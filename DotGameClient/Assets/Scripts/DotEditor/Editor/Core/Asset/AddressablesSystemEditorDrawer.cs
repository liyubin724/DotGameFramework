using UnityEditor;
using UnityEngine;

namespace AddressablesSystemExtend
{
	[CustomPropertyDrawer(typeof(AddressablesSystemEditorAttribute))]
	public sealed class AddressablesSystemEditorDrawer : PropertyDrawer
	{
		private const float PROPERTY_SPACING_HEIGHT = 3.6f;
		private const int PROPERTY_COUNT = 5;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			AddressablesSystemConfig config = property.serializedObject.targetObject as AddressablesSystemConfig;
			float propertyHeight = base.GetPropertyHeight(property, label);
			position.height = propertyHeight;
			if (config == null)
			{
				EditorGUI.LabelField(position, "AddressablesSystemEditorAttribute can only be used in AddressablesSystemConfig ");
			}
			else
			{
				if (GUI.Button(position, "Generate All"))
				{
					AddressablesSystemUtility.GenerateAll(config);
				}

				position.y += propertyHeight + PROPERTY_SPACING_HEIGHT;
				if (GUI.Button(position, "Generate Specified"))
				{
					AddressablesSystemUtility.GenerateSpecified(config);
				}

				position.y += propertyHeight + PROPERTY_SPACING_HEIGHT;
				if (GUI.Button(position, "Check Config"))
				{
					AddressablesSystemUtility.CheckConfig(config);
				}

				position.y += propertyHeight + PROPERTY_SPACING_HEIGHT;
				if (GUI.Button(position, "Generate Keys Class"))
				{
					AddressablesSystemUtility.GenerateKeysClass();
				}

				position.y += propertyHeight + PROPERTY_SPACING_HEIGHT;
				if (GUI.Button(position, "Help"))
				{
					EditorUtility.DisplayDialog("Addressables System Config", "请找唯一", "OK");
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * PROPERTY_COUNT + PROPERTY_SPACING_HEIGHT * (PROPERTY_COUNT - 1);
		}
	}
}