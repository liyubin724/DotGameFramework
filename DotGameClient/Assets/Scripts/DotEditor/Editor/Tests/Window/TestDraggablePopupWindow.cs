using DotEditor.Core.UI;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Tests.Window
{
    public class TestDraggablePopupWindow : DraggablePopupWindow
    {
        [MenuItem("Test/Window/Test Draggable Popup Win")]
        public static void ShowTestDraggablePopupWindow()
        {
            var win = GetDraggableWindow<TestDraggablePopupWindow>();
            win.Show<TestDraggablePopupWindow>(new Rect(100, 100, 400, 300), true,true);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
        }
    }
}
