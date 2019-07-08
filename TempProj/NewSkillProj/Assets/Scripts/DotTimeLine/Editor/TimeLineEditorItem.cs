using DotTimeLine.Base.Items;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DotTimeLine
{
    public class TimeLineEditorItem
    {
        public TimeLineEditorTrack Track { get; set; }
        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if(isSelected!=value)
                {
                    isSelected = value;
                    if(isSelected)
                    {
                        Track.OnItemSelected(this);
                    }
                }
                setting.isChanged = true;
                GUI.FocusControl("");
            }
        }

        public ATimeLineItem Item { get; private set; }
        private TimeLineEditorSetting setting = null;
        public TimeLineEditorItem(ATimeLineItem tlItem,TimeLineEditorSetting setting)
        {
            Item = tlItem;
            this.setting = setting;
        }

        public void DrawProperty()
        {
            GUILayout.Label(Item.GetType().Name + ":");
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using (var sope = new EditorGUI.ChangeCheckScope())
                    {
                        Item.Index = EditorGUILayout.IntField("Index:", Item.Index);
                        Item.FireTime = EditorGUILayout.FloatField("Fire Time:", Item.FireTime);
                        if (Item.FireTime > Track.Group.Group.TotalTime)
                        {
                            Item.FireTime = Track.Group.Group.TotalTime;
                        }
                        if (Item.GetType().IsSubclassOf(typeof(ATimeLineActionItem)))
                        {
                            var actionItem = (ATimeLineActionItem)Item;
                            actionItem.Duration = EditorGUILayout.FloatField("Duration:", actionItem.Duration);
                            if (actionItem.Duration <= 0)
                            {
                                actionItem.Duration = 0.01f;
                            }
                            if (actionItem.FireTime + actionItem.Duration > Track.Group.Group.TotalTime)
                            {
                                actionItem.FireTime = Track.Group.Group.TotalTime - actionItem.Duration;
                            }
                        }

                        PropertyInfo[] pInfos = Item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        foreach (var pi in pInfos)
                        {
                            TimeLineEditorLayout.PropertyInfoField(Item, pi);
                        }

                        if (sope.changed)
                            setting.isChanged = true;
                    }
                }
            }
        }

        private bool isPressed = false;
        public void DrawElement(Rect rect)
        {
            Rect itemRect = Rect.zero;
            itemRect.x = Item.FireTime * setting.pixelForSecond - setting.scrollPos.x;
            itemRect.y = rect.y;
            float timeLen = setting.timeStep;
            if(Item.GetType().IsSubclassOf(typeof(ATimeLineActionItem)))
            {
                timeLen = ((ATimeLineActionItem)Item).Duration;
            }
            itemRect.width = setting.pixelForSecond * timeLen;
            itemRect.height = setting.trackHeight;

            string name = Item.GetType().Name;
            TimeLineItemAttribute attr = Item.GetType().GetCustomAttribute<TimeLineItemAttribute>();
            if(attr !=null)
            {
                name = attr.Label;
            }
            name += "(" + Item.Index + ")";

            GUI.Label(itemRect, name, IsSelected ? "flow node 6" : "flow node 5");

            if(Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    if (itemRect.Contains(Event.current.mousePosition))
                    {
                        IsSelected = true;
                        isPressed = true;

                        setting.isChanged = true;
                        Event.current.Use();
                    }
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    if (isPressed)
                    {
                        isPressed = false;
                        Event.current.Use();
                    }
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    if (isPressed)
                    {
                        Vector2 deltaPos = Event.current.delta;
                        float deltaTime = deltaPos.x / setting.pixelForSecond;
                        Item.FireTime += deltaTime;

                        setting.isChanged = true;
                    }
                }
            }else if(Event.current.button == 1)
            {
                if (itemRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete"), false, () =>
                        {
                            Track.OnItemDelete(this);
                        });
                        menu.ShowAsContext();

                        Event.current.Use();
                    }
                }
            }

            if(IsSelected && Event.current.type == EventType.KeyUp)
            {
                if(Event.current.keyCode == KeyCode.Delete)
                {
                    Track.OnItemDelete(this);
                    Event.current.Use();
                }
                else if(Event.current.keyCode == KeyCode.LeftArrow)
                {
                    int num = Mathf.FloorToInt(Item.FireTime / setting.timeStep);
                    float val = num * setting.timeStep;
                    if(Mathf.Abs(Item.FireTime - val) > 0.01)
                    {
                        Item.FireTime = val;
                    }else
                    {
                        Item.FireTime -= setting.timeStep;
                    }
                    Event.current.Use();
                }
                else if(Event.current.keyCode == KeyCode.RightArrow)
                {
                    int num = Mathf.CeilToInt(Item.FireTime / setting.timeStep);
                    float val = num * setting.timeStep;
                    if (Mathf.Abs(Item.FireTime - val) > 0.01)
                    {
                        Item.FireTime = val;
                    }
                    else
                    {
                        Item.FireTime += setting.timeStep;
                    }
                    Event.current.Use();
                }
            }
        }
    }
}
