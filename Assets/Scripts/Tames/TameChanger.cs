using System.Collections.Generic;
using UnityEngine;
namespace Tames
{
    /// <summary>
    /// hosts a float array that contains the values of each step of a changer
    /// </summary>
    public class TameNumericStep
    {
        public float[] value;
    }

    /// <summary>
    /// the class is used for changing the material and light properties for a given progress value
    /// </summary>
    public class TameChanger
    {
              /// <summary>
        /// An auxilliary value that is used currently to set the base emissive intensity. Only works when <see cref="property"/> is set as <see cref="MaterialProperty.Glow"/>
        /// </summary>
        public float factor = 0;
        /// <summary>
        /// the list of stops, each stop contains a value relative to 0..1 progress (stop[i] is for progress = i / (float)(stop.Count -1)
        /// /// </summary>
        public List<TameNumericStep> steps;
         /// <summary>
        /// 
        /// </summary>
        public float toggle = 0.5f;
        /// <summary>
        /// 
        /// </summary>
        public ToggleType toggleType;
        /// <summary>
        /// the material or light property to be changed
        /// </summary>
        public MaterialProperty property;
        /// <summary>
        /// count of the values in each stop (1: float, 3: color)
        /// </summary>
        public int count;
        public TameElement parent = null;
        public Markers.QChanger marker;
        /// <summary>
        /// returns the value(s) that the property should have on the progress = p 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float[] On(float p, float tp, ContinuityMode cycle = ContinuityMode.Stop)
        {
            int index, sc = steps.Count;
            float k, d;
            int dc;
            if (parent != null)
            {
                p = parent.progress.subProgress;
                tp = parent.progress.totalProgress;
            }
            if (p >= 1) return steps[^1].value;
            if (p <= 0) return steps[0].value;
            switch (toggleType)
            {
                case ToggleType.Stepped:
                    if (cycle == ContinuityMode.Reverse)
                    {
                        dc = (sc - 1) * 2;
                        index = (int)(p * dc);
                        index = (index + 1) / 2;
                    }
                    else
                    {
                        index = (int)(p * sc);
                        if (index == sc) index--;
                    }
                    return steps[index].value;
                case ToggleType.Gradual:

                    k = 1f / (sc - 1);
                    d = (p % k) / k;
                    index = (int)(p / k);
                    float[] r = new float[count];
                    for (int i = 0; i < count; i++)
                        r[i] = steps[index].value[i] + (steps[index + 1].value[i] - steps[index].value[i]) * d;
                    return r;
                default:
                    if (p >= toggle) return steps[^1].value; else return steps[0].value;               
            }
        }
        public void UpdateMarker()
        {
            TameChanger ch;
            TameColor cc;
            if (count > 1 && marker.colorSteps.Length > 0)
            {
                cc = TameColor.ReadStepsOnly(marker.colorSteps, toggleType, toggle, true);
                steps = cc.steps;
            }
            else
            {
                ch = ReadStepsOnly(marker.steps, toggleType, toggle, count);
                if (ch != null)
                    steps = ch.steps;
            }
            toggle = marker.switchValue;
            toggleType = marker.GetToggle();
        }

        public static TameChanger ReadStepsOnly(string line, ToggleType st, float sv, int n)
        {
            string clean = Utils.Clean(line);
            List<string> si, s = Utils.Split(clean, " ");
            float f;
            //  Debug.Log("read-y: " + s.Count);
            List<TameNumericStep> steps = new List<TameNumericStep>();
            float[] value;
            //   int n;
            for (int i = 0; i < s.Count; i++)
            {
                si = Utils.Split(s[i], ",");
                value = new float[n];
                if (s.Count < n) return null;
                for (int j = 0; j < n; j++)
                    if (Utils.SafeParse(si[j], out f))
                        value[j] = f;
                    else
                        return null;
                steps.Add(new TameNumericStep() { value = value });
            }
            return new TameChanger()
            {
                steps = steps,
                toggleType = st,
                toggle = sv,
                count = n
            };
        }
        private const int FlickPlanCount = 3;
        private void Randomize(int[] a, int n, int spare)
        {
            int e, j;
            for (int i = 0; i < n; i++)
            {
                j = a.Length - i - 1;
                int r = UnityEngine.Random.Range(0, a.Length - n - spare);
                e = a[r];
                a[r] = a[j];
                a[j] = e;
                //       Debug.Log("flicker " + i + " > " + r);
            }
        }
        public void SetFlickerPlan()
        {
            return;
           
        }
        public void From(TameChanger tch)
        {
            count = tch.count;
            property = tch.property;
            steps = tch.steps;
            toggle = tch.toggle;
            toggleType = tch.toggleType;
            marker = tch.marker;
            parent = tch.parent;
        }
        public void FindParent(List<TameElement> tes)
        {
            if (marker.byElement != null)
                foreach (TameElement te in tes)
                    if (te.owner == marker.byElement)
                    {
                        Debug.Log("bef: " + te.name);
                        parent = te;
                        break;
                    }
        }
    }
}