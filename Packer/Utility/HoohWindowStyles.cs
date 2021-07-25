#if UNITY_EDITOR
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public static class HoohWindowStyles
    {
        public static GUIStyle Button;
        public static GUIStyle ButtonDark;
        public static GUIStyle ButtonGood;
        public static GUIStyle ButtonWarning;
        public static GUIStyle Foldout;
        public static GUIStyle Header;
        public static GUIStyle Medium;
        public static GUIStyle Title;
        public static GUIStyle SuperBold;
        public static GUIStyle ButtonTab;

        public static Color red => new Color(0.91f, 0.3f, 0.24f);
        public static Color green => new Color(0.18f, 0.8f, 0.44f);

        static HoohWindowStyles()
        {
            Medium = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11
            };
            Header = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
            SuperBold = new GUIStyle(GUI.skin.label)
                {fontSize = 20, fontStyle = FontStyle.Bold, margin = new RectOffset(4, 4, 0, 4)};
            Title = new GUIStyle(GUI.skin.label) {fontSize = 15, margin = new RectOffset(10, 10, 0, 10)};
            Medium = new GUIStyle(GUI.skin.label) {fontSize = 14, margin = new RectOffset(4, 4, 0, 4)};
            Foldout = new GUIStyle(EditorStyles.foldout) {fontSize = 15, margin = new RectOffset(10, 10, 0, 10)};
            Button = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontSize = 11, margin = new RectOffset(2, 2, 2, 2), padding = new RectOffset(5, 5, 5, 5),
                fixedHeight = 23
            };
            ButtonDark = new GUIStyle(Button)
            {
                normal = {textColor = Color.white}, active = {textColor = Color.white},
                hover = {textColor = Color.white}
            };
            ButtonWarning = new GUIStyle("button")
            {
            }; 
            ButtonGood = new GUIStyle(Button);

            ButtonTab = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 32,
                margin = new RectOffset(0, 0, 0, 32)
            };
        }
    }
}
#endif