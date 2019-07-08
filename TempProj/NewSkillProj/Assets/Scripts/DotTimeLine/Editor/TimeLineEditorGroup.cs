using DotTimeLine.Base.Condition;
using DotTimeLine.Base.Groups;
using DotTimeLine.Base.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DotTimeLine
{
    public class TimeLineEditorGroup
    {
        public TimeLineEditorController Controller { get; set; }
        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                if(!isSelected)
                {
                    if(selectedTrack!=null)
                    {
                        selectedTrack.IsSelected = false;
                        selectedTrack = null;
                    }
                }
                setting.isChanged = true;
            }
        }
        private TimeLineEditorTrack selectedTrack = null;
        public TimeLineEditorTrack SelectedTrack
        {
            get
            {
                return selectedTrack;
            }
            private set
            {
                if (selectedTrack != null && selectedTrack != value)
                {
                    selectedTrack.IsSelected = false;
                }
                selectedTrack = value;
                if(!selectedTrack.IsSelected)
                {
                    selectedTrack.IsSelected = true;
                }
            }
        }

        public float TimeLength { get
            {
                return Group.TotalTime;
            } }

        private List<TimeLineEditorTrack> tracks = new List<TimeLineEditorTrack>();
        public float ItemHeight
        {
            get
            {
                return setting.trackHeight * Group.tracks.Count;
            }
        }
        
        public TimeLineGroup Group { get; private set; }
        private TimeLineEditorSetting setting = null;
        public TimeLineEditorGroup(TimeLineGroup tlGroup,TimeLineEditorSetting setting)
        {
            Group = tlGroup;
            foreach(var track in tlGroup.tracks)
            {
                TimeLineEditorTrack tleTrack = new TimeLineEditorTrack(track,setting);
                tleTrack.Group = this;
                tracks.Add(tleTrack);
            }
            tlGroup.tracks.Clear();

            this.setting = setting;
        }

        public void DrawElement(Rect rect)
        {
            Rect rectLabel = new Rect(rect.x + 3, rect.y + 3, rect.width - 6, rect.height - 6);
            GUI.Label(rectLabel, Group.Name, IsSelected ? "flow node 1" : "flow node 0");
        }

        public void DrawChilds(Rect rect)
        {
            using (new GUILayout.AreaScope(rect))
            {
                GUI.BeginClip(new Rect(0, 0, rect.width, rect.height));
                {
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        Rect groupRect = new Rect(0, i* setting.trackHeight, rect.width, setting.trackHeight);
                        tracks[i].DrawElement(groupRect);
                    }
                }
                GUI.EndClip();
            }
        }

        public void DrawOperation(Rect rect)
        {
            using (new GUILayout.AreaScope(rect))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("+", "ButtonLeft"))
                    {
                        TimeLineTrack tlTrack = new TimeLineTrack();
                        TimeLineEditorTrack tleTrack = new TimeLineEditorTrack(tlTrack,setting);
                        tleTrack.Group = this;
                        tracks.Add(tleTrack);
                        SelectedTrack = tleTrack;
                    }
                    int trackIndex = -1;
                    if (SelectedTrack != null)
                        trackIndex = tracks.IndexOf(SelectedTrack);
                    using (new EditorGUI.DisabledGroupScope(selectedTrack == null || tracks.Count == 1))
                    {
                        if (GUILayout.Button("-", "ButtonMid"))
                        {
                            tracks.RemoveAt(trackIndex);
                            if (trackIndex == tracks.Count)
                            {
                                trackIndex = tracks.Count - 1;
                            }
                            SelectedTrack = tracks[trackIndex];
                        }
                    }

                    using (new EditorGUI.DisabledGroupScope(selectedTrack == null || trackIndex == 0))
                    {
                        if (GUILayout.Button("\u2191", "ButtonMid"))
                        {
                            TimeLineEditorTrack preTrack = tracks[trackIndex - 1];
                            tracks[trackIndex - 1] = SelectedTrack;
                            tracks[trackIndex] = preTrack;
                        }
                    }
                    using (new EditorGUI.DisabledGroupScope(selectedTrack == null || trackIndex == tracks.Count - 1))
                    {
                        if (GUILayout.Button("\u2193", "ButtonRight"))
                        {
                            TimeLineEditorTrack nextTrack = tracks[trackIndex + 1];
                            tracks[trackIndex + 1] = SelectedTrack;
                            tracks[trackIndex] = nextTrack;
                        }
                    }
                }
            }
        }

        public void DrawProperty()
        {
            GUILayout.Label("Group:");
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var sope = new EditorGUI.ChangeCheckScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        Group.Name = EditorGUILayout.TextField("Name:", Group.Name);
                        Group.TotalTime = EditorGUILayout.FloatField("TotalTime:", Group.TotalTime);
                        Group.IsEnd = EditorGUILayout.Toggle("IsEnd:", Group.IsEnd);
                    }

                    EditorGUILayout.LabelField("Conditions:");
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Add"))
                        {
                            GenericMenu menu = new GenericMenu();
                            var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                         where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                         from type in assembly.GetExportedTypes()
                                         where type.IsSubclassOf(typeof(ATimeLineCondition))
                                         select type);
                            foreach (var t in types)
                            {
                                menu.AddItem(new GUIContent(t.Name), false, (type) =>
                                {
                                    ATimeLineCondition item = (ATimeLineCondition)((Type)type).Assembly.CreateInstance(((Type)type).FullName);
                                    Group.conditionCompose.conditions.Add(item);
                                }, t);
                            }
                            menu.ShowAsContext();
                        }
                        if (GUILayout.Button("Clear"))
                        {
                            Group.conditionCompose.conditions.Clear();
                        }
                    }
                    int deleteIndex = -1;
                    for (int i = 0; i < Group.conditionCompose.conditions.Count; ++i)
                    {
                        ATimeLineCondition condition = Group.conditionCompose.conditions[i];
                        PropertyInfo[] pInfos = condition.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField(condition.GetType().Name);
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    deleteIndex = i;
                                }
                            }

                            using (new EditorGUI.IndentLevelScope())
                            {
                                foreach (var pi in pInfos)
                                {
                                    TimeLineEditorLayout.PropertyInfoField(condition, pi);
                                }
                            }
                        }
                    }
                    if (deleteIndex >= 0)
                    {
                        Group.conditionCompose.conditions.RemoveAt(deleteIndex);
                    }
                    deleteIndex = -1;

                    if (sope.changed)
                        setting.isChanged = true;
                }
            }
            
            if (SelectedTrack != null)
            {
                EditorGUILayout.Space();
                SelectedTrack.DrawProperty();
            }
        }

        public void DrawTrack(Rect clipRect)
        {
            //GUI.Label(clipRect, "", "flow node 6 on");
            for (int i = 0; i < tracks.Count; ++i)
            {
                Rect itemRect = new Rect(0, i * setting.trackHeight, clipRect.width, setting.trackHeight);
                tracks[i].DrawItem(itemRect);
            }
        }

        
        public void OnTrackSelected(TimeLineEditorTrack track)
        {
            GUI.FocusControl("");

            SelectedTrack = track;
            IsSelected = true;
        }

        public void FillGroup()
        {
            Group.tracks.Clear();
            foreach(var track in tracks)
            {
                Group.tracks.Add(track.Track);

                track.FillTrack();
            }
        }
    }
}
