﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using Records;
namespace Markers
{
    public enum NavigationMode { WASD, WSMouse}
    public enum AutoSaveType { OneFile, MultipleFiles, None}
    public class MarkerSettings : MonoBehaviour
    {
        public static MarkerSettings instance;
        public static int Version = 1;
        public static AutoSaveType AutoSaveMode =  AutoSaveType.OneFile;
        public AutoSaveType autoSaveMode = AutoSaveType.OneFile;
        public string password = "";
        public bool replay = false;
         public NavigationMode navMode =NavigationMode.WASD;
        public GameObject torch;
        /// <summary>
        /// the Y offset of the camera, from the walkable surface
        /// </summary>
        public string eyeHeights = "1.6";      
        public string materialEmission { get { return mE; } set { mE = value; } }
        private string mE;

        public string email;
        public string subject;
        public InputSetting sendBy;

        public bool time = true;
        public bool IP = false;

        private void Start()
        {
     instance= this;
        }
        /// <summary>
        /// returns <see cref="customManifests"/>
        /// </summary>
        /// <returns></returns>

        public void Load()
        {
#if UNITY_EDITOR
       
#endif
        }
        public void Save()
        {
#if UNITY_EDITOR
        
#endif
        }
        private static List<Material> mas = new();
        private static List<GameObject> gos = new();
        private static void AllGameObjects()
        {
            gos = new();
            GameObject[] root = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < root.Length; i++)
                gos.AddRange(AddChildren(root[i]));
        }
        private static List<GameObject> AddChildren(GameObject g)
        {
            List<GameObject> r = new() { g };
            int cc = g.transform.childCount;
            for (int i = 0; i < cc; i++)
                r.AddRange(AddChildren(g.transform.GetChild(i).gameObject));
            return r;
        }
        private void AllMaterials()
        {
            mas = new();
            MeshRenderer mr;
            Material[] ms;
            bool f;
            foreach (GameObject tgo in gos)
                if ((mr = tgo.GetComponent<MeshRenderer>()) != null)
                {
                    ms = mr.sharedMaterials;
                    foreach (Material m in ms)
                    {
                        f = false;
                        foreach (Material ma in mas)
                            if (ma == m)
                            { f = true; break; }
                        if (!f)
                            mas.Add(m);
                    }
                }
        }
        private static int HomonymIndex(Transform tc, Transform t)
        {
            int k = 0;
            if (t == null) return 0;
            for (int i = 0; i < t.childCount; i++)
                if (t.GetChild(i).name == tc.name)
                {
                    if (t.GetChild(i) == tc)
                    {
                        return k;
                    }
                    else
                        k++;
                }
            return -1;
        }
        private static Transform HomonymObject(string name, Transform t, int index)
        {
            int k = 0;
            for (int i = 0; i < t.childCount; i++)
                if (t.GetChild(i).name == name)
                {
                    if (index == k)
                        return t.GetChild(i);
                    else
                        k++;
                }
            return null;
        }
        public static string ObjectToLine(GameObject go, GameObject root = null)
        {
            if (go == null)
                return "";
            string r = "";
            Transform t = go.transform.parent;
            Transform tc = go.transform;
            int k = 0, i = 0;
            while (tc != root)
            {
                k = HomonymIndex(tc, t);
                r += tc.name + ":" + k;
                tc = t;
                if (t != null)
                {
                    r += ",";
                    t = t.parent;
                }
                i++;
            }
            return r;
        }

        public static GameObject LineToObject(string s, GameObject root = null)
        {
            string[] ss = s.Split(',');
            string[] nas = new string[ss.Length];
            int[] ins = new int[ss.Length];
            string[] si;
            for (int i = 0; i < ss.Length; i++)
            {
                si = ss[i].Split(':');
                nas[i] = si[0];
                ins[i] = int.Parse(si[1]);
            }
            Transform t, tg;
            for (int i = 0; i < gos.Count; i++)
                if (gos[i].transform.parent == root)
                    if (gos[i].name == nas[0])
                    {
                        t = gos[i].transform;
                        for (int j = 0; j < nas.Length; j++)
                        {
                            tg = t.parent;
                            if (tg == null)
                            {
                                if (j == nas.Length - 1) return gos[i];
                                else break;
                            }
                            else
                                t = HomonymObject(nas[i], tg, ins[i]);
                            if (t == null)
                                break;
                            else
                                t = tg;
                        }
                    }
            return null;
        }

        public static Material FindMaterial(string s)
        {
            string[] ss = s.Split(':');
            int si = int.Parse(ss[1]);
            int k = 0;
            Material last = null;
            for (int i = 0; i < mas.Count; i++)
            {
                if (mas[i].name == ss[0])
                {
                    last = mas[i];
                    if (k == si)
                        return mas[i];
                    k++;
                }
            }
            return last;
        }
        public static string FindMaterial(Material m)
        {
            int k = 0;
            for (int i = 0; i < mas.Count; i++)
                if (mas[i].name == m.name)
                {
                    if (mas[i] == m) return m.name + ":" + k;
                    else k++;
                }
            if (k > 0) return m.name + ":0";
            else return "";
        }
        private string Color(Color c)
        {
            return c.r + "," + c.g + "," + c.b;
        }
        private Color Color(string s)
        {
            string[] ss = s.Split(',');
            return new Color(float.Parse(ss[0]), float.Parse(ss[1]), float.Parse(ss[2]));
        }
        public void FreezeIntensity()
        {
            AllGameObjects();
            AllMaterials();
            string line = "";
            for (int i = 0; i < mas.Count; i++)
                line += FindMaterial(mas[i]) + "/" + Color(mas[i].GetColor("_EmissiveColor")) + "\\";
            materialEmission = line;
            gos = null;
            mas = null;
        }
        public void ResetIntensity()
        {
            AllGameObjects();
            AllMaterials();
            string line = materialEmission;
            string[] mats = line.Split('\\');
            for (int i = 0; i < mats.Length; i++)
                if (mats[i].Length > 0)
                {
                    string[] sep = mats[i].Split('/');
                    if (sep[1] != "-")
                    {
                        Material m = FindMaterial(sep[0]);
                        if (m != null)
                            m.SetColor("_EmissiveColor", Color(sep[1]));
                    }
                }
        }
        public void ExportToCSV()
        {
            bool success = false;
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Select file", "Assets", "");
            if (path != "")
            {
                TameFullRecord.allRecords = new TameFullRecord(null);
                success = TameFullRecord.allRecords.Load(path);
            }

            if (success)
            {
                path = EditorUtility.SaveFolderPanel("Select folde", "Assets", "");
                if (path != "")
                {
                    //                 TameFullRecord.allRecords.ExportToCSV(path, option);
                }
            }
#endif
        }
        }
    }
