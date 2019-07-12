using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Condition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public class InnerWindowData
    {
        public int id;
        public int parentID;
        public bool isCompose = false;
        public ACondition condition;
        public AComposeCondition composeCondition;
        public List<InnerWindowData> childs = new List<InnerWindowData>();

        public int GetDepth()
        {
            int maxDepth = 0;
            if(childs.Count == 0)
            {
                return 1;
            }
            else
            {
                foreach(var c in childs)
                {
                    int d = c.GetDepth();
                    if(d>maxDepth)
                    {
                        maxDepth = d;
                    }
                }
                return maxDepth + 1;
            }
        }
        public int GetBreadth()
        {
            List<InnerWindowData> layerData = new List<InnerWindowData>();
            List<InnerWindowData> tempData = new List<InnerWindowData>();
            layerData.Add(this);
            List<int> counts = new List<int>();
            while(layerData.Count>0)
            {
                counts.Add(layerData.Count);
                tempData.AddRange(layerData);
                layerData.Clear();
                foreach(var d in tempData)
                {
                    if(d.childs.Count>0)
                    {
                        layerData.AddRange(d.childs);
                    }
                }
                tempData.Clear();
            }

            return Mathf.Max(counts.ToArray());
        }
    }

    public class TimeLineConditionWindow : EditorWindow
    {
        private ACondition condition = null;
        private EditorSetting setting = null;

        private Dictionary<int, InnerWindowData> innerWindows = new Dictionary<int, InnerWindowData>();


        private InnerWindowData rootData = null;
        private static readonly float INNER_WIN_WIDTH = 120;
        private static readonly float INNER_WIN_HEIGHT = 150;
        private static readonly float INNER_WIN_WIDTH_SPACE = 20;
        private static readonly float INNER_WIN_HEIGHT_SPACE = 60;

        public static TimeLineConditionWindow ShowWindow(ACondition conditon,EditorSetting setting)
        {
            TimeLineConditionWindow tlcWin = GetWindow<TimeLineConditionWindow>();
            tlcWin.SetData(conditon, setting);
            tlcWin.ShowAuxWindow();
            
            return tlcWin;
        }

        private void OnEnable()
        {
            rootData = new InnerWindowData()
            {
                id = 1,
                parentID = -1,
                isCompose = true,
            };
        }

        public void SetData(ACondition con,EditorSetting setting)
        {
            this.condition = con;
            this.setting = setting;
            if(condition!=null)
            {
                CreateInnerWinData(rootData, con);
            }
        }

        private void CreateInnerWinData(InnerWindowData parentData,ACondition curCondition)
        {
            InnerWindowData winData = new InnerWindowData();
            winData.parentID = parentData.id;
            winData.id = winData.parentID * 10 + parentData.childs.Count;

            parentData.childs.Add(winData);

            Type type = curCondition.GetType();
            if(type == typeof(AComposeCondition))
            {
                winData.isCompose = true;
                winData.composeCondition = (AComposeCondition)curCondition;

                foreach(var c in ((AComposeCondition)curCondition).conditions)
                {
                    CreateInnerWinData(winData, c);
                }
            }else
            {
                winData.isCompose = false;
                winData.condition = (AComposeCondition)curCondition;
            }
        }

        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            int depth = rootData.GetDepth();
            int breadth= rootData.GetBreadth();

            GUILayout.Label("depth="+depth+", breadth = "+breadth);
            float width = breadth * (INNER_WIN_WIDTH_SPACE + INNER_WIN_WIDTH);
            float height = breadth * (INNER_WIN_HEIGHT + INNER_WIN_HEIGHT_SPACE);
            Rect maxRect = new Rect(0, 0, width, height);

            using (new GUILayout.AreaScope(new Rect(0, 0, position.width, position.height)))
            {
                using (var scope = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    GUILayout.Label("", GUILayout.Width(width), GUILayout.Height(height));

                    scrollPos = scope.scrollPosition;
                }

                using (new GUI.ClipScope(new Rect(0, 0, position.width, position.height)))
                {
                    BeginWindows();
                    {


                    }
                    EndWindows();
                }
            }
        }

        private void DrawInnerWindow(InnerWindowData data)
        {
            
        }

    }

}
