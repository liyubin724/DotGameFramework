using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using static AddressablesSystemExtend.AddressablesSystemConfig;

namespace AddressablesSystemExtend
{
    public static class AddressablesSystemUtility
	{
		public const string LOG_TAG = "Addressable";

		private static readonly Type TYPE_ADDRESSABLE_ASSETS_WINDOW = typeof(UnityEditor.AddressableAssets.GUI.AnalyzeWindow)
			.Assembly.GetType("UnityEditor.AddressableAssets.GUI.AddressableAssetsWindow");

		private static int ms_ActivePlayModeDataBuilderIndexBeforeGenerate;
		private static bool ms_AddressableOpenedBeforeGenerate;

		#region Generate
		public static void GenerateKeysClass()
		{
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, "Generate keys class start");
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.AppendLine("namespace AddressablesSystemExtend")
				.AppendLine("{")
				.AppendLine("\tpublic class Keys")
				.AppendLine("\t{");
			List<AddressableAssetEntry> assets = new List<AddressableAssetEntry>();
			AddressableAssetSettingsDefaultObject.Settings.GetAllAssets(assets);
			List<GenerateKeyItem> items = new List<GenerateKeyItem>(assets.Count);
			for (int iAsset = 0; iAsset < assets.Count; iAsset++)
			{
				AddressableAssetEntry iterAsset = assets[iAsset];
				string groupName = iterAsset.parentGroup.Name;
				if (groupName == "Built In Data")
				{
					continue;
				}
				GenerateKeyItem iterItem = new GenerateKeyItem();
				iterItem.Address = iterAsset.address;
				iterItem.GroupName = groupName;
				iterItem.AssetPath = iterAsset.AssetPath;
				items.Add(iterItem);
			}

			ParallelLoopResult parallelLoopResult = Parallel.For(0, items.Count, iItem =>
			{
				GenerateKeyItem iterItem = items[iItem];
				// I guess it is the path
				if (iterItem.Address.StartsWith("Assets/"))
				{
					iterItem.AssetKey = Leyoutech.Utility.StringUtility.SubFileNameFromPath(iterItem.AssetPath);
				}
				else
				{
					iterItem.AssetKey = iterItem.Address;
				}

				iterItem.GroupName = Leyoutech.Utility.StringUtility.ConvertToVariableName(iterItem.GroupName);
				iterItem.AssetKey = Leyoutech.Utility.StringUtility.ConvertToVariableName(iterItem.AssetKey);
				iterItem.AssetKey = string.Format("{0}_{1}", iterItem.GroupName, iterItem.AssetKey).ToUpper();
				items[iItem] = iterItem;
			});
			while (!parallelLoopResult.IsCompleted)
			{
			}

			HashSet<string> keys = new HashSet<string>();
			for (int iItem = 0; iItem < items.Count; iItem++)
			{
				GenerateKeyItem iterItem = items[iItem];
				if (keys.Add(iterItem.AssetKey))
				{
					stringBuilder.Append("\t\tpublic const string ")
						.Append(iterItem.AssetKey)
						.Append(" = @\"")
						.Append(iterItem.Address)
						.Append("\";")
						.Append("\r\n");
				}
				else
				{
					Leyoutech.Utility.DebugUtility.LogWarning(LOG_TAG, "Duplicate AssetKey: " + iterItem.AssetKey);
				}
			}

