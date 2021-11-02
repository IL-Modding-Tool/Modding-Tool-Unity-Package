using System;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using MyBox;
using Object = UnityEngine.Object;
#pragma warning disable 618
using System.IO;
using System.Linq;
using ModPackerModule.Utility;
using UnityEditor;
using UnityEngine;
#pragma warning restore 618

#if UNITY_EDITOR
using FuzzySharp.SimilarityRatio.Scorer;
using hooh_ModdingTool.asm_Packer.Editor;
using UnityEngine.SceneManagement;
using Style = Common.HoohWindowStyles;

public partial class HoohTools
{
    private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private int selectedPresetIndex = 0;

    // Ratio'd
    public static readonly IRatioScorer PartialRatio = ScorerCache.Get<PartialRatioScorer>();

    private void DrawUnityUtility(SerializedObject serializedObject)
    {
        foldoutMacros = EditorGUILayout.Foldout(foldoutMacros, "Quick Unity Macros", true, Style.Foldout);
        if (!foldoutMacros) return;

        // Starts a horizontal group
        using (new GUILayout.HorizontalScope("box"))
        {
            using (new GUILayout.VerticalScope())
            {
                Gap = EditorGUILayout.IntField("Showcase Gap: ", Gap);
                Cols = EditorGUILayout.IntField("Showcase Columns: ", Cols);
            }

            if (GUILayout.Button("Showcase Mode", Style.Button))
                if (CheckGoodSelection())
                    _guiEventAction = ShowcaseMode;
        }

        GUILayout.Space(5);

        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.Label("Easy Macros", Style.Header);
            if (GUILayout.Button("Wrap Object with new GameObject and Scale", Style.Button))
                if (CheckGoodSelection())
                    _guiEventAction = WrapObjectScale;
            if (GUILayout.Button("Wrap Object with new GameObject", Style.Button))
                if (CheckGoodSelection())
                    _guiEventAction = WrapObject;
            if (GUILayout.Button("Create Prefab from Selected Objects", Style.Button))
                if (CheckGoodSelection())
                    _guiEventAction = CreatePrefab;
        }


