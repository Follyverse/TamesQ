using UnityEngine;
using UnityEditor;
using System;
using System.ComponentModel;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Markers
{

    public class PublishProject : MonoBehaviour
    {
        public string title = "";
        public string author = "";
        public string email = "";
        public string serverIP = "";
        public string serverPort = "";
        public string password = "";
        public string id;
        [SerializeField]
        [TextAreaAttribute(5, 10)]
        private string description = "";
        private bool publishType = false;
        public bool PublishType { get { return publishType; } set { publishType = value; } }

        private void Start()
        {
         }
#if UNITY_EDITOR

        public void NewID()
        {
            DateTime now = DateTime.Now;
            id = now.ToString("yyyy.MM.dd.HH.mm.ss.") + now.Millisecond;
            Debug.Log(id);
        }

        public void Save()
        {

            //     string path = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");          
            BuildPlayer();


        }
        public void CreateDescription(string path)
        {
            string assetTypeHDRP = "HDRenderPipelineAsset";
            bool hdrp = false;
            try
            {
                hdrp = GraphicsSettings.renderPipelineAsset.GetType().Name.Contains(assetTypeHDRP);
            }
            catch { hdrp = false; }
            Debug.Log("HDRP:" + hdrp);
            List<string> lines = new List<string>();
            lines.Add(CoreTame.TamesVersion + "");
            lines.Add(hdrp ? "Unity_HDRP" : "Unity_URP");
            lines.Add(title);
            lines.Add(author);
            lines.Add(id);
            //  Debug.Log("ToID:");
            lines.Add(serverIP);
            lines.Add(serverPort);
            lines.Add(description);
            File.WriteAllLines(path + "description.ini", lines.ToArray());
        }
        public static void BuildPlayer()
        {
            string path = EditorUtility.OpenFolderPanel("Select folder", "Assets", "");

            if (!string.IsNullOrEmpty(path))
            {
                if (path.EndsWith("\\") || path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
                //            else path += "/";

#if UNITY_STANDALONE_WIN               
                BuildWindows(path);
#endif
            }
        }
        static void BuildWindows(string path)
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            GameObject[] gos = scene.GetRootGameObjects();
            PublishProject p = null;
            foreach (GameObject go in gos)
                if ((p = go.GetComponent<PublishProject>()) != null)
                    break;
            if (p != null)
            {
                string pid = p.id;
                pid.Replace('.', '_');
                string email = p.email;
                p.email = "";
                string pass = p.password;
                p.password = "";
                try
                {
                    BuildReport br = BuildPipeline.BuildPlayer(new string[] { "Assets/Scenes/" + scene.name + ".unity" }, path + "/tempproj.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
                    if (br.summary.result == BuildResult.Succeeded)
                    {
                        Debug.Log("publish success");
                        File.Move(path + "/tempproj.exe", path + "/" + pid + ".exe");
                        Directory.Move(path + "/tempproj_Data", path + "/" + pid + "_Data");
                        WriteDescription(path + "/" + pid + "_Data", p);

                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                p.password = pass;
                p.email = email;
            }
        }
        public static void WriteDescription(string path, PublishProject pp)
        {
            string assetTypeHDRP = "HDRenderPipelineAsset";
            bool hdrp = false;
            try
            {
                hdrp = GraphicsSettings.renderPipelineAsset.GetType().Name.Contains(assetTypeHDRP);
            }
            catch { hdrp = false; }
            Debug.Log("HDRP:" + hdrp);
            List<string> lines = new List<string>();
            lines.Add(hdrp ? "Unity_HDRP" : "Unity_URP");
            lines.Add(pp.title);
            lines.Add(pp.author);
            lines.Add(pp.id);
            Debug.Log("ToID:");
            lines.Add(pp.serverIP);
            lines.Add(pp.serverPort);
            lines.Add(pp.description);
            File.WriteAllLines(path + "/" + "description.ini", lines.ToArray());
        }
      
#endif
    }

#if UNITY_EDITOR
    public class ReadOnlyAttribute : PropertyAttribute { }
    /// <summary>
    /// This class contain custom drawer for ReadOnly attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Unity method for drawing GUI in Editor
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Saving previous GUI enabled value
            var previousGUIState = GUI.enabled;
            // Disabling edit for property
            GUI.enabled = false;
            // Drawing Property
            EditorGUI.PropertyField(position, property, label);
            // Setting old GUI enabled value
            GUI.enabled = previousGUIState;
        }
    }
#endif
}
