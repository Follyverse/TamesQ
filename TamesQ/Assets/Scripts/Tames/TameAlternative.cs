﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Markers;
using Multi;

namespace Tames
{
    public class TameAlternative : TameThing
    {
        public class Alternative
        {
            public List<GameObject> gameObject = new List<GameObject>();
        }
        public int count;
        public bool cycle;
        public bool multiControl = true;
        public List<Alternative> alternatives = new List<Alternative>();
        public int current = -1;
        public QMarker qMarker;
        public QControl markerControl;
        public int initial = -1;
        public float lastChanged = 0;
        public bool changeRecorded = false;

        private int initialIndex = 0;
        private int lastIndex, lastTotal, total;

        public TameAlternative()
        {
            thingType = ThingType.Alter;
            thingSubtype = ThingSubtype.Mechanical;
        }
        public override int CurrentIndex()
        {
            if (current >= 0) return current;
            else return 0;
        }
        public override int LastIndex()
        {
            return lastIndex;
        }
        public override int LastTotalIndex()
        {
            return lastTotal;
        }
        public override int TotalIndex()
        {
            return total;
        }
        public override float Progress()
        {
            return count <= 1 ? 1 : current / (float)(count - 1);
        }
        public override float LastProgress()
        {
            return count <= 1 ? 1 : lastIndex / (float)(count - 1);
        }
        public override float TotalProgress()
        {
            return count <= 1 ? 1 : total / (float)(count - 1);
        }
        public override float LastTotal()
        {
            return count <= 1 ? 1 : lastTotal / (float)(count - 1);
        }
        public void GoNext()
        {
            lastIndex = current;
            lastTotal = total;
            if (current >= 0)
            {
                if (count > 0)
                {
                    if (!cycle) current = current == count - 1 ? current : current + 1;
                    else current = (current + 1) % count;
                }
            }
            else if (count > 0) current = 0;
            total = current;
            if (cycle) total++; else total = current;
            Apply();
        }
        public void GoPrevious()
        {
            lastIndex = current;
            lastTotal = total;
            if (current >= 0)
            {
                if (count > 0)
                {
                    if (!cycle) current = current <= 0 ? 0 : current - 1;
                    else current = (current + count - 1) % count;
                }
            }
            else if (count > 0) current = 0;
            if (cycle) total--; else total = current;
            Apply();
        }
        public void Go(int i)
        {
            total = lastIndex = lastTotal = current = i;
            Apply();
        }
        public void SetInitial(int i)
        {
            if (count > 0)
            {
                initialIndex = i;
                current = i;
            }
            total = lastTotal = lastIndex = current;
            Apply();
        }
        public virtual void Apply()
        {
            if (current >= 0)
            {
                for (int i = 0; i < count; i++)
                    foreach (GameObject go in alternatives[i].gameObject)
                    {
                        go.SetActive(i == current);
                        if (i == current)
                            if (qMarker.moveTo == MoveAlter.ToMarker) go.transform.position = qMarker.transform.position;
                            else if (qMarker.moveTo == MoveAlter.ToInitial) go.transform.position = alternatives[initialIndex].gameObject[0].transform.position;
                    }
            }
            if (current!=lastIndex)
            {
                lastChanged = TameElement.ActiveTime;
                changeRecorded = false; 
            }
        }
        public virtual void Update()
        {
            //     if (name == "theme alter") Debug.Log(name + " " + count + " "+current);
            if ((count <= 0) || (current < 0))
                return;
            //    Debug.Log(name + " " );
            int d = CurrentUpdater.Directable();
            //   if (d != 0) Debug.Log("alter " + d);
            if (d < 0) GoPrevious();
            else if (d > 0) GoNext();
        }
        public void SetKeys()
        {
            markerControl.control.AssignControl(InputSetting.ControlType.DualPress);
            InputUpdate(markerControl.control, InputSetting.ControlType.DualPress);
        }

        public static List<TameAlternative> GetAlternatives(List<TameGameObject> tgos)
        {
            List<TameAlternative> tas = new();
            TameAlternative ta;
            QMarker ma;
            List<QMarker> mas = new List<QMarker>();
            List<QMarker> syncMarkers = new List<QMarker>();
            Alternative alt;
            for (int i = 0; i < tgos.Count; i++)
                if ((ma = tgos[i].gameObject.GetComponent<QMarker>()) != null)
              if(ma.qtype== QType.AlterObject)  {
                    //         Debug.Log("ALTERX " + tgos[i].gameObject.name);
                    if (ma.syncWith == null)
                        mas.Add(ma);
                    else
                        syncMarkers.Add(ma);
                }
            for (int i = 0; i < mas.Count; i++)
            {
                ta = new TameAlternative() { qMarker = mas[i], multiControl = false, cycle = mas[i].cycle, owner = mas[i].gameObject };
                QControl mc = mas[i].controls.Length > 0 ? mas[i].controls[0] : null;
                if (mc != null)
                {
                    ta.markerControl = mc;
                    ta.SetKeys();
                }
                for (int j = 0; j < mas[i].objects.Length; j++)
                    if (mas[i].objects[j] != null)
                    {
                        alt = new Alternative();
                        alt.gameObject.Add(mas[i].objects[j]);
                        for (int k = syncMarkers.Count - 1; k >= 0; k--)
                            if (syncMarkers[k].syncWith == mas[i].objects[j])
                            {
                                //         Debug.Log("ALTER " + syncMarkers[k].gameObject.name);
                                alt.gameObject.Add(syncMarkers[k].gameObject);
                                syncMarkers.RemoveAt(k);
                            }
                        ta.alternatives.Add(alt);
                    }
                int initial = 0;
                if (mas[i].initialObject != null)
                    for (int j = 0; j < ta.alternatives.Count; j++)
                        if (mas[i].initialObject == ta.alternatives[j].gameObject[0])
                        {
                            initial = j;
                            break;
                        }
                ta.owner = mas[i].gameObject;
                ta.name = mas[i].name;
                ta.count = ta.alternatives.Count;
                ta.SetInitial(initial);
                //         Debug.Log("alter: " + ta.name + " " + initial);
                tas.Add(ta);
            }
            return tas;
        }
        public void GetStatus(RiptideNetworking.Message m)
        {
            byte header = m.GetByte();
            if (header != 0)
            {
                current = m.GetInt();
                Apply();
            }
        }
        public void AddStatus(RiptideNetworking.Message m)
        {
            m.AddByte((byte)(multiControl ? 1 : 0));
            if (multiControl)
                m.AddInt(current);
        }

    }
}
