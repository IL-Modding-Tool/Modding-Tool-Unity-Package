using System;
using System.Collections.Generic;
using Common;
using UnityEditor;
using UnityEngine;
using Style = Common.HoohWindowStyles;

namespace hooh_ModdingTool.asm_Packer.Editor
{
    public static class WindowUtility
    {
        public struct DropDownItem
        {
            public string Name;
            public bool On;
            public GenericMenu.MenuFunction2 Callback;
            public object Parameter;
        }
        public static void Button(string text, Action callback)
        {
            if (!GUILayout.Button(text, Style.Button)) return;
            callback();
        }

        public static void Foldout(ref bool foldVariableReference, string text, Action callback)
        {
            foldVariableReference = EditorGUILayout.Foldout(foldVariableReference, text, true, Style.Foldout);
            if (!foldVariableReference) return;
            callback();
        }

        public static void VerticalLayout(Action callback, bool isBox = true)
        {
            GUILayout.BeginVertical();
            callback();
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }

        public static void HorizontalLayout(Action callback, bool isBox = true)
        {
            GUILayout.BeginHorizontal();
            callback();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        public static void Dropdown(string title, IEnumerable<DropDownItem> items)
        {
            if (!EditorGUILayout.DropdownButton(new GUIContent(title), FocusType.Passive, EditorStyles.toolbarDropDown)) return;
            var menu = new GenericMenu();
            foreach (var item in items)
            {
                menu.AddItem(new GUIContent(item.Name), item.On, item.Callback, item.Parameter);
            }
            menu.ShowAsContext();
        }
    }
}