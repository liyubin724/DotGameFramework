using DotEditor.Core.Util;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.EGUI
{
    public static class EditorGUIUtil
    {
        public static Color BorderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? new Color(0.13f, 0.13f, 0.13f) : new Color(0.51f, 0.51f, 0.51f);
            }
        }

        public static Color BackgroundColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.83f, 0.83f, 0.83f);
            }
        }

        private static Texture2D folderIcon = null;
        public static Texture2D FolderIcon
        {
            get
            {
                if (folderIcon == null)
                {
                    folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
                }
                return folderIcon;
            }
        }

        public static void DrawAreaLine(Rect rect, Color color)
        {
            Handles.color = color;

            var points = new Vector3[] {
                new Vector3(rect.x, rect.y, 0),
                new Vector3(rect.x + rect.width, rect.y, 0),
                new Vector3(rect.x + rect.width, rect.y + rect.height, 0),
                new Vector3(rect.x, rect.y + rect.height, 0),
            };

            var indexies = new int[] {
                0, 1, 1, 2, 2, 3, 3, 0,
            };

            Handles.DrawLines(points, indexies);
        }

        private static Stack<float> labelWidthStack = new Stack<float>();
        public static void BeginLabelWidth(float labelWidth)
        {
            labelWidthStack.Push(EditorGUIUtility.labelWidth);
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public static void EndLableWidth()
        {
            if (labelWidthStack.Count > 0)
                EditorGUIUtility.labelWidth = labelWidthStack.Pop();
        }

        private static Stack<Color> guiColorStack = new Stack<Color>();
        public static void BeginGUIColor(Color color)
        {
            guiColorStack.Push(GUI.color);
            GUI.color = color;
        }
        public static void EndGUIColor()
        {
            if (guiColorStack.Count > 0)
                GUI.color = guiColorStack.Pop();
        }

        private static Stack<Color> guiBgColorStack = new Stack<Color>();
        public static void BeginGUIBackgroundColor(Color color)
        {
            guiBgColorStack.Push(GUI.backgroundColor);
            GUI.backgroundColor= color;
        }
        public static void EndGUIBackgroundColor()
        {
            if (guiBgColorStack.Count > 0)
                GUI.backgroundColor = guiBgColorStack.Pop();
        }

        private static Stack<Color> guiContentColorStack = new Stack<Color>();
        public static void BeginGUIContentColor(Color color)
        {
            guiContentColorStack.Push(GUI.contentColor);
            GUI.contentColor = color;
        }
        public static void EndGUIContentColor()
        {
            if (guiContentColorStack.Count > 0)
                GUI.contentColor = guiContentColorStack.Pop();
        }

        public static void BeginIndent()
        {
            EditorGUI.indentLevel++;
        }

        public static void EndIndent()
        {
            EditorGUI.indentLevel--;
        }

        public static string DrawAssetFolderSelection(Rect rect, string label, string assetFolder, bool isReadonly = true)
        {
            string folder = assetFolder;

            EditorGUI.BeginDisabledGroup(isReadonly);
            {
                folder = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width - 40, rect.height), label, assetFolder);
            }
            EditorGUI.EndDisabledGroup();

            if (GUI.Button(new Rect(rect.x+rect.width - 40, rect.y, 20, rect.height), new GUIContent(EditorGUIUtil.FolderIcon)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("folder", folder, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    folder = PathUtil.GetAssetPath(folderPath);
                }
            }
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, rect.height), "\u2716"))
            {
                folder = "";
            }
            return folder;
        }
    }

    
}
