using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tames;
using Multi;
public class ProjectData : MonoBehaviour
{
    public static ProjectData instance;
    // Start is called before the first frame update
    public string title;
    public string IP;
    public string port;
    public string token;
    public int minutes = 0;

    public bool includeIP = false;
    public StateSender progress;
    public StateSender choice;
    public StateSender surveys;
    public StateSender location;
    public GameObject[] elements;
    public GameObject[] alternatives;
    private List<Vector3> position = new List<Vector3>();
    private List<Quaternion> rotation = new List<Quaternion>();
    private List<float> time = new List<float>();
    private List<TameElement> tames = new List<TameElement>();
    private List<TameAlternative> alters = new List<TameAlternative>();
    private List<InfoUI.QFrame> frames = new List<InfoUI.QFrame>();
    private float lastTime = -1f;
    int count = 0;


    public void RegisterProject(bool state)
    {
        Debug.Log("sent: ?");
        float f = progress.on ? progress.interval : 10000;
        f = choice.on ? Mathf.Min(f, choice.interval) : f;
        f = surveys.on ? Mathf.Min(f, surveys.interval) : f;
        f = location.on ? Mathf.Min(f, location.interval) : f;
        TNet.Message msg = new Multi.TNet.Message() { messageId = state ? TNet.Project : TNet.Unreg, ints = new int[] { minutes, (int)f, includeIP?1:0 }, strings = new string[] { title, token }, };
        TNet.Send(IP, port, msg);
    }
    public void RequestResults()
    {
        TNet.Message msg = new Multi.TNet.Message() { messageId = Multi.TNet.Request, strings = new string[] { token } };
        TNet.Send(IP, port, msg);
    }

    public static string[] headers;
    private void Start()
    {
        instance = this;
        TNet.instance.ip = IP;
        TNet.instance.port = port;
        //   Debug.Log("port at start " + port);
        GetElements();
    }
    float lastRecordCheck = 0;
    private void Update()
    {
        if (!IntroPage.active && IntroPage.instance.opening)
        {
            Debug.Log("check 2");
            if (lastTime < 0)
                lastTime = TameElement.ActiveTime;
            else
            {
                float t = TameElement.ActiveTime;
                if (t - lastRecordCheck > 2)
                {
                    lastRecordCheck = t;
                    CheckElements(t);
                    CheckChoices(t);
                    CheckSurveys(t);
                }
                bool reset = false;
                if (location.CheckTime(lastTime, t))
                {
                    reset = true;
                    TNet.instance.Send(position, rotation, time);
                }
                if (location.on && ((int)TameElement.ActiveTime != (int)lastTime))
                {
                    if (reset)
                    {
                        count = 0;
                        position.Clear();
                        rotation.Clear();
                        time.Clear();
                    }
                    position.Add(Camera.main.transform.position);
                    rotation.Add(Camera.main.transform.rotation);
                    time.Add(TameElement.ActiveTime);
                }
                lastTime = TameElement.ActiveTime;
            }
        }
    }
    public void GetElements()
    {
        headers = new string[4];
        headers[1] = "";
        headers[2] = "";
        foreach (GameObject g in elements)
        {
            TameElement te = TameManager.tes.Find(x => x.qMarker.gameObject == g);
            if (te != null)
            {
                tames.Add(te);
                headers[1] += (headers[1].Length > 0 ? "," : "") + te.name;
            }
            else
            {
                TameAlternative ta = TameManager.altering.Find(x => x.qMarker.gameObject == g);
                if (ta != null)
                {
                    alters.Add(ta);
                    headers[2] += (headers[2].Length > 0 ? "," : "") + ta.name;
                }
                else
                {
                    ta = TameManager.alteringMaterial.Find(x => x.qMarker.gameObject == g);
                    if (ta != null)
                    {
                        alters.Add(ta);
                        headers[2] += (headers[2].Length > 0 ? "," : "") + ta.name;
                    }
                }
            }
        }
        headers[3] = "";
        foreach (InfoUI.InfoControl ic in TameManager.info)
            foreach (InfoUI.QFrame fr in ic.frames)
                if (fr.choice.Count > 0)
                {
                    frames.Add(fr);
                    headers[3] += (headers[2].Length > 0 ? "," : "") + ic.marker.name + ":" + fr.index;
                }
    }
    void CheckElements(float t)
    {
        bool shouldSend = false;
        if (progress.CheckTime(lastTime, t))
            shouldSend = true;
        else
            foreach (TameElement te in tames)
                if (!te.progress.changeRecorded)
                    if (t - te.progress.lastChanged > 2)
                    {
                        shouldSend = true;
                        break;
                    }
        if (shouldSend)
        {
            string s = "";
            foreach (TameElement te in tames)
            {
                te.progress.changeRecorded = true;
                te.progress.lastChanged = t;
                s += te.progress.progress.ToString("0.000") + ",";
            }
            TNet.instance.Send(TNet.Progress, s);
        }
    }
    void CheckSurveys(float t)
    {
        bool shouldSend = false;
        if (surveys.CheckTime(lastTime, t))
            shouldSend = true;
        else
            foreach (InfoUI.QFrame fr in frames)
                if (!fr.changeRecorded)
                    if (t - fr.lastChanged > 2)
                    {
                        shouldSend = true;
                        break;
                    }
        if (shouldSend)
        {
            string s = "";
            foreach (InfoUI.QFrame fr in frames)
            {
                fr.changeRecorded = true;
                fr.lastChanged = t;
                for (int i = 0; i < fr.choice.Count; i++)
                    if (fr.choice[i].slider != null)
                        s += '@' + fr.choice[i].value;
                    else if (fr.choice[i].selected)
                        s += i + "+";
            }
            TNet.instance.Send(TNet.Progress, s);
        }
    }
    void CheckChoices(float t)
    {
        bool shouldSend = false;
        if (choice.CheckTime(lastTime, t))
            shouldSend = true;
        else
            foreach (TameAlternative ta in alters)
                if (!ta.changeRecorded)
                    if (t - ta.lastChanged > 2)
                    {
                        shouldSend = true;
                        break;
                    }
        if (shouldSend)
        {
            string s = "";
            foreach (TameAlternative ta in alters)
            {
                ta.changeRecorded = true;
                ta.lastChanged = t;
                s += ta.current + ",";
            }
            TNet.instance.Send(TNet.Alter, s);
        }
    }



}
[Serializable]
public class StateSender
{
    public bool on = false;
    [Range(10f, 60f)]
    public float interval;
    public bool CheckTime(float last, float now)
    {
        return on && ((int)(now / interval)) != ((int)(last / interval));
    }
}

