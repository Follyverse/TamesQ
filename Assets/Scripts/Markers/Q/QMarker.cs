using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Markers
{
    
    [System.Serializable]
    public class InfoItem
    {
        public Texture image;
        [SerializeField]
        [TextAreaAttribute(5, 10)]
        private string text;
        public int lineCount = 10;
        public InfoOrder replace = InfoOrder.ReplacePrevious;
        public string Text { get { return text; } }
    }
    [Serializable]
    public class BackMargin
    {
        public float margin;
        public MarginType type;
    }
    [Serializable]
    public class InfoColors
    {
        public Color background = Color.black;
        public Texture image = null;
        [Tooltip("Relative to the image's width")]
        public BackMargin margin;

        public Color foreground = Color.white;
        public Color choice = Color.yellow;
        public Color hoover = Color.blue;
        //     public Color hooverColor = lastChoice;
        public Color[] others;
        public Color Get(int i)
        {
            return i ==0?foreground:(i-1<others.Length?others[i-1]:foreground);
        }
    }
    public enum PosAxis { X, Y, Z, nX, nY, nZ}
    [Serializable]
    public class InfoPosManager
    {
        public InfoPosition position;
        public PosAxis X = PosAxis.X;
        public PosAxis Y = PosAxis.Y;
    }
    [Serializable]
    public class InfoLocation
    {
        public ImagePosition imagePosition;
        public TextPosition textPosition;
        public float margin;
       public bool onlyForText = false;
        public Vector2 size = new Vector2(1, 0.2f);
        public float Width { get { return size.x; } }
        public float Height { get { return size.y; } }
        public InfoPosManager position;
       }
    public class QMarker : MonoBehaviour
    {
        public QType qtype;
        // Material only
        public bool unique;
        // Light & material
        public Material material;
        public QChanger[] changer;
        // Light only
        public Light[] lights;
        public GameObject[] childrenOf;
        public GameObject[] descendantsOf;
        public float relativeIntensity;
        public Tames.Flicker flicker;
        // Mech only
        public QCycle cycler;
        public QScale scaler;
        // all alter
        public bool cycle = true;
        // mat alter
        public Material initialMaterial = null;
        public Material[] materials;
        // obj alter
        public GameObject syncWith;
        public GameObject initialObject;
        public MoveAlter moveTo;
        public GameObject[] objects;
        // info
        public bool detached = false;
        public bool appearOnce = false;
        public Vertical vertical { get { return GetVertical(); } }
        public Horizontal horizontal { get { return GetHorizontal(); } }
        //    public int lineCount = lastLine;
        public InfoColors colorSetting;
        public InfoLocation position;
        public InfoItem[] frames;
        public InputSetting pageControl;
        public GameObject[] areas;
        public GameObject[] references;
        public Texture[] inLineImages;
        public TMPro.TMP_FontAsset[] font;
        // Progress
        public QProgress progress;
        // Control
        public QControl[] controls;
        private void Update()
        {
            if(!Tames.TameElement.isPaused)
            flicker.Next();
        }
        Vertical GetVertical()
        {
            return position.imagePosition switch
            {
                ImagePosition.Top => Vertical.Top,
                ImagePosition.TopLeft => Vertical.Top,
                ImagePosition.TopRight => Vertical.Top,
                ImagePosition.Bottom => Vertical.Bottom,
                ImagePosition.BottomRight => Vertical.Bottom,
                ImagePosition.BottomLeft => Vertical.Bottom,
                _ => Vertical.Stretch,
            };
        }
        Horizontal GetHorizontal()
        {
            return position.imagePosition switch
            {
                ImagePosition.Left => Horizontal.Left,
                ImagePosition.TopLeft => Horizontal.Left,
                ImagePosition.TopRight => Horizontal.Right,
                ImagePosition.BottomLeft => Horizontal.Left,
                ImagePosition.Right => Horizontal.Right,
                ImagePosition.BottomRight => Horizontal.Right,
                _ => Horizontal.Stretch,
            };
        }
    }
    [Serializable]
    public class QScale
    {
        public bool active = false;
        public GameObject byObject = null;
        public string byName = "";
        public GameObject childrenOf = null;
        public enum ScaleAxis { X, Y, Z }
        public ScaleAxis axis = ScaleAxis.X;
        public float from;
        public float to;
        public enum AffectUV { U, V }
        public AffectUV affectedUV = AffectUV.U;
    }
    [Serializable]
    public class QCycle
    {
        public bool active = false;
        public float offset = 0;
        public string itemNames = "";
        public GameObject[] childrenOf;

    }
    [Serializable]
    public class QChanger
    {
        public const int TypeID = 4;
        public MaterialProperty property;

        public GameObject byElement = null;
        public EditorChangeStep mode;
        public float switchValue;
        public string steps;
        public Color[] colorSteps;
        public float factor = 0;
        //   public Flicker flicker;
        public Tames.TameChanger changer = null;
        //  private bool changed = false;       


        private static EditorChangerType CT(string s)
        {
            return s switch
            {
                "Color" => EditorChangerType.Color,
                "U_Offset" => EditorChangerType.U_Offset,
                "V_Offset" => EditorChangerType.V_Offset,
                "EmissiveColor" => EditorChangerType.EmissiveColor,
                "Focus" => EditorChangerType.Focus,
                "Intensity" => EditorChangerType.Intensity,
                "Emissive" => EditorChangerType.Emissive_U,
                _ => EditorChangerType.Emissive_V,
            };
        }
        public ToggleType GetToggle()
        {
            return mode switch
            {
                EditorChangeStep.Gradual => ToggleType.Gradual,
                EditorChangeStep.Switch => ToggleType.Switch,
                _ => ToggleType.Stepped,
            };
        }
        private static EditorChangeStep TT(string s)
        {
            return s switch
            {
                "Gradual" => EditorChangeStep.Gradual,
                "Switch" => EditorChangeStep.Switch,
                _ => EditorChangeStep.Stepped,

            };
        }
    }
    [Serializable]
    public class QControl
    {
        public const int TypeID = 5;
        public ControlType mode;
        public ControlTarget effect;
        // activation/visibility
        public bool initial = true;
        // alter
        public float interval = 1;
        // manual
        public InputSetting control;
        // element
        public GameObject parent;
        public string trigger;
        // object
        public bool withPeople = false;
        public bool withPeoploids = false;
        public GameObject[] trackables;
    }
    [Serializable]
    public class QProgress
    {
        public ContinuityMode continuity = ContinuityMode.Stop;
        public string steps = "0";
        public float preset = 0;
        public float setTo = 0;
        public float duration = 1;
        public string lerpXY = "";
        public bool active = true;
        public Tames.TameElement element = null;
        private bool changed = false;
    }
}
