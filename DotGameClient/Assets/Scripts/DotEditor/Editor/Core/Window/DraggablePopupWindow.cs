using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.UI
{
    /// <summary>
    /// 通过继承DraggablePopupWindow可以生成一个可拖动的弹框
    /// 
    /// Example : 示例会生成一个不带任何内容的空的弹板 , 通过重写OnGUI为弹板添加关闭按钮
    /// public class TestDraggablePopupWindow:DraggablePopupWindow
    /// {
    ///     [MenuItem("Test/Window/Test Draggable Popup Win")]
    ///     public static void ShowTestDraggablePopupWindow()
    ///     {
    ///         var win = GetDraggableWindow<TestDraggablePopupWindow>();
    ///         win.Show<TestDraggablePopupWindow>(new Rect(100, 100, 400, 300), true);
    ///     }
    ///     
    ///     protected override void OnGUI()
    ///     {
    ///         base.OnGUI();
    ///         if(GUILayout.Button("Close")) Close();
    ///     }
    /// }
    /// 
    /// </summary>
    public abstract class DraggablePopupWindow : EditorWindow
    {
        private Vector2 _offset;

        public static T GetDraggableWindow<T>() where T : DraggablePopupWindow
        {
            var array = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
            var t = (array.Length <= 0) ? null : array[0];

            return t ?? CreateInstance<T>();
        }

        public void Show<T>(Rect position, bool focus = true) where T : DraggablePopupWindow
        {
            this.minSize = position.size;
            this.position = position;

            if (focus) this.Focus();
            this.ShowPopup();
        }

        protected virtual void DrawBackground()
        {
            Rect winRect = new Rect(Vector2.zero, position.size);
            EditorGUI.DrawRect(winRect, EditorGUIUtil.BorderColor);

            Rect backgroundRect = new Rect(Vector2.one, position.size - new Vector2(2f, 2f));
            EditorGUI.DrawRect(backgroundRect, EditorGUIUtil.BackgroundColor);
        }

        protected void OnGUIDrag()
        {
            var e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                _offset = position.position - GUIUtility.GUIToScreenPoint(e.mousePosition);
            }

            if (e.button == 0 && e.type == EventType.MouseDrag)
            {
                var mousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                position = new Rect(mousePos + _offset, position.size);
            }
        }

        protected virtual void OnGUI()
        {
            DrawBackground();

            OnGUIDrag();
        }
    }

}
