using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi
{
    public class QPlayer
    {

    }
    public class QElement
    {
        public const byte Element = 1;
        public const byte AltMat = 2;
        public const byte AltObj = 3;
        public byte type;
        public bool stepped;
        public ushort index;
        public float progress, lastProgress, totalProgress, lastTotal, subProgress;
        public byte stepFrom, stepTo;
        public bool manual;
        public void ToMessage(Message m)
        {
            m.AddByte(type);
            m.AddUShort(index);
            m.AddBool(manual);

        }
    }
}
