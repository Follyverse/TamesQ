using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Tames
{
    public class DeviceControl
    {
        public uint uPressed = 0;
        public uint uHeld = 0;
        public bool[] pressed = new bool[32], lastFrame = new bool[32];
        public bool[] held = new bool[32];
        UnityEngine.InputSystem.Controls.ButtonControl[] controls = new UnityEngine.InputSystem.Controls.ButtonControl[32]; UnityEngine.InputSystem.Controls.ButtonControl[] auxControl = new UnityEngine.InputSystem.Controls.ButtonControl[3];
        public bool[] aux = new bool[3];
        public void Update()
        {
            uPressed = 0;
            uHeld = 0;
            for (int i = 0; i <= last; i++)
            {
                if (lastFrame[i]) pressed[i] = false;
                else pressed[i] = controls[i].wasPressedThisFrame;
                uPressed += (uint)(pressed[i] ? 1 << i : 0);
                held[i] = controls[i].isPressed;
                uHeld += (uint)(held[i] ? 1 << i : 0);
            }
        }
        private int last = -1;
        public int Add(UnityEngine.InputSystem.Controls.ButtonControl c)
        {
            controls[last + 1] = c;
            return last++;
        }
    }
    public class Inputter
    {
        public static Inputter Instance;
        public DeviceControl kb, gp, vr;
        public Inputter()
        {
            kb = new DeviceControl();
          //  kb.SetAux
            gp = new DeviceControl();
            vr = new DeviceControl();
        }
        public void Update()
        {

        }
        private int AddKey(UnityEngine.InputSystem.Controls.ButtonControl k)
        {
            return kb.Add(k);
        }
        public int AddKey(string key)
        {
            switch (key)
            {
                case "1": return AddKey(Keyboard.current.digit1Key);
                case "2": return AddKey(Keyboard.current.digit2Key);
                case "3": return AddKey(Keyboard.current.digit3Key);
                case "4": return AddKey(Keyboard.current.digit4Key);
                case "5": return AddKey(Keyboard.current.digit5Key);
                case "6": return AddKey(Keyboard.current.digit6Key);
                case "7": return AddKey(Keyboard.current.digit7Key);
                case "8": return AddKey(Keyboard.current.digit8Key);
                case "9": return AddKey(Keyboard.current.digit9Key);
                case "0": return AddKey(Keyboard.current.digit0Key);
                case "b": return AddKey(Keyboard.current.bKey);
                case "e": return AddKey(Keyboard.current.eKey);
                case "f": return AddKey(Keyboard.current.fKey);
                case "g": return AddKey(Keyboard.current.gKey);
                case "h": return AddKey(Keyboard.current.hKey);
                case "i": return AddKey(Keyboard.current.iKey);
                case "j": return AddKey(Keyboard.current.jKey);
                case "k": return AddKey(Keyboard.current.kKey);
                case "l": return AddKey(Keyboard.current.lKey);
                case "m": return AddKey(Keyboard.current.mKey);
                case "n": return AddKey(Keyboard.current.nKey);
                case "o": return AddKey(Keyboard.current.oKey);
                case "p": return AddKey(Keyboard.current.pKey);
                case "q": return AddKey(Keyboard.current.qKey);
                case "r": return AddKey(Keyboard.current.rKey);
                case "t": return AddKey(Keyboard.current.tKey);
                case "u": return AddKey(Keyboard.current.uKey);
                case "v": return AddKey(Keyboard.current.vKey);
                case "y": return AddKey(Keyboard.current.yKey);
                case "left": return AddKey(Keyboard.current.leftArrowKey);
                case "right": return AddKey(Keyboard.current.rightArrowKey);
                case "up": return AddKey(Keyboard.current.upArrowKey);
                case "down": return AddKey(Keyboard.current.downArrowKey);
                case "comma": return AddKey(Keyboard.current.commaKey);
                case ";": return AddKey(Keyboard.current.semicolonKey);
                case "[": return AddKey(Keyboard.current.leftBracketKey);
                case "]": return AddKey(Keyboard.current.rightBracketKey);
                case "slash": return AddKey(Keyboard.current.slashKey);
                case "backslash": return AddKey(Keyboard.current.backslashKey);
                case "back": return AddKey(Keyboard.current.backslashKey);
                case "period": return AddKey(Keyboard.current.periodKey);
                case "dot": return AddKey(Keyboard.current.periodKey);
                case "=": return AddKey(Keyboard.current.equalsKey);
                case "-": return AddKey(Keyboard.current.minusKey);
                case "quote": return AddKey(Keyboard.current.quoteKey);
                 case "enter": return AddKey(Keyboard.current.enterKey);
                case "b0": return AddKey(Mouse.current.leftButton);
                case "b1": return AddKey(Mouse.current.leftButton);

                //
                default: return -1;
            }
        }
    }
}
