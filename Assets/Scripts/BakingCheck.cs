using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BakingCheck : MonoBehaviour
{
#if UNITY_EDITOR
    //public GameObject notStatic;
    public GameObject[] GIRoots;
    public GameObject[] notGI;
    // Start is called before the first frame update
    public void Enforce()
    {
        for (int i = 0; i < GIRoots.Length; i++)
            MakeStatic(GIRoots[i], true);
        for (int i = 0; i < notGI.Length; i++)
        {
            MeshRenderer r = notGI[i].GetComponent<MeshRenderer>();
            if (r != null) GameObjectUtility.SetStaticEditorFlags(notGI[i], 0);
        }
    }
    public void Revert()
    {
        for (int i = 0; i < GIRoots.Length; i++)
            MakeStatic(GIRoots[i],false);
        for (int i = 0; i < notGI.Length; i++)
        {
            MeshRenderer r = notGI[i].GetComponent<MeshRenderer>();
            if (r != null) GameObjectUtility.SetStaticEditorFlags(notGI[i], 0);
        }
    }
    void MakeStatic(GameObject g, bool state)
    {
        GameObjectUtility.SetStaticEditorFlags(g,state ? StaticEditorFlags.ContributeGI:0);
        for(int i = 0; i < g.transform.childCount; i++)
            MakeStatic(g.transform.GetChild(i).gameObject,state);
    }
#endif
}
