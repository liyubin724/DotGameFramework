using DotEditor.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Tests.Window
{
    public class TestPopupWindow : DotPopupWindow
    {
        [MenuItem("Test/Window/Test Popup Win")]
        public static void ShowTestDraggablePopupWindow()
        {
            var win = GetPopupWindow<TestPopupWindow>();
            win.Show<TestPopupWindow>(new Rect(100, 100, 400, 300), true, true);
        }

        protected override void OnGUI()
        {
            base.OnGUI();
        }
    }
}
