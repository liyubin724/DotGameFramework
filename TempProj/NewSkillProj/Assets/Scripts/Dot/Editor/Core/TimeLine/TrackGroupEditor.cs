using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Group;
using Dot.Core.TimeLine.Base.Tracks;
using DotEditor.Core.EGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public class TrackGroupEditor
    {
        public ControllerEditor Controller { get; set; }
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
        private TrackLineEditor selectedTrack = null;
        public TrackLineEditor SelectedTrack
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

        public float TimeLength
        {
            get
            {
                return Group.TotalTime;
            }
        }

        private List<TrackLineEditor> tracks = new List<TrackLineEditor>();
        public float ItemHeight
        {
            get
            {
                return setting.trackHeight * Group.tracks.Count;
            }
        }
        
        public TrackGroup Group { get; private set; }
        private EditorSetting setting = null;
        public TrackGroupEditor(TrackGroup tlGroup,EditorSetting setting)
        {
            Group = tlGroup;
            foreach(var track in tlGroup.tracks)
            {
                TrackLineEditor tleTrack = new TrackLineEditor(track,setting);
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
                        TrackLine tlTrack = new TrackLine();
                        TrackLineEditor tleTrack = new TrackLineEditor(tlTrack,setting);
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
                            TrackLineEditor preTrack = tracks[trackIndex - 1];
                            tracks[trackIndex - 1] = SelectedTrack;
                            tracks[trackIndex] = preTrack;

                            setting.isChanged = true;
                        }
                    }
                    using (new EditorGUI.DisabledGroupScope(selectedTrack == null || trackIndex == tracks.Count - 1))
                    {
                        if (GUILayout.Button("\u2193", "ButtonRight"))
                        {
                            TrackLineEditor nextTrack = tracks[trackIndex + 1];
                            tracks[trackIndex + 1] = SelectedTrack;
                            tracks[trackIndex] = nextTrack;

                            setting.isChanged = true;
                        }
                    }
                }
            }
        }

        public void DrawProperty()
        {
            GUILayout.Label("Group:");
            using (new UnityEditor.EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var sope = new EditorGUI.ChangeCheckScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        Group.Name = UnityEditor.EditorGUILayout.TextField("Name:", Group.Name);
                        Group.TotalTime = UnityEditor.EditorGUILayout.FloatField("TotalTime:", Group.TotalTime);
                        Group.IsAwaysRun = UnityEditor.EditorGUILayout.Toggle("IsEnd:", Group.IsAwaysRun);
                    }

                    //UnityEditor.EditorGUILayout.LabelField("Conditions:");
                    //using (new UnityEditor.EditorGUILayout.HorizontalScope())
                    //{
                    //    if (GUILayout.Button("Add"))
                    //    {
                    //        GenericMenu menu = new GenericMenu();
                    //        var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    //                     where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                    //                     from type in assembly.GetExportedTypes()
                    //                     where type.IsSubclassOf(typeof(ATimeLineCondition))
                    //                     select type).ToList();
                    //        types.ForEach((t) =>
                    //        {
                    //            menu.AddItem(new GUIContent(t.Name), false, (type) =>
                    //            {
                    //                ATimeLineCondition item = (ATimeLineCondition)((Type)type).Assembly.CreateInstance(((Type)type).FullName);
                    //                Group.conditionCompose.conditions.Add(item);
                    //            }, t);
                    //        });
                    //        menu.ShowAsContext();
                    //    }
                    //    if (GUILayout.Button("Clear"))
                    //    {
                    //        Group.conditionCompose.conditions.Clear();
                    //    }
                    //}
                    //int deleteIndex = -1;
                    //for (int i = 0; i < Group.conditionCompose.conditions.Count; ++i)
                    //{
                    //    ATimeLineCondition condition = Group.conditionCompose.conditions[i];
                    //    PropertyInfo[] pInfos = condition.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    //    using (new UnityEditor.EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    //    {
                    //        using (new UnityEditor.EditorGUILayout.HorizontalScope())
                    //        {
                    //            UnityEditor.EditorGUILayout.LabelField(condition.GetType().Name);
                    //            GUILayout.FlexibleSpace();
                    //            if (GUILayout.Button("-", GUILayout.Width(20)))
                    //            {
                    //                deleteIndex = i;
                    //            }
                    //        }

                    //        using (new EditorGUI.IndentLevelScope())
                    //        {
                    //            foreach (var pi in pInfos)
                    //            {
                    //                EditorGUILayoutUtil.PropertyInfoField(condition, pi);
                    //            }
                    //        }
                    //    }
                    //}
                    //if (deleteIndex >= 0)
                    //{
                    //    Group.conditionCompose.conditions.RemoveAt(deleteIndex);
                    //}
                    //deleteIndex = -1;

                    if (sope.changed)
                        setting.isChanged = true;
                }
            }
            
            if (SelectedTrack != null)
            {
                UnityEditor.EditorGUILayout.Space();
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
        
        public void OnTrackSelected(TrackLineEditor track)
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

        internal int[] GetDependOnItem(Type type)
        {
            List<int> values = new List<int>();
            tracks.ForEach((track) =>
            {
                values.AddRange(track.GetDependOnItem(type));
            });
            return values.ToArray();
        }
    }
}