        GUILayout.Space(5);
        using (new GUILayout.VerticalScope("box"))
        {
            LightScaleSize = EditorGUILayout.FloatField("Light Scale Size: ", LightScaleSize);

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Scale Lights and Probes", Style.Button))
                    if (CheckGoodSelection())
                        _guiEventAction = ScaleLightsAndProbes;
                if (GUILayout.Button("Reset Lightmap Scale", Style.Button))
                    _guiEventAction = () =>
                    {
                        var meshes = Resources.FindObjectsOfTypeAll<MeshRenderer>();
                        foreach (var mesh in meshes)
                        {
                            var so = new SerializedObject(mesh);
                            so.FindProperty("m_ScaleInLightmap").floatValue = 1;
                            so.ApplyModifiedProperties();
                        }
                    };
            }
        }

        GUILayout.Space(5);
        using (new GUILayout.HorizontalScope("box"))
        {
            targetObject =
                (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
            if (GUILayout.Button("Move Selected to Clutter", Style.Button))
                _guiEventAction = () =>
                {
                    if (!CheckGoodSelection()) return;
                    var clutter = targetObject != null
                        ? targetObject
                        : GameObject.Find("Clutters") ?? new GameObject("Clutters");

                    foreach (var select in Selection.objects)
                    {
                        var currentObject = (GameObject)select;
                        currentObject.transform.parent = clutter.transform;
                    }
                };
        }

        GUILayout.Space(5);

        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.Label("Multi-Rename", Style.Header);
            PrepostString = EditorGUILayout.TextField("Pre/Postfix Text", PrepostString);

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add to Front", Style.Button) && CheckGoodSelection())
                    _guiEventAction = () => { UnityMacros.AddPrefixOnName(PrepostString); };
                if (GUILayout.Button("Add to End", Style.Button) && CheckGoodSelection())
                    _guiEventAction = () => { UnityMacros.AddPostfixOnName(PrepostString); };
                if (GUILayout.Button("Remove", Style.Button) && CheckGoodSelection())
                    _guiEventAction = () => { UnityMacros.ReplaceTextOfName(PrepostString, ""); };
                if (GUILayout.Button("SetTo", Style.Button) && CheckGoodSelection())
                    _guiEventAction = () => { UnityMacros.SetName(PrepostString); };
                if (GUILayout.Button("Sequence", Style.Button) && CheckGoodSelection())
                    _guiEventAction = () => { UnityMacros.SetNameSequence(PrepostString); };
            }
        }

        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.Label("Initialize Dynamic Bones", Style.Header);
            using (new GUILayout.HorizontalScope())
                selectedPresetIndex = EditorGUILayout.Popup("Dynamic Bone Preset", selectedPresetIndex,
                    DynamicBones.GetPresetList());

            if (GUILayout.Button("Apply", Style.Button))
                _guiEventAction = () => { DynamicBones.Apply(selectedPresetIndex); };
        }

        using (new GUILayout.VerticalScope("box"))
        {
            if (GUILayout.Button("Remove all lightmap related shtis"))
            {
                var scene = SceneManager.GetActiveScene();
                var renderers = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<Renderer>());
                foreach (var renderer in renderers)
                {
                }
            }
        }

        using (new GUILayout.VerticalScope("box"))
        {
            if (GUILayout.Button("Fix 2019+ Models", Style.Button))
            {
                var scene = SceneManager.GetActiveScene();
                var renderers = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<Renderer>());
                var meshes = Directory.GetFiles(PathUtils.GetProjectPath(), "*.fbx", SearchOption.AllDirectories)
                    .SelectMany(x => AssetDatabase.LoadAllAssetsAtPath(x).OfType<Mesh>())
                    .GroupBy(x => x.name, StringComparer.Ordinal)
                    .ToDictionary(x => x.Key, x => x.ToArray());


                foreach (var r in renderers)
                {
                    switch (r)
                    {
                        case MeshRenderer mr:
                        {
                            var mf = r.gameObject.GetComponent<MeshFilter>();
                            if (mf.sharedMesh == null && meshes.TryGetValue(r.name, out var m) && m.Length > 0)
                                mf.sharedMesh = m.FirstOrDefault();
                        
                            if (mf.sharedMesh != null) continue;
                            // Search one more time with deeper fuzzy search. 
                            // This is unacceptable.
                            var winner = FuzzySharp.Process.ExtractOne(mr.name, meshes.Keys, null, PartialRatio);
                            if (winner != null && meshes.TryGetValue(winner.Value, out var fm) && fm.Length > 0)
                                mf.sharedMesh = fm.FirstOrDefault();
                            break;
                        }
                        case SkinnedMeshRenderer smr:
                        {
                            if (smr.sharedMesh == null && meshes.TryGetValue(r.name, out var m) && m.Length > 0)
                                smr.sharedMesh = m.FirstOrDefault();
                            break;
                        }
                    }
                }
            }
        }

        GUILayout.Space(5);
        using (new GUILayout.VerticalScope("box"))
        {
            if (GUILayout.Button("Auto-Generate Materials", Style.Button))
                _guiEventAction = () =>
                {
                    var targetPath = PathUtils.GetProjectPath();
                    var basePath = Path.Combine(Directory.GetCurrentDirectory(), targetPath);
                    var filesWithRegex = PathUtils.GetFilesWithRegex(basePath, @"\.(jpg|png|tif|tga)");
                    var normalizedFiles = filesWithRegex.ToDictionary(Path.GetFileNameWithoutExtension, x => x);
                    foreach (var file in filesWithRegex.Where(x => Path.GetFileName(x).StartsWith("ALB_")))
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        var matPath = Path.Combine(basePath, $"{name}.mat");
                        if (File.Exists(matPath)) File.Delete(matPath);
                        var material = new Material(Shader.Find("Standard")) { color = Color.white };
                        var diffuseTexture =
                            AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(targetPath, Path.GetFileName(file)));
                        material.SetTexture(MainTex, diffuseTexture);
                        if (normalizedFiles.TryGetValue(name.Replace("ALB_", "NRM_"), out var normalPath))
                        {
                            var normalMapTexture =
                                AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(targetPath,
                                    Path.GetFileName(normalPath)));
                            material.SetTexture(BumpMap, normalMapTexture);
                        }

                        AssetDatabase.CreateAsset(material, Path.Combine(targetPath, $"{name}.mat").ToUnixPath());
                    }
                };
        }
    }

    private static void ShowcaseMode()
    {
        for (var i = 0; i < Selection.objects.Length; i++)
        {
            var currentObject = (GameObject)Selection.objects[i];
            var curRow = (int)Mathf.Floor(i / (float)Cols);
            var curCol = i % Cols;

            if (currentObject != null)
                currentObject.transform.position = new Vector3(
                    curCol * Gap,
                    0,
                    curRow * Gap
                );
        }
    }

    // Pretty useful to make stuffs X time larger.
    [MenuItem("hooh Tools/Wrap Object with Gameobject", false)]
    public static void WrapObject()
    {
        foreach (var currentObject in Selection.objects.Cast<GameObject>())
        {
            var parent = currentObject.transform.parent;
            if (currentObject == null) continue;

            var wrapObject = new GameObject(currentObject.name);
            wrapObject.transform.SetParent(parent, false);
            currentObject.transform.SetParent(wrapObject.transform);
        }
    }

    [MenuItem("hooh Tools/Wrap Object with Gameobject and Scale", false)]
    public static void WrapObjectScale()
    {
        foreach (var currentObject in Selection.objects.Cast<GameObject>())
        {
            var parent = currentObject.transform.parent;
            if (currentObject == null) continue;

            currentObject.transform.localScale = new Vector3(9, 9, 9);
            var wrapObject = new GameObject(currentObject.name);
            wrapObject.transform.parent = parent;
            currentObject.transform.SetParent(wrapObject.transform);
            wrapObject.transform.position = Vector3.zero;
        }
    }

    // Mass Register Prefabs. I'm sick and tired of dragging here and there for 10 minutes.
    [MenuItem("hooh Tools/Create Prefab")]
    public static void CreatePrefab()
    {
        var objectArray = Selection.gameObjects;

        foreach (var gameObject in objectArray)
        {
            var localPath = Path.Combine(PathUtils.GetProjectPath(), gameObject.name + ".prefab").Replace("\\", "/");
            Debug.Log(localPath);
            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            {
                CreateNew(gameObject, localPath);
            }
            else
            {
                Debug.Log(gameObject.name + " is not a prefab, will convert");
                CreateNew(gameObject, localPath);
            }
        }
    }

    [MenuItem("hooh Tools/Create Prefab", true)]
    private static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null;
    }

    private static void CreateNew(GameObject obj, string localPath)
    {
        //Create a new prefab at the path given
        Object prefab = PrefabUtility.SaveAsPrefabAsset(obj, localPath);

        // Fuck you unity you removed this without replacement API?
        // https://forum.unity.com/threads/how-to-generate-prefabs-w-scripts-w-the-new-system.613747/#post-4113010:w

#pragma warning disable 618
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
#pragma warning restore 618
    }
}
#endif