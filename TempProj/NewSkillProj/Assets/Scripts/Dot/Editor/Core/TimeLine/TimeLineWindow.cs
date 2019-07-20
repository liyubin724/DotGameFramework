﻿using Dot.Core.TimeLine;
using Dot.Core.TimeLine.Data;
using LitJson;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public enum TimeLineEditorOwner
    {
        Skill,
        Bullet,
    }
    
    public class TimeLineEditorConfig
    {
        public TimeLineEditorOwner owner = TimeLineEditorOwner.Skill;
        public string dataSaveDir = "Assets/Resources";
    }

    public class TimeLineWindow : EditorWindow
    {
        private static TimeLineWindow OpenWindow(string title)
        {
            var win = GetWindow<TimeLineWindow>();
            win.titleContent = new GUIContent(title);
            win.wantsMouseMove = true;
            return win;
        }

        [MenuItem("Tools/Skill Editor %#S")]
        private static void OpenSkillWindow()
        {
            var win = OpenWindow("Skill Editor");
            win.editorConfig = new TimeLineEditorConfig()
            {
                owner = TimeLineEditorOwner.Skill,
                dataSaveDir = "ECSResources/Resources/Skill/Data"
            };
            win.Show();
        }
        [MenuItem("Tools/Bullet Editor %#B")]
        private static void OpenBulletWindow()
        {
            var win = OpenWindow("Bullet Editor");
            win.editorConfig = new TimeLineEditorConfig()
            {
                owner = TimeLineEditorOwner.Bullet,
                dataSaveDir = "ECSResources/Resources/Bullet/Data"
            };
            win.Show();
        }

        private TimeLineEditorConfig editorConfig = null;

        private DataEditor dataEditor;
        private EditorSetting editorSetting;
        private string configPath = "";

        private int toolbarHeight = 20;
        private int contentHeight = 10;

        private void OnEnable()
        {
            editorSetting = new EditorSetting();
            minSize = new Vector2(editorSetting.groupWidth + editorSetting.trackWidth + editorSetting.propertyWidth + 200, editorSetting.timeHeight + 300);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width, toolbarHeight));
            {
                DrawToolBar();
            }
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(0, toolbarHeight, position.width, contentHeight));
            {

            }
            GUILayout.EndArea();

            Rect rect = new Rect(2, toolbarHeight + contentHeight + 2, position.width - 4, position.height - toolbarHeight - contentHeight - 4);

            if (dataEditor != null)
            {
                dataEditor.OnGUI(rect);

                if (editorSetting.isChanged)
                {
                    editorSetting.isChanged = false;
                    Repaint();
                }
            }
        }

        private void DrawToolBar()
        {
            using (new GUILayout.HorizontalScope("toolbar", GUILayout.ExpandWidth(true)))
            {
                if (GUILayout.Button("Load", "toolbarbutton", GUILayout.Width(60)))
                {
                    string filePath = EditorUtility.OpenFilePanel("Load Config", Application.dataPath + editorConfig.dataSaveDir, "txt");
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        string assetPath = "Assets" + filePath.Replace(Application.dataPath, "");
                        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                        if (textAsset != null)
                        {
                            TimeLineData data = JsonDataReader.ReadData(JsonMapper.ToObject(textAsset.text));
                            if (data != null)
                            {
                                dataEditor = new DataEditor(data, editorSetting);
                            }
                            configPath = filePath;
                        }
                    }
                }
                if (GUILayout.Button("Create", "toolbarbutton", GUILayout.Width(60)))
                {
                    string filePath = EditorUtility.SaveFilePanel("Save Config", Application.dataPath + editorConfig.dataSaveDir, "tl_config", "txt");
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        TimeLineData data = new TimeLineData();
                        dataEditor = new DataEditor(data, editorSetting);
                        editorSetting.isChanged = true;
                        configPath = filePath;
                    }
                }

                if (dataEditor != null)
                {
                    if (GUILayout.Button("Save", "toolbarbutton", GUILayout.Width(120)))
                    {
                        dataEditor.FillData();

                        string config = JsonDataWriter.WriteData(dataEditor.Data).ToJson();
                        File.WriteAllText(configPath, config, new UTF8Encoding(false));

                        string assetPath = "Assets" + configPath.Replace(Application.dataPath, "");
                        AssetDatabase.ImportAsset(assetPath);
                    }

                    if (GUILayout.Button("Export For Server", "toolbarbutton", GUILayout.Width(120)))
                    {

                    }
                }
            }
        }
    }

}
