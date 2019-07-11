using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Group;
using DotEditor.Core.EGUI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public class ControllerEditor
    {
        public float TimeLength
        {
            get
            {
                if(SelectedGroup !=null)
                {
                    return SelectedGroup.TimeLength+3;
                }

                return 10;
            }
        }
        public float ItemHeight
        {
            get
            {
                if(SelectedGroup!=null)
                {
                    return SelectedGroup.ItemHeight;
                }
                return 0;
            }
        }
        public Rect ItemClipRect { get; private set; }
        public TimeLineController Controller { get ; private set; }
        
        private EditorSetting setting = null;
        private List<TrackGroupEditor> groups = new List<TrackGroupEditor>();
        private TrackGroupEditor selectedGroup = null;
        public TrackGroupEditor SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
            private set
            {
                if(selectedGroup!=null && selectedGroup!=value)
                {
                    selectedGroup.IsSelected = false;
                }
                selectedGroup = value;
                selectedGroup.IsSelected = true;
                setting.isChanged = true;
            }
        }

        public ControllerEditor(TimeLineController tlController,EditorSetting setting)
        {
            Controller = tlController;
            this.setting = setting;
            tlController.groups.ForEach((group) =>
            {
                var tleGroup = new TrackGroupEditor(group,setting);
                tleGroup.Controller = this;
                groups.Add(tleGroup);
            });
            tlController.groups.Clear();
        }
        
        public void OnGUI(Rect rect)
        {
            var groupOperRect = new Rect(rect.x, rect.y, setting.groupWidth, setting.timeHeight);
            using (new GUILayout.AreaScope(groupOperRect))
            {
                DrawOperation(groupOperRect);
            }
             
            var timeX = rect.x + setting.groupWidth + setting.trackWidth;
            var timeY = rect.y;
            var timeWidth = rect.width - setting.trackWidth - setting.groupWidth - setting.propertyWidth;
            var timeHeight = setting.timeHeight;
            var timeRect = new Rect(timeX, timeY, timeWidth, timeHeight);
            using (new GUILayout.AreaScope(timeRect))
            {
                DrawTimeLine(timeRect);
            }
            
            var groupX = rect.x;
            var groupY = rect.y + setting.timeHeight;
            var groupWidth = setting.groupWidth;
            var groupheight = rect.height - setting.timeHeight;
            var groupRect = new Rect(groupX, groupY, groupWidth, groupheight);
            using (new GUILayout.AreaScope(groupRect))
            {
                DrawChilds(groupRect);
            }

            var trackX = rect.x + setting.groupWidth;
            var trackY = rect.y + setting.timeHeight;
            var trackWidth = setting.trackWidth;
            var trackHeight = rect.height - setting.timeHeight;
            var trackRect = new Rect(trackX, trackY, trackWidth, trackHeight);
            var trackOperRect = new Rect(rect.x + setting.groupWidth, rect.y, setting.trackWidth, setting.timeHeight);
            if (selectedGroup != null)
            {
                selectedGroup.DrawOperation(trackOperRect);
                using (var ccs = new EditorGUI.ChangeCheckScope())
                {
                    selectedGroup.DrawChilds(trackRect);

                    if (ccs.changed)
                        setting.isChanged = true;
                }
            }

            var itemClipX = rect.x + setting.trackWidth + setting.groupWidth;
            var itemClipY = rect.y + setting.timeHeight;
            var itemClipWidth = rect.width - setting.groupWidth - setting.trackWidth - setting.propertyWidth;
            var itemClipHeight = rect.height - setting.timeHeight;
            ItemClipRect = new Rect(itemClipX, itemClipY, itemClipWidth, itemClipHeight);

            var propertyX = rect.x + rect.width - setting.propertyWidth;
            var propertyY = rect.y + setting.timeHeight;
            var propertyWidth = setting.propertyWidth-4;
            var propertyHeight = rect.height - setting.timeHeight;
            var propertyRect = new Rect(propertyX, propertyY, propertyWidth, propertyHeight);

            if(SelectedGroup!=null)
            {
                using (new GUILayout.AreaScope(ItemClipRect))
                {
                    UpdateScrollPos();

                    using (new GUI.ClipScope(new Rect(0, 0, ItemClipRect.width - 16, ItemClipRect.height - 16)))
                    {
                        using (var sope = new EditorGUI.ChangeCheckScope())
                        {
                            SelectedGroup.DrawTrack(new Rect(0, 0, ItemClipRect.width - 16, ItemClipRect.height - 16));

                            if (sope.changed)
                                setting.isChanged = true;
                        }
                    }
                    DrawItemLine(ItemClipRect);
                }
                
                using (new GUILayout.AreaScope(propertyRect))
                {
                    EditorGUIUtility.labelWidth = 120;
                    using (new UnityEditor.EditorGUILayout.VerticalScope())
                    {
                        SelectedGroup.DrawProperty();
                    }
                }
            }

            EditorGUIUtil.DrawAreaLine(groupOperRect,Color.gray);
            EditorGUIUtil.DrawAreaLine(trackOperRect, Color.gray);
            EditorGUIUtil.DrawAreaLine(timeRect, Color.gray);
            EditorGUIUtil.DrawAreaLine(ItemClipRect, Color.yellow);
            EditorGUIUtil.DrawAreaLine(groupRect, Color.gray);
            EditorGUIUtil.DrawAreaLine(trackRect, Color.gray);
            EditorGUIUtil.DrawAreaLine(propertyRect, Color.gray);
        }

        private void UpdateScrollPos()
        {
            using (var scop = new UnityEditor.EditorGUILayout.ScrollViewScope(setting.scrollPos))
            {
                float scrollWith = Mathf.Max(TimeLength * setting.pixelForSecond, ItemClipRect.width);
                float scrollHeight = Mathf.Max(ItemHeight, ItemClipRect.height);

                GUILayout.Label("", GUILayout.Width(scrollWith), GUILayout.Height(scrollHeight - 20));

                setting.scrollPos = scop.scrollPosition;
            }
        }

        private void DrawTimeLine(Rect rect)
        {
            using (new GUI.ClipScope(new Rect(0, 0, rect.width, rect.height)))
            {
                int start = (int)(setting.scrollPos.x / setting.PixelForStep);
                int end = (int)((setting.scrollPos.x + rect.width) / setting.PixelForStep);
                for(int i = start;i<end;i++)
                {
                    var x = (setting.PixelForStep * i - setting.scrollPos.x);
                    if(i%10 == 0)
                    {
                        Handles.color = new Color(0, 0, 0, 0.8f);
                        Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, rect.height * 0.8f, 0));
                        GUI.Label(new Rect(x, 5, 40, 40), (i * setting.timeStep).ToString("F0"));
                    }
                    else if(i%5 == 0)
                    {
                        Handles.color = new Color(0, 0, 0, 0.5f);
                        Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, rect.height * 0.5f, 0));
                    }
                    else
                    {
                        Handles.color = new Color(0, 0, 0, 0.5f);
                        Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, rect.height * 0.3f, 0));
                    }
                }
            }
        }

        public void DrawOperation(Rect rect)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("+", "ButtonLeft"))
                {
                    TrackGroup tlGroup = TrackGroup.CreateNew();
                    TrackGroupEditor tleGroup = new TrackGroupEditor(tlGroup,setting);
                    tleGroup.Controller = this;
                    groups.Add(tleGroup);
                    
                    SelectedGroup = tleGroup;
                }
                int groupIndex = -1;
                if(SelectedGroup!=null)
                    groupIndex = groups.IndexOf(SelectedGroup);
                using (new EditorGUI.DisabledGroupScope(SelectedGroup==null || groups.Count == 1))
                {
                    if (GUILayout.Button("-", "ButtonMid"))
                    {
                        groups.RemoveAt(groupIndex);
                        if (groupIndex == groups.Count)
                        {
                            groupIndex = groups.Count - 1;
                        }
                        SelectedGroup = groups[groupIndex];
                    }
                }
                using (new EditorGUI.DisabledGroupScope(SelectedGroup == null || groupIndex == 0))
                {
                    if (GUILayout.Button("\u2191", "ButtonMid"))
                    {
                        TrackGroupEditor preGroup = groups[groupIndex - 1];
                        groups[groupIndex - 1] = selectedGroup;
                        groups[groupIndex] = preGroup;

                        setting.isChanged = true;
                    }
                }
                using (new EditorGUI.DisabledGroupScope(SelectedGroup == null || groupIndex == groups.Count - 1))
                {
                    if (GUILayout.Button("\u2193", "ButtonRight"))
                    {
                        TrackGroupEditor nextGroup = groups[groupIndex + 1];
                        groups[groupIndex + 1] = selectedGroup;
                        groups[groupIndex] = nextGroup;

                        setting.isChanged = true;
                    }
                }
            }
        }

        public void DrawChilds(Rect rect)
        {
            GUI.BeginClip(new Rect(0, 0, rect.width, rect.height - 16));
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    Rect groupRect = new Rect(0, i*setting.groupHeight, rect.width, setting.groupHeight);
                    groups[i].DrawElement(groupRect);

                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (groupRect.Contains(Event.current.mousePosition))
                        {
                            SelectedGroup = groups[i];

                            Event.current.Use();
                        }
                    }
                }
            }
            GUI.EndClip();
        }
        
        public void DrawItemLine(Rect rect)
        {
            float piexlStep = setting.pixelForSecond * setting.timeStep;
            using (new GUI.ClipScope(new Rect(0, 0, rect.width, rect.height-16)))
            {
                int startX = (int)(setting.scrollPos.x / piexlStep);
                int endX = (int)((setting.scrollPos.x + rect.width) / piexlStep);
                for(int i = startX;i<endX;i++)
                {
                    var x = (piexlStep * i - setting.scrollPos.x);
                    if(i%10 == 0)
                    {
                        Handles.color = new Color(0, 0, 0, 0.8f);
                        Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, rect.height , 0));
                    }
                    else
                    {
                        Handles.color = new Color(0, 0, 0, 0.2f);
                        Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, rect.height , 0));
                    }
                }

                int startY = (int)(setting.scrollPos.y / setting.trackHeight);
                int endY = (int)((setting.scrollPos.y + rect.height - 16) / setting.trackHeight);
                for(int i = startY; i<=endY;i++)
                {
                    var y = setting.trackHeight * i;
                    Handles.color = new Color(0, 0, 0, 0.8f);
                    Handles.DrawLine(new Vector3(0, y, 0), new Vector3(rect.width-16, y, 0));
                }

                float totalTimeLineX = setting.pixelForSecond * SelectedGroup.Group.TotalTime - setting.scrollPos.x;
                if(rect.Contains(new Vector2(totalTimeLineX,rect.y)))
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(new Vector3(totalTimeLineX, 0, 0), new Vector3(totalTimeLineX, rect.height, 0));
                }
            }
        }

        public void FillController()
        {
            Controller.groups.Clear();
            foreach(var g in groups)
            {
                Controller.groups.Add(g.Group);
                g.FillGroup();
            }
        }

        internal int[] GetDependOnItem(Type type)
        {
            List<int> values = new List<int>();
            groups.ForEach((group) =>
            {
                values.AddRange(group.GetDependOnItem(type));
            });
            return values.ToArray();
        }
    }
}
