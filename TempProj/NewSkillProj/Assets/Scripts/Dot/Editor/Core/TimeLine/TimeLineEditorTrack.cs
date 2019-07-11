using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;
using Dot.Core.TimeLine.Base.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.GenericMenu;

namespace DotEditor.Core.TimeLine
{
    public class TimeLineEditorTrack
    {
        public TimeLineEditorGroup Group { get; set; }
        
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
                if(isSelected)
                {
                    Group.OnTrackSelected(this);
                }else
                {
                    if(selectedItem!=null)
                    {
                        selectedItem.IsSelected = false;
                        selectedItem = null;
                    }
                }
                setting.isChanged = true;
            }
        }
        private TimeLineEditorItem selectedItem = null;
        public TimeLineEditorItem SelectedItem
        {
            get
            {
                return selectedItem;
            }

            private set
            {
                if (selectedItem != null && selectedItem != value)
                {
                    selectedItem.IsSelected = false;
                }
                selectedItem = value;
                if(selectedItem!=null && !selectedItem.IsSelected)
                {
                    selectedItem.IsSelected = true;
                }
            }
        }

        public TrackLine Track { get; private set; }
        private TimeLineEditorSetting setting = null;
        private List<TimeLineEditorItem> items = new List<TimeLineEditorItem>();
        public TimeLineEditorTrack(TrackLine tlTrack,TimeLineEditorSetting setting)
        {
            Track = tlTrack;
            this.setting = setting;
            foreach(var item in tlTrack.items)
            {
                TimeLineEditorItem tleItem = new TimeLineEditorItem(item,setting);
                tleItem.Track = this;
                items.Add(tleItem);
            }
            tlTrack.items.Clear();
        }
        
        public void DrawElement(Rect rect)
        {
            Rect rectLabel = new Rect(rect.x +1, rect.y + 1, rect.width - 2, rect.height - 2);
            GUI.Label(rectLabel, Track.Name, IsSelected ? "flow node 2" : "flow node 3");
            if (Event.current.type == EventType.MouseDown)
            {
                if (rectLabel.Contains(Event.current.mousePosition))
                {
                    IsSelected = true;
                    Event.current.Use();
                }
            }
        }

        public void DrawProperty()
        {
            GUILayout.Label("Track:");
            using (new UnityEditor.EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var sope = new EditorGUI.ChangeCheckScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        Track.Name = UnityEditor.EditorGUILayout.TextField("Name:", Track.Name);
                    }
                    if (sope.changed)
                        setting.isChanged = true;
                }
            }
            
            if (SelectedItem != null)
            {
                UnityEditor.EditorGUILayout.Space();
                SelectedItem.DrawProperty();
            }
        }

        public void DrawItem(Rect rect)
        {
            //GUI.Label(rect, "", IsSelected ? "flow node 5 on" : "flow node 5");
            
            for(var i =0;i<items.Count;i++)
            {
                items[i].DrawElement(rect);
            }

            if(Event.current.button == 1 && Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                  from type in assembly.GetExportedTypes()
                                  where type.IsSubclassOf(typeof(AEventItem)) || type.IsSubclassOf(typeof(AActionItem))
                                  select type);
                var fireTime = (Event.current.mousePosition.x + setting.scrollPos.x) / setting.pixelForSecond;

                MenuFunction2 callback = (type) =>
                {
                    AItem item = (AItem)((Type)type).Assembly.CreateInstance(((Type)type).FullName);
                    item.FireTime = fireTime;
                    TimeLineEditorItem eItem = new TimeLineEditorItem(item, setting);
                    eItem.Track = this;
                    items.Add(eItem);

                    eItem.IsSelected = true;
                    setting.isChanged = true;
                };

                foreach(var type in types)
                {
                    if(type.IsSubclassOf(typeof(AEventItem)))
                    {
                        TimeLineMarkAttribute attr = type.GetCustomAttribute<TimeLineMarkAttribute>();
                        if(attr!=null)
                            menu.AddItem(new GUIContent("Event/" + attr.Category + "/" + attr.Label), false, callback, type);
                    }
                }
                menu.AddSeparator("");
                foreach(var type in types)
                {
                    if (type.IsSubclassOf(typeof(AActionItem)))
                    {
                        TimeLineMarkAttribute attr = type.GetCustomAttribute<TimeLineMarkAttribute>();
                        if (attr != null)
                            menu.AddItem(new GUIContent("Action/" + attr.Category + "/" + attr.Label), false, callback, type);
                    }
                }
                menu.ShowAsContext();

                Event.current.Use();
            }
        }
        
        public void OnItemSelected(TimeLineEditorItem item)
        {
            GUI.FocusControl("");

            SelectedItem = item;
            IsSelected = true;

        }

        public void OnItemDelete(TimeLineEditorItem item)
        {
            items.Remove(item);
            if(SelectedItem == item)
            {
                SelectedItem = null;
            }
            setting.isChanged = true;
        }

        public void FillTrack()
        {
            Track.items.Clear();
            foreach(var item in items)
            {
                Track.items.Add(item.Item);
            }
            Track.items.Sort();
        }

        internal int[] GetDependOnItem(Type type) => (from item in items where item.Item.GetType() == type select item.Item.Index).ToArray();
    }
}
