using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tames
{
    [Serializable]
    public class Flicker
    {
        // Start is called before the first frame update
        float modeStart = 0;
        float lastChecked = 0;
        float modeEnd = 0;
        enum Mode { Off, On, Rest }
        Mode mode = Mode.Rest;
        [Tooltip("Whether the flicker is smooth or abrupt.")]
        public bool gradual;
        [Tooltip("The strength of the flicker (0 is full strength, and 1 is no effect")]
        public float min;
        [Tooltip("Number of flickers after which the light remains on.")]
        public int restAfter = 3;
        int currentCount = 0;
        [Tooltip("Minimum and maximum duration of a flicker step")]
        public Vector2 off = Vector2.zero;
        [Tooltip("Minimum and maximum duration of a on-stage between each flicker")]
        public Vector2 on = Vector2.one;
        [Tooltip("Minimum and maximum duration of resting period")]
        public Vector2 rest = Vector2.one;
        [HideInInspector]
        public float value = 1;
        public void Next()
        {
            float dt = Time.deltaTime;
            float r;
            if (dt + lastChecked > modeEnd)
            {
                switch (mode)
                {
                    case Mode.Rest:
                        if (off.y - off.x > 0)
                        {
                            r = off.x + UnityEngine.Random.value * (off.y - off.x);
                            currentCount = 1;
                            modeStart = TameElement.ActiveTime;
                            mode = Mode.Off;
                            modeEnd = r + modeStart;
                        }
                        break;
                    case Mode.Off:
                        if (currentCount == restAfter)
                        {
                            currentCount = 0;
                            r = rest.x + UnityEngine.Random.value * (rest.y - rest.x);
                            modeStart = TameElement.ActiveTime;
                            mode = Mode.Rest;
                            modeEnd = r + modeStart;
                        }
                        else
                        {
                            r = on.x + UnityEngine.Random.value * (on.y - on.x);
                            modeStart = TameElement.ActiveTime;
                            mode = Mode.On;
                            modeEnd = r + modeStart;
                        }
                        break;
                    case Mode.On:
                        if (off.y - off.x > 0)
                        {
                            r = off.x + UnityEngine.Random.value * (off.y - off.x);
                            modeStart = TameElement.ActiveTime;
                            currentCount++;
                            mode = Mode.Off;
                            modeEnd = r + modeStart;
                        }
                        break;
                }
            }
            lastChecked = TameElement.ActiveTime;
            switch (mode)
            {
                case Mode.Off:
                    if (gradual)
                    {
                        if (modeEnd > modeStart)
                            value = min + Mathf.Abs(lastChecked - (modeEnd + modeStart) / 2) / ((modeEnd + modeStart) / 2) * (1 - min);
                        else value = 1;
                    }
                    else
                        value = min;
                    break;
                default:
                    value = 1;
                    break;

            }

       if(mode==Mode.Off)Debug.Log("flicker " + value + " < " + (modeEnd-modeStart) + " " + TameElement.ActiveTime);
        }
    }
}