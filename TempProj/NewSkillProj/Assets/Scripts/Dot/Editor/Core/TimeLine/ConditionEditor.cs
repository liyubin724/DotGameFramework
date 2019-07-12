using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Condition;
using DotEditor.Core.EGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    class ConditionWindowData
    {
        public int id;
        public int parent;
        public bool isCompose;
        public ACondition condition;
        public Rect rect = Rect.zero;

        public AComposeCondition GetComposeCondition()
        {
            if (isCompose)
            {
                return (AComposeCondition)condition;
            }
            return null;
        }
    }

    public enum ConditionOperateType
    {
        None,
        New,
        Delete,
    }

    public class ConditionOperateData
    {
        public ACondition condition;
        public ConditionOperateType operateType = ConditionOperateType.None;
        public AComposeCondition parentCondition;
    }


    public delegate void OnConditionClose(ACondition condition);

    public class ConditionEditor : EditorWindow
    {
        private static readonly float INNER_WIN_WIDTH = 150;
        private static readonly float INNER_WIN_HEIGHT = 180;
        private static readonly float INNER_WIN_WIDTH_SPACE = 20;
        private static readonly float INNER_WIN_HEIGHT_SPACE = 40;

        private ACondition condition;
        private EditorSetting setting;
        private OnConditionClose closeCallback;
        private Dictionary<int, ConditionWindowData> dataDic = new Dictionary<int, ConditionWindowData>();

        private ConditionOperateData operateData = null;

        public static void ShowWin(ACondition condition, EditorSetting setting, OnConditionClose callback)
        {
            var win = GetWindow<ConditionEditor>();
            win.condition = condition;
            win.setting = setting;
            win.closeCallback = callback;
            win.wantsMouseMove = true;
            win.Show();
        }

        private void OnLostFocus()
        {
            EditorWindow.FocusWindowIfItsOpen<ConditionEditor>();
        }

        private void OnDestroy()
        {
            closeCallback?.Invoke(condition);
        }

        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80;
            if (condition == null && Event.current.button == 1 && Event.current.type == EventType.MouseUp)
            {
                CreateNewMenu(null);
            }
            if (condition != null)
            {
                dataDic.Clear();
                winID = 0;

                BeginWindows();
                {
                    DrawInnerWin(condition, null, 0, 0);
                }
                EndWindows();
            }

            if(operateData!=null)
            {
                if(operateData.operateType == ConditionOperateType.New)
                {
                    if(condition == null)
                    {
                        condition = operateData.condition;
                    }else
                    {
                        operateData.parentCondition.conditions.Add(operateData.condition);
                    }
                }
            }
            operateData = null;
        }
        private int winID = 0;

        private void DrawInnerWin(ACondition c, ConditionWindowData pData,int rowIndex,int colIndex)
        {
            ConditionWindowData data = new ConditionWindowData();
            data.id = winID;
            data.condition = c;
            if (c.GetType().IsSubclassOf(typeof(AComposeCondition)))
            {
                data.isCompose = true;
            }
            dataDic.Add(winID, data);

            float x = colIndex * (INNER_WIN_WIDTH + INNER_WIN_WIDTH_SPACE);
            float y = rowIndex * (INNER_WIN_HEIGHT + INNER_WIN_HEIGHT_SPACE);
            data.rect = GUI.Window(winID, new Rect(x, y, INNER_WIN_WIDTH, INNER_WIN_HEIGHT), (id) =>
            {
                using (new GUILayout.AreaScope(new Rect(2, 20, INNER_WIN_WIDTH - 4, INNER_WIN_HEIGHT - 20)))
                {
                    DrawInnerWinContent(data);
                }
                GUI.DragWindow();
            }, new GUIContent("" + winID));


            if (pData != null)
            {
                data.parent = pData.id;
                Handles.color = Color.black;
                Rect parentRect = dataDic[pData.id].rect;
                Handles.DrawLine(new Vector3(parentRect.x + parentRect.width * 0.5f, parentRect.y+parentRect.height, 0), new Vector3(data.rect.x + data.rect.width * 0.5f, data.rect.y, 0));
            }

            
            winID++;
            if (data.isCompose)
            {
                rowIndex++;
                foreach (var child in data.GetComposeCondition().conditions)
                {
                    DrawInnerWin(child, data,rowIndex,colIndex);
                    colIndex++;
                }
            }
        }

        private void CreateNewMenu(AComposeCondition parent)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var type in setting.ConditionTypes)
            {
                TimeLineMarkAttribute attr = type.GetCustomAttribute<TimeLineMarkAttribute>();
                if (attr != null)
                    menu.AddItem(new GUIContent(attr.Category + "/" + attr.Label), false, () =>
                    {
                        var newCondition = (ACondition)type.Assembly.CreateInstance(type.FullName);

                        operateData = new ConditionOperateData();
                        operateData.condition = newCondition;
                        operateData.parentCondition = parent;
                        operateData.operateType = ConditionOperateType.New;
                    });
            }
            menu.ShowAsContext();
        }


        private void DrawInnerWinContent(ConditionWindowData data)
        {
            GUILayout.Label(data.condition.GetType().Name);
            if (data.isCompose)
            {
                GUILayout.Label("Child:" + data.GetComposeCondition().conditions.Count);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add"))
                {
                    CreateNewMenu(data.GetComposeCondition());
                }
            }
            else
            {
                PropertyInfo[] pInfos = data.condition.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var pi in pInfos)
                {
                    EditorGUILayoutUtil.PropertyInfoField(data.condition, pi);
                }
            }
            if(!data.condition.IsReadonly)
            {
                if (GUILayout.Button("delete"))
                {

                }
            }
        }


    }
}