			stringBuilder.AppendLine("\t}")
				.AppendLine("}");
			string filePath = Application.dataPath + "/AddressableAssetsData/Config/Keys.cs";
			string fileContent = stringBuilder.ToString();
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG
				, string.Format("Generate keys class, write to file: {0}\n{1}"
					, filePath
					, fileContent));
			File.WriteAllText(filePath, fileContent);
			AssetDatabase.ImportAsset("Assets/AddressableAssetsData/Config/Keys.cs");

			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, "Generate keys class finish");
		}

		public static void GenerateAll(AddressablesSystemConfig config)
		{
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, "Generate all group start");
			BeforeGenerate();
			AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
			for (int iGroupRule = 0; iGroupRule < config.GroupRules.Length; iGroupRule++)
			{
				GroupRule iterGroupRule = config.GroupRules[iGroupRule];
				if (string.IsNullOrWhiteSpace(iterGroupRule.GroupName))
				{
					Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Group index ({0}) name is empty", iGroupRule));
					continue;
				}

				GenerateWithGroupRule(settings, config.MyGenerateSetting, iterGroupRule);
			}
			AfterGenerate();
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, "Generate all group finish");
		}

		public static void GenerateSpecified(AddressablesSystemConfig config)
		{
			string specifiedGroupName = config.MyGenerateSetting.SpecifiedGroupName;
			if (string.IsNullOrWhiteSpace(specifiedGroupName))
			{
				Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, "Specified group name is empty");
				return;
			}

			if (SearchGroupRule(config, specifiedGroupName, out GroupRule groupRule))
			{
				Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Generate specified group ({0}) start", specifiedGroupName));
				BeforeGenerate();
				GenerateWithGroupRule(AddressableAssetSettingsDefaultObject.Settings, config.MyGenerateSetting, groupRule);
				AfterGenerate();
				Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Generate specified group ({0}) finish", specifiedGroupName));
			}
			else
			{
				Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Not found specified group ({0})", specifiedGroupName));
			}
		}

		private static bool SearchGroupRule(AddressablesSystemConfig config
			, string groupName
			, out GroupRule groupRule)
		{
			for (int iGroupRule = 0; iGroupRule < config.GroupRules.Length; iGroupRule++)
			{
				GroupRule iterGroupRule = config.GroupRules[iGroupRule];
				if (iterGroupRule.GroupName == groupName)
				{
					groupRule = iterGroupRule;
					return true;
				}
			}

			groupRule = new GroupRule();
			return false;
		}

		private static void GenerateWithGroupRule(AddressableAssetSettings settings
			, GenerateSetting generateSetting
			, GroupRule groupRule)
		{
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Generate group ({0}) start", groupRule.GroupName));
			AddressableAssetGroup oldGroup = settings.FindGroup(groupRule.GroupName);
			AddressableAssetGroup group;
			if (generateSetting.RecreateGroup)
			{
				if (oldGroup)
				{
					Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Remove group ({0})", groupRule.GroupName));
					settings.RemoveGroup(oldGroup);
					oldGroup = null;
				}
				Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Create group ({0})", groupRule.GroupName));
				group = settings.CreateGroup(groupRule.GroupName, false, false, true, groupRule.SchemasToCopy);
			}
			else
			{
				if (oldGroup)
				{
					group = oldGroup;
					if (generateSetting.ReaddSchema)
					{
						Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Readd schema group ({0})", groupRule.GroupName));
						group.ClearSchemas(true, false);
						for (int iSchema= 0; iSchema < groupRule.SchemasToCopy.Count;iSchema++)
						{
							group.AddSchema(groupRule.SchemasToCopy[iSchema], false);
						}
					}
				}
				else
				{
					Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Create group ({0})", groupRule.GroupName));
					group = settings.CreateGroup(groupRule.GroupName, false, false, true, groupRule.SchemasToCopy);
				}
			}

			if (generateSetting.ApplyAssetRule)
			{
				for (int iAssetRule = 0; iAssetRule < groupRule.AssetRules.Length; iAssetRule++)
				{
					AssetRule iterAssetRule = groupRule.AssetRules[iAssetRule];
					GenerateWithAssetRule(settings, generateSetting, group, groupRule, iAssetRule,iterAssetRule);
				}
			}

			if (generateSetting.RemoveInvalidAsset)
			{
				RemoveInvalidAsset(group);
			}
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Generate group ({0}) finish", groupRule.GroupName));
		}

		private static void GenerateWithAssetRule(AddressableAssetSettings settings
			, GenerateSetting generateSetting
			, AddressableAssetGroup group
			, GroupRule groupRule
			, int assetRuleIndex
			, AssetRule assetRule)
		{
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG
				, string.Format("Gererate asset ruleIndex ({0}) in group ({1}) start"
					, assetRuleIndex
					, groupRule.GroupName));

			int assetsIndexOf = Application.dataPath.LastIndexOf("Assets");
			string realPath = Application.dataPath.Substring(0, assetsIndexOf) + assetRule.Path;
			if (!Directory.Exists(realPath))
			{
				Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Path ({0}) of group ({1}) not exists", realPath, groupRule.GroupName));
				return;
			}

			DirectoryInfo directoryInfo = new DirectoryInfo(realPath);
			FileInfo[] files = directoryInfo.GetFiles("*.*", assetRule.IncludeChilder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			for (int iFile = 0; iFile < files.Length; iFile++)
			{
				FileInfo iterFile = files[iFile];
				string iterFileName = iterFile.Name.Substring(0, iterFile.Name.Length - iterFile.Extension.Length);
				// meta文件肯定是要忽略的
				if (iterFile.Extension == ".meta")
				{
					continue;
				}

				if (!Filter(assetRule.ExtensionFilterType, iterFile.Extension, assetRule.ExtensionFilters)
					|| !Filter(assetRule.FileNameFilterType, iterFileName, assetRule.FileNameFilters))
				{
					continue;
				}

				string iterAssetPath = iterFile.FullName.Substring(assetsIndexOf);
				string assetKey;
				switch (assetRule.AssetKeyType)
				{
					case AssetKeyType.FileName:
						assetKey = iterFileName;
						break;
					case AssetKeyType.FileNameFormat:
						assetKey = string.Format(assetRule.AssetKeyFormat, iterFileName);
						break;
					case AssetKeyType.Path:
						// Unity默认的路径分隔符是'/'
						assetKey = iterAssetPath.Replace('\\', '/');
						break;
					default:
						assetKey = string.Empty;
						throw new Exception(string.Format("not support ExtensionFilterType ({0})", assetRule.ExtensionFilterType));
				}

				string iterAssetGuid = AssetDatabase.AssetPathToGUID(iterAssetPath);
				Leyoutech.Utility.DebugUtility.Log(LOG_TAG
					, string.Format("Create or move entry address:({0}) path:({1}) to group ({2})"
						, assetKey
						, iterAssetPath
						, groupRule.GroupName));
				AddressableAssetEntry iterAssetEntry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(iterAssetGuid, group);
				if (iterAssetEntry == null)
				{
					Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Cant load asset at path ({0})", iterAssetPath));
				}
				else
				{
					iterAssetEntry.SetAddress(assetKey);
					for (int iLabel = 0; iLabel < assetRule.AssetLables.Length; iLabel++)
					{
						iterAssetEntry.SetLabel(assetRule.AssetLables[iLabel], true);
					}
				}
			}

			Leyoutech.Utility.DebugUtility.Log(LOG_TAG
				, string.Format("Gererate asset ruleIndex ({0}) in group ({1}) finish"
					, assetRuleIndex
					, groupRule.GroupName));
		}

		private static bool Filter(FilterType filterType
			, string value
			, List<string> filters)
		{
			switch (filterType)
			{
				case FilterType.BlackList:
					return !filters.Contains(value);
				case FilterType.WhiteList:
					return filters.Contains(value);
				default:
					throw new Exception(string.Format("not support ExtensionFilterType ({0})", filterType));
			}
		}

		private static void RemoveInvalidAsset(AddressableAssetGroup group)
		{
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Remove invalid asset in group ({0}) start", group.Name));
			List<AddressableAssetEntry> invalidAssets = new List<AddressableAssetEntry>();
			foreach (AddressableAssetEntry iterAsset in group.entries)
			{
				if (string.IsNullOrWhiteSpace(AssetDatabase.GUIDToAssetPath(iterAsset.guid)))
				{
					invalidAssets.Add(iterAsset);
				}
			}

			for (int iAsset = 0; iAsset < invalidAssets.Count; iAsset++)
			{
				AddressableAssetEntry iterAsset = invalidAssets[iAsset];
				Leyoutech.Utility.DebugUtility.Log(LOG_TAG
					, string.Format("Remove invalid asset address:({0}) path:({1})"
						, iterAsset.address
						, iterAsset.AssetPath));
				group.RemoveAssetEntry(iterAsset, false);
			}
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, string.Format("Remove invalid asset in group ({0}) finish", group.Name));
		}

		private static void BeforeGenerate()
		{
			UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(TYPE_ADDRESSABLE_ASSETS_WINDOW);
			ms_AddressableOpenedBeforeGenerate = objects.Length > 0;
			for (int iObject = 0; iObject < objects.Length; iObject++)
			{
				EditorWindow iterWindow = objects[iObject] as EditorWindow;
				iterWindow.Close();
			}

			ms_ActivePlayModeDataBuilderIndexBeforeGenerate = AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex;
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG
				, string.Format("BeforeGenerate, ActivePlayModeDataBuilderIndex changed from {0} to 0"
				, ms_ActivePlayModeDataBuilderIndexBeforeGenerate));
			// 0: Fast Mode
			AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex = 0;
		}

		private static void AfterGenerate()
		{
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG
				, string.Format("AfterGenerate, ActivePlayModeDataBuilderIndex changed from 0 to {0}"
				, ms_ActivePlayModeDataBuilderIndexBeforeGenerate));
			AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex = ms_ActivePlayModeDataBuilderIndexBeforeGenerate;

			if (ms_AddressableOpenedBeforeGenerate)
			{
				EditorWindow.GetWindow(TYPE_ADDRESSABLE_ASSETS_WINDOW, false, "Addressables", true);
			}
		}
		#endregion End Generate

		public static void CheckConfig(AddressablesSystemConfig config)
		{
			for (int iGroup = 0; iGroup < config.GroupRules.Length; iGroup++)
			{
				GroupRule iterGroupRule = config.GroupRules[iGroup];
				if (string.IsNullOrWhiteSpace(iterGroupRule.GroupName))
				{
					Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Group-{0}的GroupName为空", iterGroupRule.GroupName));
				}

				for (int iAsset = 0; iAsset < iterGroupRule.AssetRules.Length; iAsset++)
				{
					AssetRule iterAssetRule = iterGroupRule.AssetRules[iAsset];
					if (!(iterAssetRule.Path.StartsWith("Assets")
						&& iterAssetRule.Path.EndsWith("/")))
					{
						Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Group-{0}({1})的AssetRule-{2}的Path不是\"Assets*/\"格式", iGroup, iterGroupRule.GroupName, iAsset));
					}

					for (int iExtension = 0; iExtension < iterAssetRule.ExtensionFilters.Count; iExtension++)
					{
						string iterExtension = iterAssetRule.ExtensionFilters[iExtension];
						if (!iterExtension.StartsWith("."))
						{
							Leyoutech.Utility.DebugUtility.LogError(LOG_TAG, string.Format("Group-{0}({1})的AssetRule-{2}的ExtensionFilters-{3}({4})不是\".*/\"格式", iGroup, iterGroupRule.GroupName, iAsset, iExtension, iterExtension));
						}
					}
				}
			}
			Leyoutech.Utility.DebugUtility.Log(LOG_TAG, "Check config finish");
		}

		public static AddressablesSystemConfig GetConfig()
		{
			string[] assetGuids = AssetDatabase.FindAssets("t:AddressablesSystemConfig");
			if (assetGuids.Length > 0)
			{
				return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuids[0])
					, typeof(AddressablesSystemConfig)) as AddressablesSystemConfig;
			}
			else
			{
				return null;
			}
		}

		[MenuItem("Custom/Addressables/Create Config")]
		private static AddressablesSystemConfig CreateConfig()
		{
			AddressablesSystemConfig config = GetConfig();
			if (config == null)
			{
				config = CreateAssetAtSelectionFolder<AddressablesSystemConfig>("AddressablesSystemConfig");
			}

			return config;
		}

		[MenuItem("Custom/Addressables/Schema/Create Bundled Asset Group Schema")]
		private static BundledAssetGroupSchema CreateBundledAssetGroupSchema()
		{
			return CreateAssetAtSelectionFolder<BundledAssetGroupSchema>("BundledAssetGroupSchema");
		}

		[MenuItem("Custom/Addressables/Schema/Create Content Update Group Schema")]
		private static ContentUpdateGroupSchema CreateContentUpdateGroupSchema()
		{
			return CreateAssetAtSelectionFolder<ContentUpdateGroupSchema>("ContentUpdateGroupSchema");
		}

		[MenuItem("Custom/Addressables/Schema/Create Player Data Group Schema")]
		private static PlayerDataGroupSchema CreatePlayerDataGroupSchema()
		{
			return CreateAssetAtSelectionFolder<PlayerDataGroupSchema>("PlayerDataGroupSchema");
		}

		private static T CreateAssetAtSelectionFolder<T>(string assetName) where T : ScriptableObject
		{
			string selectionPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
			string configDirectory;
			if (Directory.Exists(selectionPath))
			{
				configDirectory = selectionPath;
			}
			else
			{
				if (File.Exists(selectionPath))
				{
					configDirectory = selectionPath.Substring(0, selectionPath.LastIndexOf('/'));
				}
				else
				{
					configDirectory = "Assets";
				}
			}

			string configPath = string.Format("{0}/{1}.asset", configDirectory, assetName);
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<T>(), configPath);

			T asset = AssetDatabase.LoadAssetAtPath(configPath, typeof(T)) as T;
			if (asset)
			{
				Selection.activeObject = asset;
			}
			return asset;
		}

		private struct GenerateKeyItem
		{
			public string AssetPath;
			public string Address;
			public string GroupName;

			public string AssetKey;
		}
	}
}