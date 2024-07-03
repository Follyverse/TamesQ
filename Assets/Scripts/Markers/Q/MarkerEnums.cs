using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markers
{
    public enum MarginType { Width, Height, Pixels }
    public enum QType { Mechanical, Material, Light, Custom, AlterObject, AlterMaterial, Info }
    public enum MoveAlter { None, ToInitial, ToMarker }
    public enum Vertical { Top, Bottom, Stretch }
    public enum Horizontal { Left, Right, Stretch }
    public enum InfoPosition { OnObject, WithObject, Top, Bottom, TopLeft, BottomRight, TopRight, BottomLeft, Left, Right }
    public enum ImagePosition { Top, Bottom, TopLeft, BottomRight, TopRight, BottomLeft, Left, Right }
    public enum TextPosition { Left, Mid, Right, Justified }
    public enum InfoOrder { ReplacePrevious, ReplaceAll, AddVertical, AddHorizontal, ReplaceImage }
    public enum EditorChangerType { Color, U_Offset, V_Offset, EmissiveColor, Emissive_U, Emissive_V, Focus, Intensity }
    public enum EditorChangeStep { Stepped, Gradual, Switch }
    public enum ControlTarget { Progress, Activation, Visibility, Alter }
    public enum ControlType { Manual, Element, Object, Time }
}
