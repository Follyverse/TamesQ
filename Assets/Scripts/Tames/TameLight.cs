using System.Collections.Generic;
using UnityEngine;
using Multi;

namespace Tames
{
    /// <summary>
    /// the class containing the necessary properties for an interactive light. This class is only concerned with the light component not the game objects. 
    /// </summary>
    public class TameLight : TameElement
    {
        /// <summary>
        /// the Light component, automatically set in <see cref="TameManager"/> class. The component should be assigned to a game object that is a child or descendant of the root object "interactives" in the scene.
        /// </summary>
        public List<Light> lights;
        public Material material;
        public float relativeIntensity;
        //  public List<TameArea> areas;
         string[] keys = null;
        public TameLight()
        {
            tameType = TameKeys.Light;
            thingType = ThingType.Element;
        }

        /// <summary>
        /// ovverrides <see cref="TameElement.AssignParent"/>
        /// </summary>
        /// <param name="all"></param>
        /// <param name="index"></param>
        public override void AssignParent(Updater[] all, int index)
        {
            Updater ps = GetParent();
            //        if (name.Equals("blink-blue"))
            //        Debug.Log("custom: blink " + index+ " "+all.Length);
            // for (int i = 0; i < 3; i++)
            all[index] = ps;
        }
        /// <summary>
        /// applies updates after progress is set by other update methods: <see cref="Update()"/> and <see cref="Update(TameProgress)"/>. This uses information read from the manifest file and stored in the manifest headers of the <see cref="manifest"/> field.
        /// </summary>
        private void ApplyUpdate()
        {
            if (keys == null && material != null)
                keys = TameMaterial.GetKeys(material, out bool[] hasKeys);
            float[] f;
            Color c;
            //   ManifestLight m = (ManifestLight)manifest;
            //   Debug.Log("coll "+(m == null ? "null" : "not"));
            TameProgress progress = this.progress;
            float flicker = qMarker.flicker.value;
            if (progress != null)
            //           Debug.Log("color updating " + m.properties.Count);
            {
                foreach (TameChanger tc in properties)
                {
                    if (tc.parent != null) progress = tc.parent.progress; else progress = this.progress;
                    //      if (tc.parent != null) Debug.Log("xe: "+name+" < "+tc.parent.name+" : "+progress.progress);
                    f = tc.On(progress.subProgress, progress.totalProgress, progress.continuity);

                    switch (tc.property)
                    {
                        case MaterialProperty.Glow:
                        case MaterialProperty.Color:
                            c = TameColor.ToColor(f);
                        //    Debug.Log("el name " + name);
                            for (int i = 0; i < lights.Count; i++)
                                lights[i].color = c;
                            //       if (name == "cooler") Debug.Log("colj: " + name + " " + progress.progress + light.color.ToString());
                            break;
                        case MaterialProperty.Bright:
                            for (int i = 0; i < lights.Count; i++)
                                lights[i].intensity = f[0] * flicker;
                            break;
                        case MaterialProperty.Focus:
                            for (int i = 0; i < lights.Count; i++)
                                lights[i].spotAngle = f[0];
                            break;
                    }
                }
                if (material != null)
                {
                    material.SetColor(keys[TameMaterial.EmissionColor], qMarker.relativeIntensity * lights[0].intensity * lights[0].color);
                    material.SetColor(keys[TameMaterial.BaseColor],lights[0].color);
                }
            }
        }
        /// <summary>
        /// updates the light element as child of another <see cref="TameElement"/>, overriding <see cref="TameElement.Update(TameProgress)"/>
        /// </summary>
        /// <param name="p"></param>
        override public void Update(float p)
        {
            SetProgress(p);
            ApplyUpdate();
        }
        override public void Update(TameProgress p)
        {
            SetByParent(p);
            ApplyUpdate();
        }
        public override void UpdateManually()
        {
            base.UpdateManually();
            ApplyUpdate();
        }
        /// <summary>
        /// updates the light element by time, overriding <see cref="TameElement.Update"/>
        /// </summary>
        override public void Update()
        {
            if (directProgress >= 0)
                Update(directProgress);
            else
                SetByTime();
            ApplyUpdate();
        }
        public static List<TameChanger> ExternalChanger(Markers.QChanger[] chs)
        {
            TameChanger tch;
            TameColor tco;
            bool found;
            MaterialProperty mp;
            List<TameChanger> properties = new List<TameChanger>();
            int pcount = properties.Count;
            if (chs != null)
                foreach (Markers.QChanger ch in chs)
                {
                  //  if (ch.byElement != null) Debug.Log("be: " + ch.gameObject.name + " " + ch.byElement.name);
                    mp = ch.property;
                    switch (mp)
                    {
                        case MaterialProperty.Bright:
                        case MaterialProperty.MapY:
                        case MaterialProperty.LightY:
                        case MaterialProperty.MapX:
                        case MaterialProperty.LightX:
                        case MaterialProperty.Focus:
                            if ((tch = TameChanger.ReadStepsOnly(ch.steps, ch.GetToggle(), ch.switchValue, 1)) != null)
                                tch.property = mp;
                            break;
                        default:
                            if (ch.colorSteps.Length > 0)
                                tch = tco = TameColor.ReadStepsOnly(ch.colorSteps, ch.GetToggle(), ch.switchValue, false);
                            else
                                tch = tco = TameColor.ReadStepsOnly(ch.steps, ch.GetToggle(), ch.switchValue, false);
                            if (tch != null) tco.property = mp;
                            break;
                    }
                    if (tch != null)
                    {
                        tch.marker = ch;
                   //     ch.ChangedThisFrame(false);
                        found = false;
                        for (int i = 0; i < pcount; i++)
                            if (mp == properties[i].property)
                            {
                                if (tch.count == 1)
                                    properties[i].From(tch);
                                else
                                    ((TameColor)properties[i]).From((TameColor)tch);
                                found = true;
                            }
                        if (!found)
                            properties.Add(tch);
                    }
                }
            return properties;
        }
    }

}