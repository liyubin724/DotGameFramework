#if !NOT_ETERNITY
using Dot.Config;
using Dot.Core.Effect;
using FlatBuffers;
using Game.Data.SkillSystemFx;
using Game.VFXController;

using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace DotEditor.Config
{
    public static class EffectConvertTool
    {
        [MenuItem("Tool/Config/Effect prefab Convert")]
        public static void EffectConvert()
        {
            string fxAssetGroup = "Assets/AddressableAssetsData/AssetGroups/FX.asset";
            AddressableAssetGroup fxGroup = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(fxAssetGroup);

            string fxConfigPath = "Assets/Config/data/skill_system_fx.bytes";
            string configDataPath = "Assets/DotGameRes/ConfigData/config_data.asset";
            ConfigData configData = AssetDatabase.LoadAssetAtPath<ConfigData>(configDataPath);
            
            TextAsset fxConfigText = AssetDatabase.LoadAssetAtPath<TextAsset>(fxConfigPath);
            SkillSystemFx m_Config = SkillSystemFx.GetRootAsSkillSystemFx(new ByteBuffer(fxConfigText.bytes));
            
            Action<string> convertEffectAction = delegate (string address)
            {
                if(configData.GetEffect(address)!=null)
                {
                    return;
                }

                string prefabGUID = "";

                foreach(var entry in fxGroup.entries)
                {
                    if(entry.address == address)
                    {
                        prefabGUID = entry.guid;
                        break;
                    }
                }
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    GameObject effectObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    VFXController fXController = effectObj.GetComponent<VFXController>();

                    EffectConfigData effectConfigData = new EffectConfigData();
                    effectConfigData.address = address;
                    if (fXController==null)
                    {
                        effectConfigData.lifeTime = 3f;
                        effectConfigData.stopDelayTime = 3.0f;
                    }
                    else
                    {
                        if (fXController.AutoStop)
                        {
                            effectConfigData.lifeTime = fXController.StopDelay;
                        }
                        else
                        {
                            effectConfigData.lifeTime = 0f;
                        }
                        effectConfigData.stopDelayTime = fXController.DestroyDelay;
                    }

                    EffectBehaviour effectBehaviour = effectObj.GetComponent<EffectBehaviour>();
                    if(effectBehaviour == null)
                    {
                        effectBehaviour = effectObj.AddComponent<EffectBehaviour>();
                    }
                    effectBehaviour.AutoFind();

                    if (fXController != null)
                        fXController.enabled = false;

                    PrefabUtility.ApplyPrefabInstance(effectObj, InteractionMode.AutomatedAction);
                    configData.effectConfig.configs.Add(effectConfigData);
                }
            };



            int count = m_Config.SkillFx.Value.DataLength;
            for (int i = 0; i < count; i++)
            {
                SkillFxVO skillFxVO = (SkillFxVO)(m_Config.SkillFx.Value.Data(i));
                string fxBegin = skillFxVO.CastFxBegin;
                string fxLoop = skillFxVO.CastFxLoop;
                string fxEnd = skillFxVO.CastFxEnd;
                string launcherFx = skillFxVO.LauncherFx;
                string hitFx = skillFxVO.HitFx;

                if (!IsEmptyOrNull(fxBegin))
                {
                    convertEffectAction(fxBegin);
                }
                if (!IsEmptyOrNull(fxLoop))
                {
                    convertEffectAction(fxLoop);
                }
                if (!IsEmptyOrNull(fxEnd))
                {
                    convertEffectAction(fxEnd);
                }
                if (!IsEmptyOrNull(launcherFx))
                {
                    convertEffectAction(launcherFx);
                }
                if (!IsEmptyOrNull(hitFx))
                {
                    convertEffectAction(hitFx);
                }
            }

            EditorUtility.SetDirty(configData);
            AssetDatabase.SaveAssets();
        }

        private static bool IsEmptyOrNull(string path)
        {
            return string.IsNullOrEmpty(path) || path == "None";
        }
    }
}
#endif