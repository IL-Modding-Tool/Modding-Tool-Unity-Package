using System;
using System.Linq;
using Common;
using ModPackerModule.Structure.SideloaderMod;
using MyBox;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

namespace ModPackerModule.Utility.Inspector
{
    [CustomEditor(typeof(SideloaderMod))]
    public class SideloaderXMLDrawer : Editor
    {
        public Vector2 scrollPosition;

        public override void OnInspectorGUI()
        {
            GUI.enabled = true;
            // DrawDefaultInspector();
            if (serializedObject.targetObjects.Length > 1)
            {
                EditorGUILayout.HelpBox("You cannot see more than 2 mod infos at once.", MessageType.Error);
            }
            else
            {
                var mainProperty = serializedObject.FindProperty("MainData");
                GUILayout.BeginVertical("box");
                var guid = mainProperty.FindPropertyRelative("guid");
                var name = mainProperty.FindPropertyRelative("name");
                var version = mainProperty.FindPropertyRelative("version");
                var author = mainProperty.FindPropertyRelative("author");
                var description = mainProperty.FindPropertyRelative("description");
                var lastTime = serializedObject.FindProperty("lastBuildTime");
                var lastVersion = serializedObject.FindProperty("lastBuildVersion");

                var o = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60;
                if (!lastTime.stringValue.IsNullOrEmpty() && !lastVersion.stringValue.IsNullOrEmpty())
                {
                    EditorGUILayout.HelpBox(
                        $"You built the mod in {lastTime.stringValue}.\nThe last build was version {lastVersion.stringValue}.", 
                        MessageType.Info);
                }

                GUILayout.Label(name.stringValue, HoohWindowStyles.SuperBold);
                GUILayout.Label(guid.stringValue, HoohWindowStyles.Medium);
                GUILayout.Label(version.stringValue, HoohWindowStyles.Medium);
                GUILayout.Label(author.stringValue, HoohWindowStyles.Medium);
                GUI.enabled = false;
                EditorGUILayout.TextField(description.stringValue, new GUIStyle()
                {
                    fixedHeight = 120
                });
                GUI.enabled = true;
                EditorGUIUtility.labelWidth = o;
                GUILayout.EndVertical();

                GUILayout.Label("Issues", HoohWindowStyles.Title);
                GUILayout.BeginVertical("box");
                var serializedProperty = serializedObject.FindProperty("Issues");
                if (serializedProperty.arraySize <= 0)
                {
                    EditorGUILayout.HelpBox("This Mod XML is valid.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Modding tool found some issues while parsing the mod XML files.",
                        MessageType.Error);
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true);
                    foreach (var i in Enumerable.Range(0, serializedProperty.arraySize))
                    {
                        var prop = serializedProperty.GetArrayElementAtIndex(i);
                        var value = prop.FindPropertyRelative("Message").stringValue;
                        var suggest = prop.FindPropertyRelative("Suggest").stringValue;
                        if (!suggest.IsNullOrEmpty())
                        {
                            GUILayout.Label(value + "\n" + suggest);
                        }
                        else
                        {
                            GUILayout.Label(value);
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }

                GUILayout.EndVertical();
                GUILayout.Label("Serialized Data (Read Only)", HoohWindowStyles.Title);
                GUILayout.BeginVertical("box");
                base.OnInspectorGUI();
                GUILayout.EndVertical();
            }

            GUI.enabled = false;
        }
    }
}