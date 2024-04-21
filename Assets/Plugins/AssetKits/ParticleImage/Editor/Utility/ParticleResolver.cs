using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetKits.ParticleImage.Editor
{
    public class ParticleResolver : EditorWindow
    {
        // Constants for old and new files and GUIDs
        private const string OLD_FILE = "947937735";
        private const string NEW_FILE = "11500000";

        private const string OLD_GUID = "1758a5da93e2b4f1fa41fb17e5705ae3";
        private const string NEW_GUID = "a0ce80c06832d4a49a2a30ece9fb2df6";

        // List of assets that need to be replaced
        private readonly List<Asset> _assets = new();

        private bool _isForceText;
        private bool _isSolved;

        private Vector2 _scrollPosition = Vector2.zero;
        private bool _searchPerformed;

        private void OnGUI()
        {
            GUILayout.Box(
                "If you upgraded Particle Image from version 1.0.0 to 1.1.0, you might have encountered an issue with script references for your particles because of source code inclusion. However, don't worry, this tool can easily resolve this problem for you.",
                EditorStyles.helpBox);
            if (_isForceText)
            {
                EditorGUILayout.HelpBox("Please, backup the project before using this tool!", MessageType.Warning);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Find unresolved particles", GUILayout.Height(32))) FindAssetsByGuid(OLD_GUID);
                if (GUILayout.Button(EditorGUIUtility.IconContent("_Help@2x"), GUILayout.Width(32),
                        GUILayout.Height(32))) Application.OpenURL("https://doc.assetkits.io/update");
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Change asset serialization mode to Force Text for use this tool!",
                    MessageType.Error);
                if (GUILayout.Button("Open Settings")) SettingsService.OpenProjectSettings("Project/Editor");
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            for (var i = 0; i < _assets.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(_assets[i].Path,
                    _assets[i].Solved
                        ? EditorGUIUtility.IconContent("Installed").image
                        : EditorGUIUtility.IconContent("Error").image));
                GUILayout.EndHorizontal();
            }

            if (_assets.Count == 0 && _searchPerformed)
                GUILayout.Label(
                    new GUIContent("No unresolved particles were found in the project!",
                        EditorGUIUtility.IconContent("TestPassed").image), EditorStyles.centeredGreyMiniLabel);

            GUILayout.EndScrollView();

            EditorGUI.BeginDisabledGroup(_assets.Count == 0);
            if (GUILayout.Button(_isSolved ? "Restart Unity" : "Fix", GUILayout.Height(42)))
            {
                if (_isSolved)
                    EditorApplication.OpenProject(Application.dataPath);
                else
                    Fix();
            }

            EditorGUI.EndDisabledGroup();
        }

        [MenuItem("Tools/AssetKits/Particle Resolver...")]
        private static void Init()
        {
            var window = GetWindow<ParticleResolver>(false, "Particle Resolver", true);
            window.position = new Rect(100, 100, 500, 400);
            window.Show();

            window._isForceText = EditorSettings.serializationMode == SerializationMode.ForceText;
        }

        private void FindAssetsByGuid(string guid)
        {
            var assetGUIDs = AssetDatabase.FindAssets("t:Object");

            for (var i = 0; i < assetGUIDs.Length; i++)
                try
                {
                    var assetFilePath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);

                    if (Path.GetExtension(assetFilePath).Equals(".unity") ||
                        Path.GetExtension(assetFilePath).Equals(".prefab") ||
                        Path.GetExtension(assetFilePath).Equals(".asset"))
                    {
                        var assetFile = File.ReadAllText(assetFilePath);

                        if (assetFile.Contains(guid)) _assets.Add(new Asset(assetFilePath, "", false));

                        EditorUtility.DisplayProgressBar("Finding particle references in the project", assetFilePath,
                            (float)i / assetGUIDs.Length);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }

            EditorUtility.ClearProgressBar();
            _searchPerformed = true;
        }

        private void Fix()
        {
            for (var i = 0; i < _assets.Count; i++)
                try
                {
                    var assetFile = File.ReadAllText(_assets[i].Path);
                    assetFile = assetFile.Replace(OLD_GUID, NEW_GUID);
                    assetFile = assetFile.Replace(OLD_FILE, NEW_FILE);
                    File.WriteAllText(_assets[i].Path, assetFile);
                    _assets[i].Solved = true;
                }
                catch (Exception e)
                {
                    // ignored
                }

            _isSolved = true;
        }

        private class Asset
        {
            public Asset(string path, string name, bool solved)
            {
                Path = path;
                Name = name;
                Solved = solved;
            }

            public string Path { get; }

            public string Name { get; }

            public bool Solved { get; set; }
        }
    }
}