using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InfoUI
{
    public class HashTag
    {
        public string leftover = "";
        public bool aligned = false;
        public TextAlignmentOptions alignment = TextAlignmentOptions.Left;

        public bool added = false, normal = false;
        public int context = 0;
        public FontStyles style;
        public InfoReference.RefProperty refProp = InfoReference.RefProperty.None;
        public int refIndex = -1;
        public int fontSize = -1;
        public int color = -1;

        public int columns = 1;
        public bool dotted = false;
        public float portion = 1;
        public bool divisor = false;

        public int font = -1;
        const int Error = -2;
        const int Repeated = -3;
     public   const int BaseSize = 8;
        const float Factor = 1.17f;
        static string refProps = "tpev", aligns = "cjlr", styles = "niub+-", duals = "zf|.";
        public static float Size(float bs, int size)
        {
            return bs * Mathf.Pow(Factor,  size- BaseSize);
        }
        public static int Dual(string prefix, char command, out string after, bool hex = false)
        {
            after = prefix;
            int p = prefix.IndexOf(command);
            if (p < 0) return -1;
            else if (p > prefix.Length - 2) return Error;
            else if (prefix[p + 1] == command)
            {
                after = prefix.Substring(0, p) + prefix.Substring(p + 2);
                return Repeated;
            }
            else
            {
                int v = hex ? "0123456789abcdef".IndexOf(prefix[p + 1]) : "0123456789".IndexOf(prefix[p + 1]);
                if (v >= 0)
                {
                    after = prefix.Substring(0, p) + prefix.Substring(p + 2);
                    return v;
                }
                else return Error;
            }
        }
        public static int After(string prefix, char command, string leftover, out string after)
        {
            after = prefix;
            if (prefix[^1] != command) return -1;
            else if (!int.TryParse(leftover, out int v)) return Error;
            else if (v < 0) return Error;
            else
            {
                after = prefix.Substring(0, prefix.Length - 1);
                return v;
            }
        }
        public static HashTag Get(string s, InfoControl ic)
        {
            if (s == "#") return new HashTag() { divisor = true };
            int p = s.IndexOf('#');

            if (p < 1) return null;
            string prefix = s.Substring(0, p).ToLower();
            string leftover = p < s.Length - 1 ? s.Substring(p + 1) : "";

            int e = After(prefix, '%', leftover, out prefix);
            bool isPortion = false;
            float portion = 100;
            if (e == Error) return null;
            else if (e >= 0) { isPortion = true; leftover = ""; portion = e; }

            int rt = -1;
            InfoReference.RefProperty refProp = InfoReference.RefProperty.None;
            if (prefix.Length > 0)
                if (prefix[^1] == ':')
                {
                    e = 0;
                    refProp = InfoReference.RefProperty.Time;
                }
            int refIndex = -1;
            if (e < 0)
                for (int i = 0; i < refProps.Length; i++)
                {
                    e = After(prefix, refProps[i], leftover, out prefix);
                    if (e == Error) return null;
                    else if (e >= 0)
                    {
                        refIndex = e;
                        isPortion = true;
                        leftover = "";
                        rt = i;
                        refProp = rt switch
                        {
                            0 => InfoReference.RefProperty.Total,
                            1 => InfoReference.RefProperty.Image,
                            2 => InfoReference.RefProperty.Name,
                            _ => InfoReference.RefProperty.Value,
                        };
                        break;
                    }
                }

            int fontSize = -1;
            int fi = -1;
            int cols = 1;
            bool dotted = false;
            for (int i = 0; i < duals.Length; i++)
                if (prefix.Length >= 2)
                {
                    int d = Dual(prefix, duals[i], out prefix, duals[i]=='z');
                    if (d == Error) return null;
                    else if (d == Repeated)
                        switch (duals[i])
                        {
                            case 'z': fontSize = BaseSize; break;
                            case 'f': fi = 0; break;
                            default: return null;
                        }
                    else if (d >= 0)
                        switch (duals[i])
                        {
                            case 'z': fontSize = d; break;
                            case 'f':
                                if (d < ic.marker.font.Length)
                                    if (ic.marker.font[d] != null)
                                        fi = d + 1; break;
                            case '.':
                            case '|':
                                if (d > 1)
                                {
                                    cols = d;
                                    dotted = duals[i] == '.';
                                }
                                break;
                        }
                }
            bool valid = true;
            if (refIndex >= 0)
                switch (refProp)
                {
                    case InfoReference.RefProperty.Image:
                        valid = refIndex < ic.marker.inLineImages.Length;
                        break;
                    case InfoReference.RefProperty.Time:
                        valid = true;
                        break;
                    case InfoReference.RefProperty.Value:
                    case InfoReference.RefProperty.Total:
                    case InfoReference.RefProperty.Name:
                        valid = refIndex < ic.marker.references.Length;
                        break;
                }

            if (!valid) return null;

           string colorIndex = "";
            int alignIndex = -1, styleIndex = 0, si;


            for (int i = 0; i < prefix.Length; i++)
            {
                if ((si = aligns.IndexOf(prefix[i])) >= 0) alignIndex = si;
                else if ((si = styles.IndexOf(prefix[i])) >= 0) styleIndex |= 1 << si;
                else if ("0123456789".IndexOf(prefix[i]) >= 0) colorIndex += prefix[i];
                else if (prefix[i] != ',') return null;
            }

            int color = -1;
            if (colorIndex.Length > 0)
            {
                color = int.Parse(colorIndex);
                if (color > 0)
                    if (color - 1 >= ic.marker.colorSetting.others.Length) color = -1;
            }

            bool aligned = false;
            TextAlignmentOptions alignment = TextAlignmentOptions.Center;
            if (alignIndex >= 0)
            {
                aligned = true;
                alignment = alignIndex switch
                {
                    0 => TextAlignmentOptions.Center,
                    1 => TextAlignmentOptions.Justified,
                    2 => TextAlignmentOptions.Left,
                    _ => TextAlignmentOptions.Right,
                };
            }
            FontStyles style = FontStyles.Normal;
            bool added = false;
            int context = 0;
            bool normal = false;
            if (styleIndex > 0)
            {
                style = FontStyles.Normal;
                added = true;
                if ((styleIndex & 32) > 0) context = -1;
                if ((styleIndex & 16) > 0) context = 1;
                if ((styleIndex & 1) > 0) normal = true;
                if ((styleIndex & 2) > 0) style = FontStyles.Italic;
                if ((styleIndex & 4) > 0) style |= FontStyles.Underline;
                if ((styleIndex & 8) > 0) style |= FontStyles.Bold;
            }

            return new HashTag()
            {
                added = added,
                alignment = alignment,
                context = context,
                normal = normal,
                refIndex = refIndex,
                refProp = refProp,
                leftover = leftover,
                style = style,
                color = color,
                aligned = aligned,
                fontSize = fontSize,
                portion = portion / 100,
                dotted = dotted,
                columns = cols,
                font = fi
            };

        }
    }
    public enum IndentType { None, Numeric, Bullet, Dash, Alpha }
    public class Indent
    {
        public IndentType type = IndentType.None;
        public int count = 0;
        public static Indent Get(string s)
        {
            int count = -1;
            IndentType type = IndentType.None;
            int p = s.IndexOf('>');
            if (p < 0) return null;
            for (int i = p; i < s.Length; i++)
                if (s[i] == '>') count++;
                else return null;

            if (p == 0) type = IndentType.None;
            else if (p == 1)
            {
                p = "#*-@".IndexOf(s[0]);
                if (p < 0) return null;
                else type = p switch
                {
                    0 => IndentType.Numeric,
                    1 => IndentType.Bullet,
                    2 => IndentType.Dash,
                    _ => IndentType.Alpha,
                };
            }

            return new Indent() { count = count, type = type };
        }
    }
    public enum ChoiceType { Single, Multiple, Progress, End }
    public class Choice
    {
        public bool valid = false;
        public int stops = 1;
        public ChoiceType type;
        public char key = '1';
        public static Choice Get(string s)
        {
            if (s == "!!") return new Choice() { type = ChoiceType.End };
            if (s.IndexOf("!") == 0)
            {
                 switch (s.Length)
                {
                    case 2:
                        if (s[1] == '-') return new Choice() { type = ChoiceType.Progress, stops = 10 };
                        else return new Choice() { type = ChoiceType.Single, key = s[1] };
                    case 3:
                        if (s[1] == '-')
                            if (int.TryParse(s[2] + "", out int st))
                            {
                                 return new Choice() { type = ChoiceType.Progress, stops = st };
                            }
                        break;
                }
                if (s.IndexOf("!!") == 0)
                    if (s.Length == 3) return new Choice() { type = ChoiceType.Multiple, key = s[1] };

            }
            return null;
        }
    }
    public class InfoScreen
    {
        public static int width { get { return CoreTame.VRMode ? CoreTame.VRSize.x : UnityEngine.Screen.width; } }
        public static int height { get { return CoreTame.VRMode ? CoreTame.VRSize.y : UnityEngine.Screen.height; } }

    }
    public class InfoChoice
    {
        // public TextMeshProUGUI text;
        public Tames.TameInputControl control;
        public string text;
        public int from, to = -1;
        public bool selected = false, multiple = false, hoovered = false;
        public RectTransform[] slider = null;
        public int value = 0;
        public int stops = 0;
        public List<int> indexes = new List<int>();
    }
    public enum WordOperation { Text, Indent, Alignment, NewLine, Reference, Table, Dots, Next }
    public class WordRef
    {
        public string text = "";
        //      public InfoReference.RefType type = InfoReference.RefType.Element;
        public InfoReference.RefProperty prop = InfoReference.RefProperty.None;
        public FontStyles style = 0;
        public WordOperation operation = WordOperation.Text;
        public TextAlignmentOptions alignment = TextAlignmentOptions.Left;

        public int index = -1;
        public IndentType bullet = IndentType.None;
        public Color color;
        public Texture texture = null;
        public bool spaceBefore = true;
        public int size = HashTag.BaseSize;
        public int line = 0;
        public int slider = -1;
        public int font = -1;
        public bool NonText { get { return operation != WordOperation.Text && operation != WordOperation.Reference; } }
        public void Set(WordStyle ws)
        {
            size = ws.size;
            font = ws.font;
            color = ws.color;
            style = ws.style;
        }
    }
    public class LineRef
    {
        public int y = 0, i = 0, indent = 0, col = 0, line = 0, count = 1, bulletIndex = 0;
        public float x, colOffset, textOffset, bulletOffset, width, max;
        public bool nextLine, created, nextCol, dotted = false;
        public TextAlignmentOptions subAlign = TextAlignmentOptions.Left;
        public string bullet = "";
        public void Init(TextAlignmentOptions tao, float total, float portion, int div, float height)
        {
            float lineMax = portion * total;
            float offset = tao switch
            {
                TextAlignmentOptions.Left => 0,
                TextAlignmentOptions.Right => total - lineMax,
                TextAlignmentOptions.Justified => 0,
                _ => (total - lineMax) / 2
            };
            x = offset;
            colOffset = 0;
            bulletOffset = height * indent;
            textOffset = bulletOffset;
            textOffset += bullet == "" ? 0 : height;
            max = lineMax / div;
            width = 0;
            col = 0;
            nextLine = false;
            created = false;
            nextCol = false;
        }
        public bool NextCol()
        {
            col++;
            colOffset += max;
            width = 0;
            subAlign = col < count - 1 ? TextAlignmentOptions.Center : TextAlignmentOptions.Right;
            return col < count;
        }
        public void NewLine(bool manual)
        {
            y++;
            nextCol = false;
            nextLine = false;
            if (manual)
            {
                subAlign = TextAlignmentOptions.Left;
                line++;
                indent = 0;
                created = false;
                dotted = false;
                bullet = "";
                count = 1;
            }
        }
    }
    public class IndentOrder
    {
        public IndentType[] orderType;
        public int[] orderIndex;
        public int current;
        public IndentOrder()
        {
            orderType = new IndentType[10];
            orderIndex = new int[10];
            for (int i = 0; i < orderType.Length; i++)
            {
                orderType[i] = IndentType.None;
                orderIndex[i] = 0;
            }
            current = 0;
        }
        public void CurrentLine(int index, IndentType type)
        {
            if (index < current)
            {
                for (int i = index + 1; i < orderType.Length; i++)
                {
                    orderType[i] = IndentType.None;
                    orderIndex[i] = 0;
                }
                current = index;
                orderIndex[current]++;
            }
            else if (index == current)
            {
                if (type == orderType[current])
                    orderIndex[current]++;
                else
                {
                    orderType[current] = type;
                    orderIndex[current] = 1;
                }
            }
            else
            {
                current = index;
                orderIndex[current] = 1;
                orderType[current] = type;
            }
        }
        public string Bullet()
        {
            return orderType[current] switch
            {
                IndentType.Dash => "-",
                IndentType.Bullet => "●",
                IndentType.Alpha => " abcdefghijklmnopqrstuvwxyz"[orderIndex[current] - 1] + ".",
                IndentType.Numeric => orderIndex[current] + ".",
                _ => ""
            };
        }
    }
    public class WordStyle
    {
        public Color color;
        public int size = HashTag.BaseSize;
        public int font = -1;
        public FontStyles style = FontStyles.Normal;
        public void Get(WordStyle ws)
        {
            color = ws.color;
            size = ws.size;
            font = ws.font;
            style = ws.style;
        }
        public void Set(HashTag hash, Markers.QMarker marker)
        {
            if (hash.normal) style = 0;
            else if (hash.context == 0) style = hash.style;
            else if (hash.context == 1) style |= hash.style;
            else
            {
                if ((hash.style & FontStyles.Bold) > 0) style &= FontStyles.Italic | FontStyles.Underline;
                if ((hash.style & FontStyles.Italic) > 0) style &= FontStyles.Bold | FontStyles.Underline;
                if ((hash.style & FontStyles.Underline) > 0) style &= FontStyles.Italic | FontStyles.Bold;
            }
            if (hash.color >= 0) color = marker.colorSetting.Get(hash.color);
            if (hash.fontSize >= 0) size = hash.fontSize;
            if (hash.font >= 0)
                if (hash.font <= marker.font.Length)
                    font = hash.font;
        }
    }
}
