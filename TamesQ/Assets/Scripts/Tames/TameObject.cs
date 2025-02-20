using Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Markers;
namespace Tames
{
    /// <summary>
    /// this class establishes a link between a <see cref="gameObject"/> and its parent <see cref="TameElement"/>. This is used to quickly understand what game object is connected to which element, and use this to find parental relationships between elements as well.
    /// </summary>
    public class TameGameObject
    {
        public TameThing thing = null;
        public QProgress markerProgress = null;
        /// <summary>
        /// this is used to know if the object is already included in a name search
        /// </summary>
        public bool alreadyFound = false;
        /// <summary>
        /// the game object in the <see cref="TameManager.RootObject"/>
        /// </summary>
        public GameObject gameObject;
        /// <summary>
        /// the lowest elements whose corresponding gameobject is <see cref="gameObject"/> or a parent or ancestor of it. 
        /// </summary>
        public TameElement tameParent;
        public bool isElement = false;
        /// <summary>
        /// transform of the <see cref="gameObject"/>.
        /// </summary>
        public Transform transform { get { return gameObject.transform; } }
        /// <summary>
        /// finds the first <see cref="TameGameObject"/> based on its <see cref="gameObject"/> name.
        /// </summary>
        /// <param name="name">the search name</param>
        /// <param name="tgos">the list of items</param>
        /// <returns>the found item, or null if not</returns>
        public static TameGameObject Find(string name, List<TameGameObject> tgos)
        {
            foreach (TameGameObject tame in tgos)
                if (tame.gameObject.name.ToLower().Equals(name.ToLower()))
                    return tame;
            return null;
        }
        /// <summary>
        /// finds the first <see cref="TameGameObject"/> based on its <see cref="gameObject"/> name.
        /// </summary>
        /// <param name="name">the search name</param>
        /// <param name="tgos">the list of items</param>
        /// <returns>the found item, or null if not</returns>
        public static TameGameObject Find(GameObject g, List<TameGameObject> tgos)
        {
            foreach (TameGameObject tgo in tgos)
                if (tgo.gameObject == g)
                    return tgo;
            return null;
        }
        /// <summary>
        /// lists the name of objects inside a list of TameGameObjects
        /// </summary>
        /// <param name="tgos">the list of TameGameObjects</param>
        /// <returns></returns>
        public static List<GameObject> ToObjectList(List<TameGameObject> tgos)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (TameGameObject tgo in tgos)
                list.Add(tgo.gameObject);
            return list;
        }

    }
    /// <summary>
    /// this class represents all <see cref="TameElement"/>s that are mechanical in nature.
    /// </summary>
    public class TameObject : TameElement
    {
        public float scaleFrom, scaleTo;
        public bool scales = false;
        public int scaleAxis = -1;
        public int scaleUV = 0;
        public MarkerQueue markerQueue = null;
//        public QC markerCycle = null;
        /// <summary>
        /// the movement handle of this element. This is responsible for all machanical movements within the element, usually dictated by a <see cref="TameProgress"/>.
        /// </summary>
        public TameHandles handle;

        /// <summary>
        /// the parent tame object of this object (this is the parent object in the 3D model, not in the update hierarchy 
        /// </summary>
        public TameObject parentObject = null;
        public TameGameObject tameGameObject = null;
        public TameObject()
        {
            handle = new TameHandles();
            thingType = ThingType.Element;
        }
        /// <summary>
        /// see <see cref="TameElement.Update(float)"/>
        /// </summary>
        /// <param name="p"></param>
        public override void Update(float p)
        {
            if (progress != null)
            {
                float pp = progress.subProgress;
                SetProgress(p);
                handle.Move(p, pp);
            }
        }
        /// <summary>
        /// updates the elements based on a parent element's progress. 
        /// </summary>
        /// <param name="p"></param>
        override public void Update(TameProgress p)
        {
            SetByParent(p);
            //     base.Update(p);
            if (progress != null)
            {
                handle.Move(progress.subProgress, progress.lastSub);
            }
        }
        /// <summary>
        /// updates the object based on position
        /// </summary>
        /// <param name="p"></param>
        override public void Update(Vector3 p)
        {
            float m = progress.progress;
            if (progress != null)
            {
                m = handle.Move(p, progress.progress, progress.manager.Speed, TameElement.deltaTime);
                progress.SetProgress(m);
            }
        }
        /// <summary>
        /// <see cref="TameElement.Update"/>
        /// </summary>
        public override void UpdateManually()
        {
            base.UpdateManually();
            if (progress != null)
            {
                handle.Move(progress.subProgress, progress.lastSub);
            }
        }
        override public void Update()
        {
            if (directProgress >= 0)
                Update(directProgress);
            else
                SetByTime();
            if (progress != null)
            {
                handle.Move(progress.subProgress, progress.lastSub);
                if (name == "last-door.001")
                {
                    //        handle.path.MoveVirtual(progress.slerpProgress);
                    //       Debug.Log(" last " + handle.path.virtualMover.position + " " + handle.path.bases[0].position);
                    //       TameOrbit to  =(TameOrbit)handle.path;
                    //       Debug.Log(" >> "+ (to.self.TransformPoint(to.axis + to.pivot) - to.self.TransformPoint(to.pivot)));

                }
                if (name == "rotax")
                {
                    //     handle.path.MoveVirtual(progress.slerpProgress);
                    //     Debug.Log(" rotax " + handle.path.virtualMover.position + " " + handle.path.bases[0].position);
                    //       TameOrbit to  =(TameOrbit)handle.path;
                    //       Debug.Log(" >> "+ (to.self.TransformPoint(to.axis + to.pivot) - to.self.TransformPoint(to.pivot)));

                }
            }
            //              SetChildren();
        }
        /// <summary>
        /// updates the element based on gripping hand
        /// </summary>
        public override void Grip(Updater tp)
        {
            if (progress != null)
            {
                //         float m = handle.Grip(tp.position, areas[tp.areaIndex].localGripVector, areas[tp.areaIndex].displacement, p.progress);
                //          p.SetProgress(m);
            }
        }
        public void Grip(float delta)
        {
            //       Debug.Log(name + " " + (progress == null ? "null" : "-"));
            if (progress != null)
            {
                progress.SetProgress(progress.progress + delta);
                handle.Move(progress.progress, progress.lastProgress);
            }
        }

        private void AddArea(TameArea ti, GameObject g = null)
        {
            TameArea ti2 = ti;
            if (ti.update == InteractionUpdate.Object)
            {
                ti2 = ti.Duplicate();
                ti2.gameObject = new GameObject(g.name);
                ti2.gameObject.transform.SetParent(g.transform);
                ti2.gameObject.transform.rotation = g.transform.rotation;
                ti2.gameObject.transform.localScale = g.transform.localScale;
                ti2.SetUpdate(this, g);
            }
            if (ti.update == InteractionUpdate.Mover)
            {
                ti2 = ti.Duplicate();
                ti2.gameObject = new GameObject();
                ti2.gameObject.transform.SetParent(mover.transform);
                ti2.gameObject.transform.rotation = ti.gameObject.transform.rotation;
                ti2.gameObject.transform.localScale = ti.gameObject.transform.localScale;
                ti2.SetUpdate(this, g);
            }
            if (ti.update != InteractionUpdate.Parent)
            {
                ti2 = ti.Duplicate();
                ti2.gameObject = new GameObject();
                ti2.gameObject.transform.SetParent(owner.transform);
                ti2.gameObject.transform.rotation = ti.gameObject.transform.rotation;
                ti2.gameObject.transform.localScale = ti.gameObject.transform.localScale;
                ti2.SetUpdate(this, g);
            }
            areas.Add(ti2);
        }
        public override void AssignParent(Updater[] all, int index)
        {
            Updater ps = GetParent(null);
            all[index] = ps;
        }
        public bool HasGrippableParent()
        {
            foreach (Updater p in updaters)
                if (p.isGrippable) return true;
            return false;
        }
        public static TameObject Create(GameObject g)
        {
            TameHandles handle = TameHandles.GetHandles(g);
            if (handle != null)
            {
                TameObject to = new TameObject() { handle = handle };
                QMarker marker = g.GetComponent<QMarker>();
                if (marker != null)
                    if (marker.qtype == QType.Mechanical)
                    {
                        to.qMarker = marker;
                        to.markerProgress = marker.progress;
                        to.markerSpeed = g.GetComponent<MarkerSpeed>();
                    }
                to.handle.path.element = to;
                to.mover = handle.mover;
                to.owner = g;
                to.name = g.name;
                return to;
            }
            return null;
        }
     
        private static TameGameObject CheckLight(GameObject owner, List<TameElement> tes, TameElement parentElement, int software)
        {
            QMarker ml = null;
            Material mat = null;
            float ri = 0;
            List<Light> lights = new List<Light>();
            if ((ml = owner.GetComponent<QMarker>()) != null)
                if (ml.qtype == QType.Light)
                {
                    mat = ml.material;
                    ri = ml.relativeIntensity;
                    Debug.Log("light check: " + owner.name);
                    lights.AddRange(ml.lights);
                    if (lights.Count > 0 && ml != null)
                    {
                        TameLight tl = new TameLight()
                        {
                            name = owner.name,
                            owner = owner,
                            mover = owner,
                            lights = lights,
                            index = (ushort)tes.Count,
                            material = mat,
                            relativeIntensity = ri,
                            qMarker = ml
                        };
                        if (ml != null) tl.markerProgress = ml.progress;
                        //MarkerFlicker[] mf = owner.GetComponents<MarkerFlicker>();
                        // if (mf.Length == 0) mf = null;
                        //        tl.markerFlicker = mf;
                        tl.GetAreas(software);
                        tl.startingParent = parentElement;
                        //   MarkerProgress mp = owner.GetComponent<MarkerProgress>();
                        //   if (mp != null) tl.markerProgress = mp;
                        //     tl.changers = owner.GetComponents<MarkerChanger>();
                        tes.Add(tl);
                        return new TameGameObject() { gameObject = owner, tameParent = tl, markerProgress = tl.markerProgress, isElement = true };
                    }
                }
            return null;
        }
        private static TameGameObject CheckCustom(GameObject owner, List<TameElement> tes, TameElement parentElement)
        {
            QProgress mc;
            QMarker qm;
            if ((qm = owner.GetComponent<QMarker>()) != null)
                if (qm.qtype == QType.Custom)
                {

                    TameCustomValue tc = new TameCustomValue()
                    {
                        name = owner.name,
                        owner = owner,
                        mover = owner,
                        index = (ushort)tes.Count,
                        markerProgress = qm.progress,
                        markerSpeed = owner.GetComponent<MarkerSpeed>(),
                        qMarker = qm
                    };

                    tc.GetAreas();
                    tc.startingParent = parentElement;
                    //     tl.changers = owner.GetComponents<MarkerChanger>();
                    if (tc.name == "lights") Debug.Log("q7 : " + tc.name);
                    tes.Add(tc);
                    return new TameGameObject() { gameObject = owner, tameParent = tc, markerProgress = tc.markerProgress, isElement = true };
                }
            return null;
        }

        private static TameGameObject CheckArea(GameObject owner, List<TameElement> tes, TameElement parentElement)
        {
            MarkerArea mc;
            if ((mc = owner.GetComponent<MarkerArea>()) != null)
                if (mc.appliesTo == null)
                {
                    TameCustomValue tc = new TameCustomValue()
                    {
                        name = mc.name,
                        owner = owner,
                        mover = owner,
                        index = (ushort)tes.Count,
                        markerProgress = mc.progress,
                        markerSpeed = owner.GetComponent<MarkerSpeed>(),
                        qMarker = null
                    };
                    tc.GetAreas(-1, true);
                    tc.owner.SetActive(true);
                    foreach (TameArea ta in tc.areas)
                    {
                        if (ta.IsVolumetric) ta.gameObject.SetActive(false);
                        if (ta.geometry == InteractionGeometry.Distance)
                            tc.updaters.Clear();
                    }

                    Debug.Log("Custom: " + tc.name);
                    //    if (tc.areas.Count > 0) Debug.Log("Custom: " + tc.areas[0].gameObject.name);
                    tc.startingParent = parentElement;
                    //     tl.changers = owner.GetComponents<MarkerChanger>();
                    tes.Add(tc);
                    return new TameGameObject() { gameObject = owner, tameParent = tc, markerProgress = tc.markerProgress, isElement = true };

                }
            return null;
        }
        public static List<TameGameObject> CreateInteractive(TameElement parentElement, GameObject owner, List<TameElement> tes, int software = -1)
        {
            MarkerOrigin mo;
            if ((mo = owner.GetComponent<MarkerOrigin>()) != null)
                software = mo.GetOrigin();

            List<TameGameObject> tgo = new List<TameGameObject>();
            TameGameObject tg = null;
            GameObject gi;
            int cc = owner.transform.childCount;
            //    TameElement te;
            TameObject obj;
            //     TameElement leader = null;
            TameElement[] local = new TameElement[cc];
            //     Debug.Log("creating " + owner.name);
            //     Debug.Log("check: " + owner.name+" "+ cc);
            for (int i = 0; i < cc; i++)
            {
                local[i] = null;
                gi = owner.transform.GetChild(i).gameObject;
                //    Debug.Log("check: " + owner.name + " ?");
                //    if (gi.name == "path") Debug.Log("path ");
                if (!TameHandles.HandleKey(gi, false))
                {
                    tg = null;
                    obj = Create(gi);
                    if (obj != null)
                    {
                        //                  Debug.Log("obj: "+obj.name);
                     //   obj.markerCycle = gi.GetComponent<MarkerCycle>();
                        obj.markerQueue = gi.GetComponent<MarkerQueue>();
                        obj.GetAreas();
                        obj.parentObject = parentElement.tameType == TameKeys.Object ? (TameObject)parentElement : null;
                        obj.index = (ushort)tes.Count;
                        tes.Add(obj);
                        local[i] = obj;
                        if (obj.handle.trackBasis == TrackBasis.Head)
                            obj.startingParent = null;
                        else
                            obj.startingParent = parentElement;
                    }
                    else
                    if ((tg = CheckLight(gi, tes, parentElement, software)) == null)
                        if ((tg = CheckCustom(gi, tes, parentElement)) == null)
                            tg = CheckArea(gi, tes, parentElement);
                    if (tg == null) tg = new TameGameObject()
                    {
                        gameObject = gi,
                        tameParent = obj == null ? parentElement : obj,
                        markerProgress = obj == null ? null : obj.markerProgress,
                        isElement = obj != null
                    };
                    tgo.Add(tg);
                    if (obj != null) obj.tameGameObject = tg;
                }
            }
            //       Debug.Log("check: " + owner.name + " " + cc);
            for (int i = 0; i < cc; i++)
            {
                //       Debug.Log("check: " + owner.name + " at " + i + " of "+ cc);
                gi = owner.transform.GetChild(i).gameObject;
                tgo.AddRange(CreateInteractive(local[i] ?? parentElement, gi, tes, software));
            }

            return tgo;
        }
    
        public void CreateClones(List<TameGameObject> gos, List<TameElement> tes)
        {
            List<Vector3> linkedPositions = new List<Vector3>();
            List<Quaternion> linkedRotations = new List<Quaternion>();
            TameObject to;
            TameArea ta;
            Transform t;
            for (int i = 0; i < gos.Count; i++)
            {
                linkedPositions.Add(gos[i].transform.position - mover.transform.position);
                linkedRotations.Add(gos[i].transform.rotation * Quaternion.Inverse(mover.transform.rotation));
                to = new TameObject();
                to.name = gos[i].gameObject.name;
                to.progress = progress == null ? null : new TameProgress(progress);
                to.progress = progress == null ? null : new TameProgress(progress);
                to.updaters = updaters;
                //  to.basis = basis;
                to.handle = TameHandles.Duplicate(handle, gos[i].gameObject);
                to.index = (ushort)tes.Count;
                to.mover = gos[i].gameObject;
                to.owner = gos[i].transform.parent.gameObject;
                to.updaters.Add(new Updater(to, TrackBasis.Grip));
                to.tameType = TameKeys.Object;
                foreach (TameArea a in areas)
                {
                    if (a.update == InteractionUpdate.Fixed)
                        to.areas.Add(a);
                    else
                    {
                        ta = a.Duplicate();
                        if (a.update == InteractionUpdate.Object)
                        {
                            t = gos[i].transform.Find(a.relative.name);
                            if (t != null)
                                ta.SetUpdate(to, t.gameObject);
                            else
                                ta.SetUpdate(to, gos[i].gameObject);
                        }
                        else
                            ta.SetUpdate(to, null);
                        to.areas.Add(ta);
                    }
                }
                tes.Add(to);
            }

        }
    }
}