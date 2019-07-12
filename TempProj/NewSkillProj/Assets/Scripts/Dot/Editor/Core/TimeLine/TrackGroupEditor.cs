using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Condition;
using Dot.Core.TimeLine.Base.Group;
using Dot.Core.TimeLine.Base.Tracks;
using System;
using System.Collections.Generic;
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
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var sope = new EditorGUI.ChangeCheckScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        Group.Name = EditorGUILayout.TextField("Name", Group.Name);
                        Group.TotalTime = EditorGUILayout.FloatField("TotalTime", Group.TotalTime);
                        Group.IsAwaysRun = EditorGUILayout.Toggle("IsAwaysRun", Group.IsAwaysRun);
                    }
                    if (sope.changed)
                        setting.isChanged = true;
                }

                EditorGUILayout.LabelField("Conditions:");
                using (new EditorGUILayout.HorizontalScope())
                {
                    if(GUILayout.Button("Begin Condition"))
                    {
                        ConditionEditor.ShowWin(Group.beginCondition, setting,(condition)=>
                        {
                            Group.beginCondition = condition;
                        });
                    }
                    if (GUILayout.Button("End Condition"))
                    {
                        ConditionEditor.ShowWin(Group.endCondition, setting, (condition) =>
                        {
                            ParallelCondition pc = (ParallelCondition)condition;
                            if(pc!=null)
                            {
                                Group.endCondition = pc;
                            }
                        });
                    }
                }
            }
            
            if (SelectedTrack != null)
            {
                EditorGUILayout.Space();
                SelectedTrack.DrawProperty();
            }
        }

        private int conditionSelectedIndex = 0;
        private ACondition DrawConditionProperty(ACondition conditon)
        {
            if(conditon == null)
            {
                if(GUILayout.Button("Create", "DropDownButton"))
                {
                    
                }
            }else
            {
                if(conditon.GetType() == typeof(AComposeCondition))
                {

                }else
                {

                }
            }

            return conditon;
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
