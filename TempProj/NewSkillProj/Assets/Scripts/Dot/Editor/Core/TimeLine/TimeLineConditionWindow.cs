using Dot.Core.TimeLine.Base.Condition;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.TimeLine
{
    public class TimeLineConditionWindow : EditorWindow
    {
        private ACondition condition = null;
        private EditorSetting setting = null;
        public static TimeLineConditionWindow ShowWindow(ACondition conditon,EditorSetting setting)
        {
            TimeLineConditionWindow tlcWin = GetWindow<TimeLineConditionWindow>(true,"",true);
            tlcWin.condition = conditon;
            tlcWin.setting = setting;
            tlcWin.ShowTab();
            
            return tlcWin;
        }

        private void OnGUI()
        {
            
        }

    }

    public class TLCWindow : PopupWindowContent
    {
        public override void OnGUI(Rect rect)
        {
            throw new System.NotImplementedException();
        }
    }

}
