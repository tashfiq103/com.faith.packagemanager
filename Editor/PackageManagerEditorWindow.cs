﻿namespace com.faith.package_manager {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class PackageManagerEditorWindow : EditorWindow {

        #region CustomVariables

        public struct PackageInfo {
            public string packageLink;
            public string packageVersion;
        }

        #endregion

        #region Private Variables

        private static PackageManagerEditorWindow m_PackageManagerEditorWindow;

        private static GitRepoInfo m_GitRepositoryInfo;

        private Texture2D m_IconForTickMark;
        private Texture2D m_BackgroundTextureOfPackageList;
        private Texture2D m_BackgroundTextureOfPackageDescription;
        private Texture2D m_BackgroundTextureOfHeader;
        private Texture2D m_BackgroundTextureOfFooter;

        private Vector2 m_ScrollPositionAtPackageList;
        private Vector2 m_ScrollPositionAtPackageDescription;

        private string m_NameOfManifestDirectory = "Packages";
        private string m_PackageName;
        private string m_RepositoryLink;

        private int m_SelectedPackageIndex = 0;

        #endregion

        #region Public Callback :   Static

        [MenuItem ("FAITH/PackageManager")]
        public static void ShowWindow () {

            string[] m_GUIDOfGitRepositoryInfo = AssetDatabase.FindAssets ("GitRepositoryInfo");

            if (m_GUIDOfGitRepositoryInfo.Length > 0) {

                string t_AssetPath = AssetDatabase.GUIDToAssetPath (m_GUIDOfGitRepositoryInfo[0]);
                m_GitRepositoryInfo = (GitRepoInfo) AssetDatabase.LoadAssetAtPath (t_AssetPath, typeof (GitRepoInfo));
            }

            m_PackageManagerEditorWindow = (PackageManagerEditorWindow) GetWindow<PackageManagerEditorWindow> ("Package Manager", typeof (PackageManagerEditorWindow));

            m_PackageManagerEditorWindow.minSize = new Vector2 (360f, 240f);
            m_PackageManagerEditorWindow.Show ();
        }

        public static void HideWindow () {

        }

        #endregion

        #region EditorWindow

        private void OnEnable () {

            Initialization ();
        }

        private void OnGUI () {

            DrawHeader ();
            DrawPackageList ();
            DrawPackageDescription ();
            DrawFooter ();

            // m_PackageName    = EditorGUILayout.TextField("PackageName",m_PackageName);
            // m_RepositoryLink = EditorGUILayout.TextField("RepositoryLink", m_RepositoryLink);
            // if(GUILayout.Button("Show Directory")){

            //     AddNewRepositoryToManifestJSON(m_PackageName,m_RepositoryLink);
            // }
        }

        #endregion

        #region Custom GUI

        private void Initialization () {

            m_SelectedPackageIndex = 0;
            
            string[] m_GUIDOfTickMarkIcon = AssetDatabase.FindAssets ("Icon_TickMark",new string[]{"Assets/com.faith.package_manager"});

            if (m_GUIDOfTickMarkIcon.Length > 0) {

                string t_AssetPath = AssetDatabase.GUIDToAssetPath (m_GUIDOfTickMarkIcon[0]);
                m_IconForTickMark = (Texture2D) AssetDatabase.LoadAssetAtPath (t_AssetPath, typeof (Texture2D));
            }

            m_BackgroundTextureOfHeader = new Texture2D (1, 1);
            m_BackgroundTextureOfHeader.SetPixel (
                0,
                0,
                new Color (
                    0.8352942f,
                    0.8352942f,
                    0.8352942f,
                    1f)
            );
            m_BackgroundTextureOfHeader.Apply ();

            m_BackgroundTextureOfPackageList = new Texture2D (1, 1);
            m_BackgroundTextureOfPackageList.SetPixel (
                0,
                0,
                new Color (
                    13f / 255f,
                    32f / 255f,
                    44f / 255f,
                    0.05f)
            );
            m_BackgroundTextureOfPackageList.Apply ();

            m_BackgroundTextureOfPackageDescription = new Texture2D (1, 1);
            m_BackgroundTextureOfPackageDescription.SetPixel (
                0,
                0,
                new Color (
                    13f / 255f,
                    32f / 255f,
                    44f / 255f,
                    0.05f)
            );
            m_BackgroundTextureOfPackageDescription.Apply ();

            m_BackgroundTextureOfFooter = new Texture2D (1, 1);
            m_BackgroundTextureOfFooter.SetPixel (
                0,
                0,
                new Color (
                    0.8352942f,
                    0.8352942f,
                    0.8352942f,
                    1f)
            );
            m_BackgroundTextureOfFooter.Apply ();

        }

        private void DrawHeader () {
            Vector2 t_PanelOriginOfHeaderPanel = new Vector2 (0, 1);
            Rect t_RectTransformOfHeaderPanel = new Rect (
                t_PanelOriginOfHeaderPanel.x,
                t_PanelOriginOfHeaderPanel.y,
                Screen.width,
                18f
            );
            GUI.DrawTexture (t_RectTransformOfHeaderPanel, m_BackgroundTextureOfHeader);
        }

        private void DrawPackageList () {
            Vector2 t_PanelOriginOfPackageListPanel = new Vector2 (0, 20);
            Rect t_RectTransformOfPackageListPanel = new Rect (
                t_PanelOriginOfPackageListPanel.x,
                t_PanelOriginOfPackageListPanel.y,
                180,
                Screen.height - (t_PanelOriginOfPackageListPanel.y * 3.5f)
            );

            GUI.DrawTexture (t_RectTransformOfPackageListPanel, m_BackgroundTextureOfPackageList);

            GUILayout.BeginArea (t_RectTransformOfPackageListPanel);
            m_ScrollPositionAtPackageList = GUILayout.BeginScrollView (
                m_ScrollPositionAtPackageList,
                false,
                false
            ); {

                int t_NumberOfGitRepositoryInfo = m_GitRepositoryInfo.gitInfos.Count;

                for (int i = 0; i < t_NumberOfGitRepositoryInfo; i++) {

                    GUILayout.BeginHorizontal (EditorStyles.helpBox); {

                        if (GUILayout.Button (m_GitRepositoryInfo.gitInfos[i].displayName, GUILayout.Height (20))) {

                            m_SelectedPackageIndex = i;
                        }

                        GUILayout.Label (m_IconForTickMark, GUILayout.Width (20), GUILayout.Height (20));
                    }
                    GUILayout.EndHorizontal ();

                }
            }
            EditorGUILayout.EndScrollView ();
            GUILayout.EndArea ();
        }

        private void DrawPackageDescription () {

            Vector2 t_PanelOriginOfPackageDescriptionPanel = new Vector2 (185, 20);
            GUI.DrawTexture (new Rect (
                    t_PanelOriginOfPackageDescriptionPanel.x,
                    t_PanelOriginOfPackageDescriptionPanel.y,
                    Screen.width - 185,
                    Screen.height - (t_PanelOriginOfPackageDescriptionPanel.y * 3.5f)
                ),
                m_BackgroundTextureOfPackageDescription);

            Rect t_RectTransformOfPackageListPanel = new Rect (
                t_PanelOriginOfPackageDescriptionPanel.x,
                t_PanelOriginOfPackageDescriptionPanel.y,
                360f,
                Screen.height - (t_PanelOriginOfPackageDescriptionPanel.y * 3.5f)
            );

            GUILayout.BeginArea (t_RectTransformOfPackageListPanel); {
                m_ScrollPositionAtPackageDescription = GUILayout.BeginScrollView (m_ScrollPositionAtPackageDescription); {
                    EditorGUILayout.BeginVertical (); {
                        EditorGUILayout.LabelField (
                            m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].displayName + " : v" + m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].version,
                            EditorStyles.boldLabel);
                        EditorGUILayout.LabelField (
                            "Unity" + m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unity,
                            EditorStyles.helpBox,
                            GUILayout.Width (70));

                        EditorGUILayout.Space ();
                        EditorGUILayout.LabelField (
                            "Description",
                            EditorStyles.boldLabel);
                        EditorGUILayout.LabelField (
                            m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].description);

                        int t_NumberOfUnityDependencies = m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unityDependencies.Count;
                        if (t_NumberOfUnityDependencies > 0) {

                            EditorGUILayout.Space ();
                            EditorGUILayout.LabelField (
                                "Unity Dependencies",
                                EditorStyles.boldLabel);

                            EditorGUI.indentLevel += 1;
                            for (int i = 0; i < t_NumberOfUnityDependencies; i++) {
                                EditorGUILayout.LabelField (
                                    "(" + (i + 1) + ") " +
                                    m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unityDependencies[i].name +
                                    " v" +
                                    m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].unityDependencies[i].version);
                            }
                            EditorGUI.indentLevel -= 1;
                        }

                        int t_NumberOfInternalDependencies = m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].internalDependencies.Count;
                        if (t_NumberOfInternalDependencies > 0) {

                            EditorGUILayout.Space ();
                            EditorGUILayout.LabelField (
                                "Internal Dependencies",
                                EditorStyles.boldLabel);

                            EditorGUI.indentLevel += 1;
                            for (int i = 0; i < t_NumberOfInternalDependencies; i++) {
                                EditorGUILayout.LabelField (
                                    "(" + (i + 1) + ") " +
                                    m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].internalDependencies[i].name +
                                    " v" +
                                    m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].internalDependencies[i].version);
                            }
                            EditorGUI.indentLevel -= 1;
                        }

                        int t_NumberOfExternalDependencies = m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].externalDependencies.Count;
                        if (t_NumberOfExternalDependencies > 0) {

                            EditorGUILayout.Space ();
                            EditorGUILayout.LabelField (
                                "External Dependencies",
                                EditorStyles.boldLabel);

                            EditorGUI.indentLevel += 1;
                            for (int i = 0; i < t_NumberOfExternalDependencies; i++) {
                                EditorGUILayout.LabelField (
                                    "(" + (i + 1) + ") " +
                                    m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].externalDependencies[i].name +
                                    " v" +
                                    m_GitRepositoryInfo.gitInfos[m_SelectedPackageIndex].externalDependencies[i].version);
                            }
                            EditorGUI.indentLevel -= 1;
                        }
                    }
                    EditorGUILayout.EndVertical ();
                }
                GUILayout.EndScrollView ();
            }
            GUILayout.EndArea ();
        }
        private void DrawFooter () {

            Vector2 t_PanelOriginOfFooterPanel = new Vector2 (0, Screen.height - 45f);
            Rect t_RectTransformOfFooterPanel = new Rect (
                t_PanelOriginOfFooterPanel.x,
                t_PanelOriginOfFooterPanel.y,
                Screen.width,
                25
            );
            GUI.DrawTexture (t_RectTransformOfFooterPanel, m_BackgroundTextureOfFooter);
        }

        #endregion

        #region Configuretion   :   Reading/Writing manifest.json

        private void AddNewRepositoryToManifestJSON (string t_PackageName, string t_RepositoryLink) {

            string t_StreamingAssetPath = Application.streamingAssetsPath;
            string[] t_Split = t_StreamingAssetPath.Split ('/');
            string t_ManifestPath = "";

            int t_NumberOfSplit = t_Split.Length - 2;
            for (int i = 0; i < t_NumberOfSplit; i++) {

                t_ManifestPath += t_Split[i];
                t_ManifestPath += "/";
            }
            t_ManifestPath += m_NameOfManifestDirectory;
            t_ManifestPath += "/";
            t_ManifestPath += "manifest.json"; //"manifest.json"; 

            string t_Result = System.IO.File.ReadAllText (t_ManifestPath);

            //Extracting    :   Package
            string[] t_SplitByComa = t_Result.Split (',');
            t_NumberOfSplit = t_SplitByComa.Length;
            List<PackageInfo> t_CurrentPackageInfo = new List<PackageInfo> ();

            for (int i = 0; i < t_NumberOfSplit; i++) {

                string t_ConcatinatedString = "";
                List<char> t_Converted = t_SplitByComa[i].ToList ();
                int t_NumberOfCharacter = t_Converted.Count;
                for (int j = 0; j < t_NumberOfCharacter;) {

                    if (t_Converted[j] == ' ' ||
                        t_Converted[j] == '{' ||
                        t_Converted[j] == '}' ||
                        t_Converted[j] == '\t' ||
                        t_Converted[j] == '\n') {

                        t_Converted.RemoveAt (j);
                        t_Converted.TrimExcess ();

                        t_NumberOfCharacter--;
                    } else {

                        t_ConcatinatedString += t_Converted[j];
                        j++;
                    }
                }

                string[] t_SplitByColon = t_ConcatinatedString.Split (':');
                if (i == 0) {
                    t_CurrentPackageInfo.Add (new PackageInfo () {
                        packageLink = t_SplitByColon[1],
                            packageVersion = t_SplitByColon[2]
                    });
                } else {
                    t_CurrentPackageInfo.Add (new PackageInfo () {
                        packageLink = t_SplitByColon[0],
                            packageVersion = t_SplitByColon[1]
                    });
                }

                Debug.Log (t_CurrentPackageInfo[t_CurrentPackageInfo.Count - 1].packageLink + " : " + t_CurrentPackageInfo[t_CurrentPackageInfo.Count - 1].packageVersion);
            }

            //WritingPackage
            using (StreamWriter streamWrite = new StreamWriter (t_ManifestPath)) {

                bool t_IsRepositoryAlreadyAdded = false;

                streamWrite.WriteLine ("{");
                streamWrite.WriteLine ("\t\"dependencies\":{");

                int t_NumberOfPackage = t_CurrentPackageInfo.Count;
                for (int i = 0; i < t_NumberOfPackage; i++) {

                    if (t_CurrentPackageInfo[i].packageLink == t_PackageName)
                        t_IsRepositoryAlreadyAdded = true;

                    streamWrite.WriteLine (
                        "\t\t" +
                        t_CurrentPackageInfo[i].packageLink +
                        " : " +
                        t_CurrentPackageInfo[i].packageVersion +
                        ((i == (t_NumberOfPackage - 1)) ? (t_IsRepositoryAlreadyAdded ? "" : ",") : ","));
                }

                if (!t_IsRepositoryAlreadyAdded) {

                    streamWrite.WriteLine (
                        "\t\t" +
                        "\"" +
                        t_PackageName +
                        "\"" +
                        " : " +
                        "\"git+" +
                        t_RepositoryLink +
                        "\"");
                }

                streamWrite.WriteLine ("\t}");
                streamWrite.WriteLine ("}");
            }
            AssetDatabase.Refresh ();
        }

        #endregion

        #region Editor Module

        private void DrawHorizontalLine () {
            EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider);
        }

        private void DrawHorizontalLineOnGUI (Rect rect) {
            EditorGUI.LabelField (rect, "", GUI.skin.horizontalSlider);
        }

        #endregion
    }
}