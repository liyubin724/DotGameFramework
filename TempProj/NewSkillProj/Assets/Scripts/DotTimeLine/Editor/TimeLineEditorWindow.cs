using DotTimeLine;
using DotTimeLine.Base;
using DotTimeLine.Data;
using LitJson;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TimeLineEditorWindow : EditorWindow
{
    [MenuItem("Tools/TimeLine Window %#T")]
    private static void OpenWindow()
    {
        var win = GetWindow<TimeLineEditorWindow>();
        win.wantsMouseMove = true;
    }
    private TimeLineEditorController editorController;
    private TimeLineEditorSetting editorSetting;
    private string configPath = "";

    private int toolbarHeight = 20;
    private int contentHeight = 10;

    private void OnEnable() 
    {
        editorSetting = new TimeLineEditorSetting();
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

        if(editorController != null)
        {
            editorController.OnGUI(rect);

            if(editorSetting.isChanged)
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
                string filePath = EditorUtility.OpenFilePanel("Load Config", Application.dataPath+"/Resources", "txt");
                if (!string.IsNullOrEmpty(filePath))
                {
                    string assetPath = "Assets" + filePath.Replace(Application.dataPath, "");
                    TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    if (textAsset != null)
                    {
                        TimeLineController controller = TimeLineReader.ReadController(JsonMapper.ToObject(textAsset.text));
                        if (controller != null)
                        {
                            editorController = new TimeLineEditorController(controller, editorSetting);
                        }
                        configPath = filePath;
                    }
                }
            }
            if (GUILayout.Button("Create", "toolbarbutton", GUILayout.Width(60)))
            {
                string filePath = EditorUtility.SaveFilePanel("Save Config", Application.dataPath+"/Resources", "tl_config", "txt");
                if (!string.IsNullOrEmpty(filePath))
                {
                    TimeLineController controller = new TimeLineController();
                    editorController = new TimeLineEditorController(controller, editorSetting);
                    editorSetting.isChanged = true;
                    configPath = filePath;
                }
            }

            if(editorController!=null)
            {
                if (GUILayout.Button("Save", "toolbarbutton", GUILayout.Width(120)))
                {
                    editorController.FillController();

                    string config = TimeLineWriter.WriteController(editorController.Controller).ToJson();
                    File.WriteAllText(configPath, config, new UTF8Encoding(false));

                    string assetPath = "Assets" + configPath.Replace(Application.dataPath, "");
                    AssetDatabase.ImportAsset(assetPath);
                }

                if(GUILayout.Button("Export For Server", "toolbarbutton", GUILayout.Width(120)))
                {

                }
            }
        }
    }
}
