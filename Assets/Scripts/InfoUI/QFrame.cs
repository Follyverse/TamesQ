using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Markers;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace InfoUI
{
    public class QFrame : Tames.TameThing
    {
        public static List<Sprite> sprites = new List<Sprite>();

        public InfoControl parent;
        public Rect outer, inner, main;
        Rect[] context, lines;
        public QMarker marker;
        public Material material;
        public int index;
        public float lineHeight = -1;
        Canvas canvas = null;
        public GameObject owner;
        RectTransform rectTransform;
        float margin, backMargin, textHeight, factor = 1;
        Vector3 objectBound;
        public enum ItemType { TextOnly, Image, Object }
        public ItemType type = ItemType.TextOnly;
        Vector2[] panelSize = new Vector2[10];
        Vector2[] panelPos = new Vector2[10];
        GameObject instance;
        List<RectTransform> children = new();
        //List<Vector2> Ys = new List<Vector2>();
        List<Vector3> childCoord = new List<Vector3>();
        List<int> childTypes = new List<int>();
        List<WordRef> words = new List<WordRef>();
        TextMeshProUGUI[] text = null;
        Image[] textBack;
        const int TypePanel = -1;
        const int TypeImage = -2;
        int validWordCount = 0;
        List<IndexCounter> sections = new List<IndexCounter>();
        public InfoItem item;
        public int linesCount = 0;
        public List<InfoChoice> choice = new List<InfoChoice>();
        public bool justImage = false;
        public RawImage imageItem;
        public QFrame parentFrame = null;
        public int hooverIndex = -1;
        public float lastChanged = 0;
        public bool changeRecorded = false;
        private InfoPosition mpp;
        private float[] portions;
        public override int CurrentIndex()
        {
            if (choice.Count >= 0)
                for (int i = 0; i < choice.Count; i++)
                    if (choice[i].selected) return i + 1;
            return 0;
        }

        public void Reset()
        {
            choice.Clear();
            lastAlignment = TextAlignmentOptions.Left;
            lastFont = 0;
            words.Clear();
            childCoord.Clear();
            childTypes.Clear();
            sections.Clear();
            if (text != null)
                for (int i = 0; i < text.Length; i++)
                    if (text[i] != null)
                        GameObject.Destroy(text[i].gameObject);

            foreach (RectTransform rect in children)
                if (rect != null)
                    GameObject.Destroy(rect.gameObject);
            children.Clear();
            if (owner != null)
                GameObject.Destroy(owner);
            if (instance != null) GameObject.Destroy(instance);
            lineHeight = -1;
        }
        public void Initialize(float height)
        {
            mpp = marker.position.position.position;
            if (marker.colorSetting.image == null)
                backMargin = 0;
            else
                backMargin = marker.colorSetting.margin.type switch
                {
                    MarginType.Pixels => marker.colorSetting.margin.margin / marker.colorSetting.image.width,
                    MarginType.Width => marker.colorSetting.margin.margin,
                    _ => marker.colorSetting.margin.margin * marker.colorSetting.image.height / marker.colorSetting.image.width,
                };
              if (height < 0)
            {
                if (mpp == InfoPosition.WithObject)
                {
                    main = outer = new Rect(0, 0, marker.position.Width, marker.position.Height);
                    inner = new Rect(marker.position.margin, marker.position.margin, marker.position.Width - marker.position.margin * 2, marker.position.Height - marker.position.margin * 2);
                    margin = marker.position.margin;
                }
                else
                {
                    main = outer = new Rect(0, 0, InfoScreen.width, InfoScreen.height);
                    margin = InfoScreen.height * marker.position.margin;
                    MoveRects();
                    inner = new Rect(margin + outer.xMin, margin + outer.yMin, outer.width - margin * 2, outer.height - margin * 2);
                }
                context = ItemRect();
                lines = TextRectangles();
                //    lineHeight = lines[0].height;
                //       Debug.Log("base: " + marker.name + " " + outer.height);
            }
            else
            {
                float h;
                if (mpp == InfoPosition.WithObject)
                {
                    margin = marker.position.margin;
                    h = item.lineCount * height + margin * 2;
                    main = outer = new Rect(0, 0, marker.position.Width, h);
                    inner = new Rect(marker.position.margin, marker.position.margin, marker.position.Width - marker.position.margin * 2, h - marker.position.margin * 2);
                }
                else
                {
                    margin = InfoScreen.height * marker.position.margin;
                    h = item.lineCount * height + margin * 2;
                    main = outer = new Rect(0, 0, InfoScreen.width, InfoScreen.height);
                    MoveRects(h);
                    inner = new Rect(margin + outer.xMin, margin + outer.yMin, outer.width - margin * 2, outer.height - margin * 2);
                }
                lineHeight = height;
                context = ItemRect();
                lines = TextRectangles();
                //        Debug.Log("next: " + marker.name + " " + outer.height);

            }
            CreateCanvas();
            Panelize();
            if (mpp == InfoPosition.WithObject)
                AdjustWithObject();
            else
                AdjustOnScreen();
            ReadText(item.Text);
            //CreateText();
            AddTexts();
        }


        void AddChild(RectTransform child, int type, Vector3 coord)
        {
            children.Add(child);
            childCoord.Add(coord);
            childTypes.Add(type);
        }




        void MoveRects()
        {
            Vector2 s = new Vector2(InfoScreen.width, InfoScreen.height);
            Vector2 m = new Vector2(marker.position.Width, marker.position.Height);
            m = Vector2.Scale(m, s);
            Vector2 d = s - m;
            //    Debug.Log(marker.position);
            switch (mpp)
            {
                case InfoPosition.Top: outer = new Rect(d.x / 2, 0, m.x, m.y); break;
                case InfoPosition.TopLeft: outer = new Rect(0, 0, m.x, m.y); break;
                case InfoPosition.TopRight: outer = new Rect(d.x, 0, m.x, m.y); break;
                case InfoPosition.Bottom: outer = new Rect(d.x / 2, d.y, m.x, m.y); break;
                case InfoPosition.BottomLeft: outer = new Rect(0, d.y, m.x, m.y); break;
                case InfoPosition.BottomRight: outer = new Rect(d.x, d.y, m.x, m.y); break;
                case InfoPosition.Left: outer = new Rect(0, d.y / 2, m.x, m.y); break;
                case InfoPosition.Right: outer = new Rect(d.x, d.y / 2, m.x, m.y); break;
                case InfoPosition.OnObject: outer = new Rect(0, 0, m.x, m.y); break;
            }
            bool repPrev = false;
            if (index != 0)
            {
                QFrame prev = parent.frames[index - 1];
                switch (item.replace)
                {
                    case InfoOrder.ReplacePrevious:
                        repPrev = index > 0;
                        break;
                    case InfoOrder.ReplaceAll:
                        break;
                    case InfoOrder.AddVertical:
                        switch (mpp)
                        {
                            case InfoPosition.Top:
                            case InfoPosition.TopLeft:
                            case InfoPosition.TopRight:
                            case InfoPosition.OnObject: outer.y = prev.outer.yMax + margin; break;
                            case InfoPosition.Bottom:
                            case InfoPosition.BottomLeft:
                            case InfoPosition.BottomRight: outer.y = prev.outer.yMin - margin - outer.height; break;
                            default: repPrev = index > 0; break;
                        }
                        break;
                    case InfoOrder.AddHorizontal:
                        switch (mpp)
                        {
                            case InfoPosition.Left:
                            case InfoPosition.TopLeft:
                            case InfoPosition.BottomLeft:
                            case InfoPosition.OnObject: outer.x = prev.outer.xMax + margin; break;
                            case InfoPosition.Right:
                            case InfoPosition.TopRight:
                            case InfoPosition.BottomRight: outer.x = prev.outer.xMin - margin - outer.width; break;
                            default: repPrev = index > 0; break;
                        }
                        break;
                }

                if (repPrev)
                {
                    outer.y = prev.outer.yMin;
                    outer.x = prev.outer.xMin;
                }
            }
        }
        void MoveRects(float h)
        {
            Vector2 s = new Vector2(InfoScreen.width, InfoScreen.height);
            Vector2 m = new Vector2(marker.position.Width * InfoScreen.width, h);
            //    m = Vector2.Scale(m, s);
            Vector2 d = s - m;
            //    Debug.Log(marker.position);
            switch (mpp)
            {
                case InfoPosition.Top: outer = new Rect(d.x / 2, 0, m.x, m.y); break;
                case InfoPosition.TopLeft: outer = new Rect(0, 0, m.x, m.y); break;
                case InfoPosition.TopRight: outer = new Rect(d.x, 0, m.x, m.y); break;
                case InfoPosition.Bottom: outer = new Rect(d.x / 2, d.y, m.x, m.y); break;
                case InfoPosition.BottomLeft: outer = new Rect(0, d.y, m.x, m.y); break;
                case InfoPosition.BottomRight: outer = new Rect(d.x, d.y, m.x, m.y); break;
                case InfoPosition.Left: outer = new Rect(0, d.y / 2, m.x, m.y); break;
                case InfoPosition.Right: outer = new Rect(d.x, d.y / 2, m.x, m.y); break;
                case InfoPosition.OnObject: outer = new Rect(0, 0, m.x, m.y); break;
            }
            bool repPrev = false;
            if (index != 0)
            {
                QFrame prev = parent.frames[index - 1];
                switch (item.replace)
                {
                    case InfoOrder.ReplacePrevious:
                        repPrev = index > 0;
                        break;
                    case InfoOrder.ReplaceAll:
                        break;
                    case InfoOrder.AddVertical:
                        switch (mpp)
                        {
                            case InfoPosition.Top:
                            case InfoPosition.TopLeft:
                            case InfoPosition.TopRight:
                            case InfoPosition.OnObject: outer.y = prev.outer.yMax + margin; break;
                            case InfoPosition.Bottom:
                            case InfoPosition.BottomLeft:
                            case InfoPosition.BottomRight: outer.y = prev.outer.yMin - margin - outer.height; break;
                            default: repPrev = index > 0; break;
                        }
                        break;
                    case InfoOrder.AddHorizontal:
                        switch (mpp)
                        {
                            case InfoPosition.Left:
                            case InfoPosition.TopLeft:
                            case InfoPosition.BottomLeft:
                            case InfoPosition.OnObject: outer.x = prev.outer.xMax + margin; break;
                            case InfoPosition.Right:
                            case InfoPosition.TopRight:
                            case InfoPosition.BottomRight: outer.x = prev.outer.xMin - margin - outer.width; break;
                            default: repPrev = index > 0; break;
                        }
                        break;
                }

                if (repPrev)
                {
                    outer.y = prev.outer.yMin;
                    outer.x = prev.outer.xMin;
                }
            }
        }

        public int GetReplace()
        {
            return item.replace switch
            {
                InfoOrder.ReplacePrevious => 1,
                InfoOrder.ReplaceAll => 2,
                _ => 0,
            };
        }
        public void MoveTo(float x, float y, float Ox, float Oy)
        {
            if (justImage)
            {
                parentFrame.MoveTo(x, y, Ox, Oy);
                return;
            }
            for (int i = 0; i < children.Count; i++)
                children[i].anchoredPosition = new Vector2(x + childCoord[i].x - Ox, outer.height - childCoord[i].z - (childCoord[i].y - Oy) + y);
        }

        Rect[] ItemRect()
        {
            float tr = 0.7f;
            float ir = 1 - tr;
            float frameRatio = inner.width / inner.height;
            float expected = frameRatio * ir;
            float itemRatio;
            if (item.image == null)
                return new Rect[] { inner };
            else
            {
                type = ItemType.Image;
                itemRatio = item.image.width / (float)item.image.height;
            }
            //  Debug.Log(itemRatio + " " + type);
            int added = 1;
            float w, h;
            float x = inner.xMin, y = inner.yMin;
            if ((marker.vertical == Vertical.Stretch) && (marker.horizontal == Horizontal.Stretch))
                return new Rect[] { inner, inner };
            else if (marker.vertical == Vertical.Stretch)
            {
                if (expected < itemRatio)
                {
                    w = inner.width * ir;
                    h = w / itemRatio;
                    y += ((marker.position.onlyForText ? outer.height : inner.height) - h) / 2;
                    if (marker.horizontal == Horizontal.Right)
                        x += inner.width * tr;
                }
                else
                {
                    h = marker.position.onlyForText ? outer.height : inner.height;
                    w = h * itemRatio;
                    if (marker.horizontal == Horizontal.Right)
                        x += inner.width - w;
                }
            }
            else if (marker.horizontal == Horizontal.Stretch)
            {
                if (expected < itemRatio)
                {
                    w = inner.width;
                    h = w / itemRatio;
                    if (marker.vertical == Vertical.Bottom)
                        y += inner.height - h;
                }
                else
                {
                    h = inner.height / 2;
                    w = h * itemRatio;
                    x += (inner.width - w) / 2;
                    if (marker.vertical == Vertical.Bottom)
                        y += inner.height - h;
                }
            }
            else
            {
                added = 2;
                if (frameRatio < itemRatio)
                {
                    w = inner.width * ir;
                    h = w / itemRatio;
                }
                else
                {
                    h = inner.height * ir;
                    w = h * itemRatio;
                }
                if ((marker.vertical == Vertical.Top) && (marker.horizontal == Horizontal.Right)) x += inner.width - w;
                else if ((marker.vertical == Vertical.Bottom) && (marker.horizontal == Horizontal.Left)) y += inner.width - h;
                else if ((marker.vertical == Vertical.Bottom) && (marker.horizontal == Horizontal.Right))
                {
                    x += inner.width - w; y += inner.width - h;
                }
            }



            Rect[] r = new Rect[added + 1];
            r[0] = new Rect(x, y, w, h);
            if (added == 1)
            {
                x = marker.horizontal == Horizontal.Left ? r[0].xMax + margin : inner.xMin;
                y = marker.vertical == Vertical.Top ? r[0].yMax : inner.yMin;
                w = marker.horizontal == Horizontal.Stretch ? inner.width - margin : inner.width - w - margin;
                h = marker.vertical == Vertical.Stretch ? inner.height : inner.height - h;
                r[1] = new Rect(x, y, w, h);
            }
            else
            {
                if (marker.horizontal == Horizontal.Left)
                {
                    if (marker.vertical == Vertical.Top)
                    {
                        r[1] = new Rect(r[0].xMax + margin, inner.yMin, inner.width - w - margin, h);
                        r[2] = new Rect(inner.xMin, r[0].yMax, inner.width, inner.height - h);
                    }
                    else
                    {
                        r[1] = new Rect(inner.xMin, inner.yMin, inner.width, inner.height - h);
                        r[2] = new Rect(r[0].xMax + margin, r[0].yMin, inner.width - w - margin, h);
                    }
                }
                else
                {
                    if (marker.vertical == Vertical.Top)
                    {
                        r[1] = new Rect(inner.xMin, inner.yMin, inner.width - w - margin, h);
                        r[2] = new Rect(inner.xMin, r[0].yMax, inner.width, inner.height - h);
                    }
                    else
                    {
                        r[1] = new Rect(inner.xMin, inner.yMin, inner.width, inner.height - h);
                        r[2] = new Rect(inner.xMin, r[0].yMin, inner.width - w - margin, h);
                    }
                }
            }
            return r;
        }
        void PanelizeBorderWithObject()
        {
            GameObject go;
            RectTransform rect;
            Image image;
            Rect C = context[0];
            Vector2 p;
            float w = outer.width, h = outer.height;
            float margin = outer.width * backMargin;
            // top left
            panelSize[0] = new(margin, margin);
            panelSize[1] = new(w - 2 * margin, margin);
            panelSize[2] = new(margin, margin);
            panelSize[3] = new(margin, h - 2 * margin);
            panelSize[4] = new(margin, margin);
            panelSize[5] = new(w - 2 * margin, margin);
            panelSize[6] = new(margin, margin);
            panelSize[7] = new(margin, h - 2 * margin);

            panelPos[0] = new(0, 0);
            panelPos[1] = new(margin, 0);
            panelPos[2] = new(w - margin, 0);
            panelPos[3] = new(0, margin);
            panelPos[4] = new(0, h - margin);
            panelPos[5] = new(margin, h - margin);
            panelPos[6] = new(w - margin, h - margin);
            panelPos[7] = new(w - margin, margin);

            int m = 0;
            Rect[] iRect = new Rect[8];
            if (marker.colorSetting.image != null)
            {
                w = marker.colorSetting.image.width;
                h = marker.colorSetting.image.height;
                m = (int)(backMargin * marker.colorSetting.image.width);
                 iRect[4] = new(0, 0, m, m);
                iRect[5] = new(m, 0, w - m * 2, m);
                iRect[6] = new(w - m, 0, m, m);
                iRect[3] = new(0, m, m, h - 2 * m);
                iRect[0] = new(0, h - m, m, m);
                iRect[1] = new(m, h - m, w - 2 * m, m);
                iRect[2] = new(w - m, h - m, m, m);
                iRect[7] = new(w - m, m, m, h - 2 * m);
                //      Debug.Log("margin is " + m + " " + margin);
            }
            Vector3 cc;
            for (int i = 0; i < 8; i++)
            {
                panelPos[i] += outer.min;
                cc = new(panelPos[i].x, panelPos[i].y, panelSize[i].y);
                panelPos[i].y = main.height - panelPos[i].y - panelSize[i].y;
                go = new GameObject("border " + i);
                image = go.AddComponent<Image>();
                rect = go.GetComponent<RectTransform>();
                rect.parent = rectTransform;
                rect.sizeDelta = panelSize[i];
                rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
                rect.anchoredPosition = panelPos[i];
                if (marker.colorSetting.image == null)
                    image.color = marker.colorSetting.background;
                else
                {
                    image.sprite = Sprite.Create((Texture2D)marker.colorSetting.image, iRect[i], Vector2.zero);
                    sprites.Add(image.sprite);
                }
                AddChild(rect, TypePanel, cc);
            }
        }
        void PanelizeBorder()
        {
            if (mpp == InfoPosition.WithObject)
            {
                PanelizeBorderWithObject();
                return;
            }
            GameObject go;
            RectTransform rect;
            Image image;
            Rect C = context[0];
            Vector2 p;
            int w = (int)outer.width, h = (int)outer.height;
            float margin = outer.width * backMargin;

            // top left
            panelSize[0] = new(margin, margin);
            panelSize[1] = new(w - 2 * margin, margin);
            panelSize[2] = new(margin, margin);
            panelSize[3] = new(margin, h - 2 * margin);
            panelSize[4] = new(margin, margin);
            panelSize[5] = new(w - 2 * margin, margin);
            panelSize[6] = new(margin, margin);
            panelSize[7] = new(margin, h - 2 * margin);

            panelPos[0] = new(0, 0);
            panelPos[1] = new(margin, 0);
            panelPos[2] = new(w - margin, 0);
            panelPos[3] = new(0, margin);
            panelPos[4] = new(0, h - margin);
            panelPos[5] = new(margin, h - margin);
            panelPos[6] = new(w - margin, h - margin);
            panelPos[7] = new(w - margin, margin);

            int m = 0;
            Rect[] iRect = new Rect[8];
            if (marker.colorSetting.image != null)
            {
                w = marker.colorSetting.image.width;
                h = marker.colorSetting.image.height;
                m = (int)(backMargin * marker.colorSetting.image.width);
                 iRect[4] = new(0, 0, m, m);
                iRect[5] = new(m, 0, w - m * 2, m);
                iRect[6] = new(w - m, 0, m, m);
                iRect[3] = new(0, m, m, h - 2 * m);
                iRect[0] = new(0, h - m, m, m);
                iRect[1] = new(m, h - m, w - 2 * m, m);
                iRect[2] = new(w - m, h - m, m, m);
                iRect[7] = new(w - m, m, m, h - 2 * m);
                //           Debug.Log("margin is " + m + " " + margin);
            }
            Vector3 cc;
            for (int i = 0; i < 8; i++)
            {
                panelPos[i] += outer.min;
                cc = new(panelPos[i].x, panelPos[i].y, panelSize[i].y);
                panelPos[i].y = main.height - panelPos[i].y - panelSize[i].y;
                go = new GameObject("border " + i);
                image = go.AddComponent<Image>();
                rect = go.GetComponent<RectTransform>();
                rect.parent = rectTransform;
                rect.sizeDelta = panelSize[i];
                rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
                rect.anchoredPosition = panelPos[i];
                if (marker.colorSetting.image == null)
                    image.color = marker.colorSetting.background;
                else
                {
                    image.sprite = Sprite.Create((Texture2D)marker.colorSetting.image, iRect[i], Vector2.zero);
                    sprites.Add(image.sprite);
                }
                AddChild(rect, TypePanel, cc);
            }
        }

        void Panelize()
        {
            GameObject go;
            RectTransform rect;
            RawImage image;
            Image back;

            if (marker.colorSetting.image != null)
                PanelizeBorder();
            go = new GameObject("area");
            back = go.AddComponent<Image>();
            if (marker.colorSetting.image == null)
                back.color = marker.colorSetting.background;
            else
            {
                int w = marker.colorSetting.image.width;
                int h = marker.colorSetting.image.height;
                int m = (int)(backMargin * marker.colorSetting.image.width);
                back.sprite = Sprite.Create((Texture2D)marker.colorSetting.image, new Rect(m, m, w - 2 * m, h - 2 * m), Vector2.zero);
                sprites.Add(back.sprite);
            }
            rect = go.GetComponent<RectTransform>();
            rect.parent = rectTransform;
            float border = backMargin * outer.width;
            Rect inner = new Rect(outer.xMin + border, outer.yMin + border, outer.width - 2 * border, outer.height - 2 * border);
            Rect R = marker.colorSetting.image != null ? inner : outer;
            rect.sizeDelta = R.size;
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
            rect.anchoredPosition = new Vector2(R.xMin, main.height - R.yMin - R.height);
            AddChild(rect, TypePanel, new(R.xMin, R.yMin, R.height));
            if (item.image != null)
            {
                go = new GameObject("image");
                image = go.AddComponent<RawImage>();
                image.texture = item.image;
                imageItem = image;
                rect = go.GetComponent<RectTransform>();
                rect.parent = rectTransform;
                rect.sizeDelta = context[0].size;
                rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
                rect.anchoredPosition = new Vector2(context[0].x, main.height - context[0].yMin - context[0].height);
                AddChild(rect, TypeImage, new(context[0].xMin, context[0].yMin, context[0].height));
            }

        }
        Rect[] TextRectangles()
        {
            float offsetY, offsetX, endX;
            Rect[] line;
            int cTop, cBottom;
            float y;
            int count = item.lineCount;
            float height = lineHeight < 0 ? inner.height / count : lineHeight;
            if (lineHeight < 0) lineHeight = height;
            Rect T;
            //      if (marker.name == "savoye") Debug.Log("savo " + context.Length);
            if (context.Length < 3)
            {
                T = context.Length == 1 ? context[0] : context[1];
                if (inner.height > T.height)
                    count = (int)(T.height / height);
                //  if (marker.name == "savoye") Debug.Log("savo " + count + " " + T.height + " " + inner.height);
                //  height = T.height / count;
                offsetY = height / 4f;
                offsetX = (context.Length == 2 && marker.horizontal == Horizontal.Left) ? height / 4 : 0;
                endX = (context.Length == 2 && marker.horizontal == Horizontal.Right) ? height / 4 : 0;
                textHeight = height - offsetY * 2;
                line = new Rect[count];
                for (int i = 0; i < count; i++)
                    line[i] = new Rect(T.xMin + offsetX, T.yMin + height * i + offsetY, T.width - endX, textHeight);
            }
            else
            {
                //      count = (int)(inner.height / height);
                //        height = inner.height / count;
                cTop = (int)(context[1].height / height);
                cBottom = (int)(context[2].height / height);
                offsetY = height / 6f;
                y = context[2].y - cTop * height + offsetY;
                textHeight = height - offsetY * 2;
                line = new Rect[cBottom + cTop];
                count = cBottom + cTop;
                int j;
                for (int i = 0; i < count; i++)
                {
                    if ((i < cTop && marker.vertical == Vertical.Top) || (i >= cTop && marker.vertical == Vertical.Bottom))
                        offsetX = marker.horizontal == Horizontal.Left ? height / 4 : 0;
                    else offsetX = 0;
                    if ((i < cTop && marker.vertical == Vertical.Top) || (i >= cTop && marker.vertical == Vertical.Bottom))
                        endX = marker.horizontal == Horizontal.Right ? height / 4 : 0;
                    else endX = 0;
                    j = i < cTop ? 1 : 2;
                    line[i] = new Rect(context[j].x + offsetX, y + height * i + offsetY, context[j].width - endX, textHeight);
                }
            }
            return line;
        }

        void CreateCanvas()
        {
            owner = new GameObject("canvas");
            canvas = owner.AddComponent<Canvas>();
            if ((rectTransform = owner.GetComponent<RectTransform>()) == null) rectTransform = owner.AddComponent<RectTransform>();
            rectTransform.parent = parent.parent.transform;
            if (mpp != InfoPosition.WithObject) owner.layer = 5; // ui
            owner.SetActive(false);
            //      Debug.Log("world " + main.size.ToString() + outer.size.ToString());
        }
        void SetTransform()
        {
            Vector3 right, up;
            bool shiftX = false, shiftY = false;
            switch (marker.position.position.X)
            {
                case PosAxis.X: right = marker.transform.right; break;
                case PosAxis.nX: right = -marker.transform.right; shiftX = false; break;
                case PosAxis.Y: right = marker.transform.up; break;
                case PosAxis.nY: right = -marker.transform.up; shiftX = false; break;
                case PosAxis.Z: right = marker.transform.up; break;
                default: right = -marker.transform.up; shiftX = false; break;
            }
            switch (marker.position.position.Y)
            {
                case PosAxis.X: up = marker.transform.right; break;
                case PosAxis.nX: up = -marker.transform.right; shiftY = false; break;
                case PosAxis.Y: up = marker.transform.up; break;
                case PosAxis.nY: up = -marker.transform.up; shiftY = false; break;
                case PosAxis.Z: up = marker.transform.forward; break;
                default: up = -marker.transform.forward; shiftY = false; break;
            }
            Vector3 fwd = Vector3.Cross(right, up);
            Vector3 pos = Vector3.zero;
            if (shiftX) pos.x -= outer.width;
            if (shiftY) pos.y -= outer.height;
            Quaternion rot = Quaternion.LookRotation(fwd, up);
            owner.transform.rotation = rot;
            owner.transform.position = marker.transform.position - pos.x * right - pos.y * up;
        }
        void AdjustWithObject()
        {
            rectTransform.anchoredPosition = Vector2.zero;
            canvas.renderMode = RenderMode.WorldSpace;
            rectTransform.sizeDelta = outer.size;
            //     rectTransform.pivot=
            canvas.worldCamera = Camera.main;
            owner.transform.parent = marker.transform;
            SetTransform();


        }
        void AdjustOnScreen()
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            factor = InfoScreen.height;
            if (mpp == InfoPosition.OnObject)
            {
                //         canvas.renderMode = RenderMode.ScreenSpaceCamera;
                //       owner.transform.parent = marker.transform;
                //        owner.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                //       rectTransform.localPosition = Vector3.zero;
            }
        }

        InfoChoice GetChoice(int i)
        {
            foreach (InfoChoice ch in choice)
                if (ch.from <= i)
                {
                    if (ch.to < 0 || ch.to >= i)
                        return ch;
                }
            return null;
        }
        void CreateNavigs()
        {

        }
        void ReadText(string text)
        {
            text = text.Replace("\r", "");
            string[] lines = text.Split('\n', StringSplitOptions.None);

            //   List<string> words = new List<string>();
            WordStyle globalFormat = new WordStyle() { color = marker.colorSetting.foreground }, localFormat = new WordStyle();
            localFormat.Get(globalFormat);
            InfoChoice currentChoice = null;
            Tames.TameInputControl tci;
            WordRef wr = null;
             portions = new float[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                portions[i] = 1;
            for (int l = 0; l < lines.Length; l++)
            {
                string[] split = lines[l].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Choice ch;
                Indent indent = split.Length > 1 ? Indent.Get(split[0]) : null;
                int start = indent == null ? 0 : 1;
                if (indent != null)
                {
                    if (indent.type != IndentType.None)
                    {
                        words.Add(new WordRef() { bullet = indent.type, index = indent.count, style = localFormat.style, color = localFormat.color, operation = WordOperation.Indent, size = localFormat.size, line = l });
                  
                    }
                }
                  bool lineDivided = false;
                for (int i = start; i < split.Length; i++)
                {
                    HashTag hash = HashTag.Get(split[i], parent);
                    ch = Choice.Get(split[i]);
                    localFormat.Get(globalFormat);
                    if (hash != null)
                    {
                        if (hash.aligned && i == start)
                        {
                              words.Add(wr = new WordRef() { text = "", alignment = hash.alignment, operation = WordOperation.Alignment, line = l });
                        }
                        if (hash.columns > 1 && i == start)
                        {
                            lineDivided = true;
                            words.Add(wr = new WordRef() { text = "", operation = hash.dotted ? WordOperation.Dots : WordOperation.Table, index = hash.columns, line = l });
                        }
                        if (hash.divisor && lineDivided)
                            words.Add(wr = new WordRef() { text = "", operation = WordOperation.Next, line = l });

                        if (hash.portion < 1) portions[l] = hash.portion;
                        if (hash.refProp != InfoReference.RefProperty.None)
                            words.Add(wr = new WordRef() { text = "", prop = hash.refProp, index = hash.refIndex, operation = WordOperation.Reference, line = l });
                        else
                            words.Add(wr = new WordRef() { text = hash.leftover, prop = hash.refProp, line = l });

                        if (hash.leftover.Length == 0 && hash.refProp == InfoReference.RefProperty.None)
                        {
                         globalFormat.Set(hash, parent.marker);
                            localFormat.Get(globalFormat);
                        }
                        else
                        {
                             localFormat.Set(hash, parent.marker);
                        }
                        wr.Set(localFormat);
                    }
                    else if (ch != null)
                    {
                        if (ch.type == ChoiceType.End)
                        {
                            if (currentChoice != null) currentChoice.to = words.Count - 1;
                            currentChoice = null;
                        }
                        else if (ch.type == ChoiceType.Progress)
                        {
                            if (currentChoice != null) currentChoice.to = words.Count - 1;
                            choice.Add(new InfoChoice() { control = null, from = words.Count, to = words.Count, multiple = false, text = "", stops = ch.stops });
                            currentChoice = null;
                            words.Add(wr = new WordRef() { text = "", prop = InfoReference.RefProperty.None, line = l, slider = choice.Count - 1, color = globalFormat.color });
                        }
                        else
                        {
                            tci = Tames.TameInputControl.StringToMono(ch.key + "", Tames.InputTypes.KeyboardMouse, Tames.InputControlHold.None);
                            if (tci != null)
                            {
                                if (currentChoice != null) currentChoice.to = words.Count - 1;
                                choice.Add(currentChoice = new InfoChoice() { control = tci, from = words.Count, multiple = ch.type == ChoiceType.Multiple, text = "" });
                            }
                        }
                    }
                    else
                    {
                        words.Add(wr = new WordRef() { text = split[i] });
                        wr.Set(localFormat);
                    }
                   }
                if (l != lines.Length - 1) words.Add(new WordRef() { operation = WordOperation.NewLine });
                if (currentChoice != null)
                {
                    currentChoice.to = words.Count - 1;
                    currentChoice = null;
                }
            }
            if (choice.Count > 0)
                choice[0].hoovered = true;
          }
        TextAlignmentOptions lastAlignment = TextAlignmentOptions.Left;
        int lastFont = 0;
        Image[] inline;
        void Add(LineRef l)
        {
            int i = l.i;
            int start = i;
            string s = "";
            GameObject go;
            RectTransform rect;
            l.created = false;

            bool existing = text[l.i] != null;
            float spaceCount = text[l.i] == null ? 0 : 1;
            float space = lines[l.y].height / 5;
            float[] widths = new float[words.Count];

            InfoChoice ic;
            Image img;
            Texture t;

            Vector2 preferred;
            int sliderChoice = -1, sliderText = -1;
            while (l.i < words.Count)
            {
                i = l.i;
                if (words[i].NonText) break;
                if (words[i].slider >= 0)
                {
                    sliderChoice = words[i].slider;
                    sliderText = i;
                    l.nextCol = true;
                    l.i++;
                    break;
                }
                s = words[i].text;

                if (words[i].prop == InfoReference.RefProperty.Name)
                    s = parent.references[words[i].index].MaxName();
                else if (words[i].prop != InfoReference.RefProperty.None && words[i].prop != InfoReference.RefProperty.Image)
                    s = words[i].index < 0 ? "888" : "" + parent.references[words[i].index].MaxLength();

                if (words[i].prop == InfoReference.RefProperty.Image)
                {
                    if (inline[i] == null)
                    {
                        t = marker.inLineImages[words[i].index];
                        if (t != null)
                        {
                            go = new GameObject("word " + i);
                            inline[i] = go.AddComponent<Image>();
                            //     Debug.Log("words " + i + " " + (t == null));
                            inline[i].sprite = Sprite.Create((Texture2D)t, new(0, 0, t.width, t.height), Vector2.zero);
                            sprites.Add(inline[i].sprite);
                            words[i].texture = t;
                        }
                    }
                    float size = HashTag.Size(textHeight, words[i].size);
                    spaceCount++;
                    preferred = new Vector2(words[i].texture == null ? size : words[i].texture.width / (float)words[i].texture.height * size, size);
                }
                else
                {
                    if (text[i] == null)
                    {
                        go = new GameObject("word " + i);
                        text[i] = go.AddComponent<TextMeshProUGUI>();

                        text[i].fontSize = HashTag.Size(textHeight, words[i].size);
                        text[i].color = words[i].color;
                        text[i].fontStyle = words[i].style;
                        text[i].text = s;
                        if (words[i].font <= 0)
                            text[i].font = CoreTame.instance.uitext.font;
                        else
                            if (parent.marker.font[words[i].font - 1] != null)
                            text[i].font = parent.marker.font[words[i].font - 1];
                        ic = GetChoice(i);
                        if (ic != null) ic.indexes.Add(i);
                    }
                    spaceCount++;
                    preferred = text[i].GetPreferredValues();
                    if (words[i].prop != InfoReference.RefProperty.None)
                    {
                        if (words[i].prop == InfoReference.RefProperty.Time)
                        {
                            text[i].text = (int)Tames.TameElement.ActiveTime + "";
                            text[i].alignment = TextAlignmentOptions.Right;
                        }
                        else if (parent.references[words[i].index] != null)
                        {
                            text[i].text = parent.references[words[i].index].Get(words[i].prop);
                            text[i].alignment = words[i].prop != InfoReference.RefProperty.Name ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                        }
                    }
                }
                //    Debug.Log("preferred " + preferred.x + " " + s);
                if (l.width + preferred.x + space * (spaceCount - 1) > l.max)
                {
                    l.created = true;
                    if (l.count > 1)
                        l.nextCol = true;
                    else
                    {
                        l.nextLine = true;
                        l.width = preferred.x;
                    }
                    break;
                }
                else
                {
                    l.width += (existing && start == l.i) ? 0 : preferred.x;
                    widths[i] = preferred.x;
                    if (i == words.Count - 1)
                    {
                        l.i++;
                        break;
                    }
                }
                l.i++;
            }

            float x, xBullet;
            TextAlignmentOptions tao = l.dotted ? l.subAlign : lastAlignment;
            if (l.i > start)
            {
                x = GetX(tao, l, spaceCount, space, sliderText >= 0);
                x += inner.xMin + l.x;
                xBullet = x - textHeight;
                for (i = start; i < l.i; i++)
                    if (i == sliderText)
                    {
                        float sw = l.max - l.width - space * spaceCount;
                        choice[sliderChoice].slider = CreateFakeSlider(sliderText, l.y, x, sw, words[sliderText].color);
                        AddChild(choice[sliderChoice].slider[0], i, new(x, lines[l.y].yMin, lines[l.y].height));
                    }
                    else if (text[i] != null || inline[i] != null)
                    {
                        float dif = HashTag.Size(1, words[i].size) - 1;
                        //      if (inline[i] != null) Debug.Log("InLine " + (text[i] == null));
                        rect = text[i] != null ? text[i].GetComponent<RectTransform>() : inline[i].GetComponent<RectTransform>();
                        rect.SetParent(rectTransform, false);
                        rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;
                        if (inline[i] != null)
                        {
                            rect.sizeDelta = new Vector2(widths[i], widths[i] / words[i].texture.width * words[i].texture.height);
                            rect.anchoredPosition = new Vector2(x, main.height - lines[l.y].yMin - lines[l.y].height);
                        }
                        else
                        {
                            rect.sizeDelta = new Vector2(widths[i], lines[l.y].height);
                            rect.anchoredPosition = new Vector2(x, main.height - lines[l.y].yMin - lines[l.y].height + dif * textHeight);
                        }//   rect.anchoredPosition3D = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, -0.01f);
                        AddChild(rect, i, new(x, lines[l.y].yMin, lines[l.y].height));
                        x += widths[i] + space;
                        //     if (marker.name == "bargraph") Debug.Log("bar: " + s);
                    }
                i = l.bulletIndex;
                if (l.bullet != "")
                {
                    float dif = HashTag.Size(1, words[i].size) - 1;
                    go = new GameObject("word " + i);
                    text[i] = go.AddComponent<TextMeshProUGUI>();
                    text[i].fontSize = textHeight;
                    text[i].color = words[start].color;
                    text[i].fontStyle = words[start].style;
                    text[i].text = l.bullet;
                    rect = text[i].GetComponent<RectTransform>();
                    rect.SetParent(rectTransform, false);
                    rect.pivot = rect.anchorMin = rect.anchorMax = Vector2.zero;
                    rect.sizeDelta = new Vector2(l.width, lines[l.y].height);
                    rect.anchoredPosition = new Vector2(xBullet, main.height - lines[l.y].yMin - lines[l.y].height + dif * textHeight);
                    //   rect.anchoredPosition3D = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, -0.01f);
                    AddChild(rect, i, new(xBullet, lines[l.y].yMin, lines[l.y].height));
                }
            }
        }
        float GetX(TextAlignmentOptions tao, LineRef l, float spaceCount, float space, bool hasSlider)
        {
            float x = l.colOffset;
            float midFactor = l.count > 1 ? l.col / (float)(l.count - 1) : 0.5f;
            if (hasSlider)
                x += l.textOffset;
            else
                switch (tao)
                {
                    case TextAlignmentOptions.Left:
                        x += l.textOffset;
                        break;
                    case TextAlignmentOptions.Right:
                        x = l.colOffset + l.max - l.width - spaceCount * space;
                        break;
                    case TextAlignmentOptions.Center:
                        midFactor *= l.max - l.textOffset;
                        float mid = midFactor - (l.width - spaceCount * space) / 2;
                        x = l.colOffset + mid;
                        break;
                    default:
                        x += l.textOffset;
                        space = (l.max - l.width) / (spaceCount <= 1 ? 1 : spaceCount);
                        break;
                }
                return x;
        }
        void AddTexts()
        {
            text = new TextMeshProUGUI[words.Count];
            textBack = new Image[words.Count];
            inline = new Image[words.Count];
            for (int i = 0; i < words.Count; i++)
            { text[i] = null; inline[i] = null; }
            if (lines.Length == 0) return;
            int lastIndent = -1;
            string indentString = "";
            LineRef l = new LineRef();
            bool first = true, onlyNewLine = false, onlyNext = false, indented = false;
            IndentOrder io = new IndentOrder();
            while (l.i < words.Count && l.y < lines.Length)
            {
                bool add = true;
                WordRef w = words[l.i];
                if (w.operation == WordOperation.NewLine)
                {
                    l.NewLine(true);
                    indented = false;
                    first = true;
                    onlyNewLine = false;
                    onlyNext = false;
                    if (l.line >= lines.Length) break;
                }
                else if (onlyNewLine)
                { }
                else if (w.operation == WordOperation.Next)
                {
                    onlyNext = false;
                    if (!l.NextCol())
                        onlyNewLine = true;
                   }
                else if (onlyNext)
                { }
                else if (w.operation == WordOperation.Alignment)
                    lastAlignment = w.alignment;
                else if (w.operation == WordOperation.Table || w.operation == WordOperation.Dots)
                {
                    l.count = w.index;
                    l.dotted = w.operation == WordOperation.Dots;
                }
                else if (w.operation == WordOperation.Indent)
                {
                    io.CurrentLine(w.index, w.bullet);
                    l.indent = io.current;
                    indented = true;
                    l.bulletIndex = l.i;
                }
                else
                {
                    if (first && !indented)
                        io.CurrentLine(0, IndentType.None);
                    l.bullet = first ? io.Bullet() : "";
                    if (first) l.Init(lastAlignment, lines[l.y].width, portions[l.line], l.count, textHeight);
                    first = false;
                    int li = l.i;
                    Add(l);
                     if (l.i >= words.Count || l.y >= lines.Length)
                        break;
                    else if (l.nextCol)
                        onlyNext = true;
                    else if (l.nextLine)
                        l.NewLine(false);
                    add = false;
                }
                if (add) l.i++;//   Debug.Log("valid count = " + validWordCount + " > " + marker.gameObject.name);
            }
        }

        Slider CreateSlider(int index, int y, float x, float w, Color c)
        {
            GameObject g = CoreTame.instance.messageCanvas.gameObject.transform.Find("Slider").gameObject;
            g = GameObject.Instantiate(g);
            g.name = "slider " + index;
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.parent = rectTransform;
            rt.sizeDelta = new Vector2(w, textHeight * 1.2f);
            rt.anchoredPosition = new Vector2(x, main.height - lines[y].yMin - lines[y].height);
            rt.position = new Vector3(x, main.height - lines[y].yMin - lines[y].height, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchorMax = rt.anchorMin = new Vector2(0, 0);

            Slider slider = g.GetComponent<Slider>();
            slider.value = 0;
            slider.colors = new ColorBlock { normalColor = marker.colorSetting.choice, colorMultiplier = 1 };

            Image[] ims = rt.GetComponentsInChildren<Image>();
            foreach (Image im in ims)
                if (im.gameObject.name == "Background" || im.gameObject.name == "Fill") im.color = c;
            return slider;
        }
        RectTransform[] CreateFakeSlider(int index, int y, float x, float w, Color c)
        {
            RectTransform[] rts = new RectTransform[2];

            GameObject g = new GameObject("slider " + index);
            Image im = g.AddComponent<Image>();
            im.color = c;
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.parent = rectTransform;
            rt.sizeDelta = new Vector2(w, textHeight * 0.3f);
            rt.anchoredPosition3D = new Vector3(x, main.height - lines[y].yMin - lines[y].height / 2, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchorMax = rt.anchorMin = new Vector2(0, 0);
            rts[0] = rt;

            g = new GameObject("handle " + index);
            im = g.AddComponent<Image>();
            im.color = marker.colorSetting.choice;
            rt = g.GetComponent<RectTransform>();
            rt.parent = rectTransform;
            rt.sizeDelta = new Vector2(textHeight * 0.8f, textHeight * 0.8f);
            rt.anchoredPosition3D = new Vector3(x, main.height - lines[y].yMin - lines[y].height / 2 - rt.sizeDelta.y / 3, 0);
            rt.pivot = new Vector2(0, 0);
            rt.anchorMax = rt.anchorMin = new Vector2(0, 0);
            rts[1] = rt;

            //    rts[0].anchoredPosition3D
            return rts;
        }
        void UpdateSlider(InfoChoice ic, float dir, int stops)
        {
            int current = ic.value - 1;
            if (current == stops) current = stops - 1;
            if (dir > 0) current++;
            if (dir < 0) current--;
            if (current < 0) current = 0;
            if (current >= stops) current = stops - 1;
            ic.value = current + 1;
            float d = ic.slider[0].sizeDelta.x / (stops - 1);
            float x = ic.slider[0].anchoredPosition.x + current * d;
            ic.slider[1].anchoredPosition = new Vector2(x, ic.slider[1].anchoredPosition.y);
        }
        public void UpdateReferences()
        {
            if (justImage)
            {
                parentFrame.UpdateReferences();
                return;
            }   //    Debug.Log("update ref " + text[0].text);
            for (int i = 0; i < words.Count; i++)
                if (text[i] != null)
                {
                    if (words[i].prop == InfoReference.RefProperty.Time)
                        text[i].text = (int)Tames.TameElement.ActiveTime + "";
                    else if (words[i].prop == InfoReference.RefProperty.Value)
                    {
                        //       Debug.Log("hold ref");
                        if (parent.references[words[i].index] != null)
                        {
                            //       Debug.Log("ref bef " + words[i].index + "," + (text[i] == null) + " " + words[i].style);
                            text[i].text = parent.references[words[i].index].Get(InfoReference.RefProperty.Value);
                            //         Debug.Log("hold " + words[i].depth + " " + text[i].text);
                        }
                    }
                    else if (words[i].prop == InfoReference.RefProperty.Name)
                        text[i].text = parent.references[words[i].index].Get(InfoReference.RefProperty.Name);

                }
                else if (words[i].prop == InfoReference.RefProperty.Value) Debug.Log("hold null");
        }
        private static float[] XS = new float[] { -1, -1, -1, 0, 1, 1, 1, 0 };
        private static float[] YS = new float[] { -1, 0, 1, 1, 1, 0, -1, -1 };
        public Vector3 ClosestToTarget(Vector3 p)
        {
            Vector3 q = Vector3.zero;
            Vector3[] a = new Vector3[8];
            float d, min;
            if (mpp == InfoPosition.WithObject)
            {
                for (int i = 0; i < 4; i++)
                    a[i] = rectTransform.position + main.size.x * (i < 2 ? -0.5f : 0.5f) * rectTransform.right + main.size.y * (i % 2 == 0 ? -0.5f : 0.5f) * rectTransform.up;
                q = a[0];
                min = Vector3.Distance(p, a[0]);
                for (int i = 1; i < 4; i++)
                    if ((d = Vector3.Distance(p, a[i])) < min) { min = d; q = a[i]; }
                q = Camera.main.transform.position + (q - Camera.main.transform.position).normalized;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    a[i] = rectTransform.position + main.size.x * XS[i] * 0.5f * rectTransform.right + main.size.y * YS[i] * 0.5f * rectTransform.up;
                    a[i] = Camera.main.ScreenToWorldPoint(new Vector3(a[i].x, a[i].y, 1));
                }
                q = a[0];
                min = Vector3.Distance(p, a[0]);
                for (int i = 1; i < 8; i++)
                    if ((d = Vector3.Distance(p, a[i])) < min) { min = d; q = a[i]; }
            }

            return q;
        }

        public void UpdateColors()
        {
            if (justImage)
            {
                parentFrame.UpdateColors();
                return;
            }
            if (marker.colorSetting.image == null)
                for (int i = 0; i < childTypes.Count; i++)
                    if (childTypes[i] == -1)
                    {
                        Image im = children[i].gameObject.GetComponent<Image>();
                        if (im != null) im.color = marker.colorSetting.background;
                    }
                    else if (childTypes[i] >= 0)
                        text[childTypes[i]].color = words[childTypes[i]].color;

        }
        public void UpdateChoice()
        {
            if (justImage)
            {
                parentFrame.UpdateChoice();
                return;
            }
            int j, dir = Tames.TameInputControl.hooverDirection;
            for (int i = 0; i < choice.Count; i++)
            {
                InfoChoice ch = choice[i];
                if (ch.slider != null)
                {
                    int v = ch.value;
                    if (Mouse.current != null)
                        UpdateSlider(ch, Mouse.current.scroll.value.y, ch.stops);
                    if (v != ch.value)
                    {
                        lastChanged = Tames.TameElement.ActiveTime;
                        changeRecorded = false;
                    }
                }
                else if (ch.control != null)
                    if (ch.control.Pressed())
                    {
                        Debug.Log("cvic " + i);
                        ch.selected = !ch.selected;
                        lastChanged = Tames.TameElement.ActiveTime;
                        changeRecorded = false;
                        if (ch.selected && !ch.multiple)
                            foreach (InfoChoice ch2 in choice)
                                if (ch2 != ch && !ch2.multiple)
                                    ch2.selected = false;
                    }

            }
            foreach (InfoChoice ch in choice)
                foreach (int g in ch.indexes)
                    if (text[g] != null) text[g].color = ch.selected ? marker.colorSetting.choice : words[g].color;


        }

        public int currentSection = 0;
        public bool GoPrev()
        {
            if (currentSection <= 0 || sections.Count == 0)
                return false;
            currentSection--;
            ShowSections();
            return true;
        }
        public bool GoNext()
        {
            if (currentSection >= sections.Count - 1 || sections.Count == 0)
                return false;
            currentSection++;
            Debug.Log(marker.name + " " + index + ": sec " + currentSection + " of " + sections.Count);
            ShowSections();
            return true;

        }
        public void Enter(int dir, bool ji = false)
        {
            //      Debug.Log(": " + index + " " + justImage);
            if (justImage)
                parentFrame.imageItem.texture = item.image;
            else
            {
                if (!ji && item.image != null) imageItem.texture = item.image;
                if (dir < 0)
                    currentSection = sections.Count - 1;
                else
                    currentSection = 0;
                ShowSections();
            }
        }
        void ShowSections()
        {
            if (sections.Count != 0)
                for (int i = 0; i < sections.Count; i++)
                    for (int j = sections[i].start; j < sections[i].end; j++)
                        ShowText(text[j], i <= currentSection);

        }
        void ShowText(TextMeshProUGUI t, bool vis)
        {
            if (t != null)
            {
                t.gameObject.SetActive(vis);
                t.enabled = vis;
                Debug.Log($"show {t.text}: {vis}");
            }
        }
        public void Show(bool vis, bool passed = true, bool ji = false)
        {
            owner.SetActive(vis);
            if (vis && passed) Enter(-1, ji);
        }


    }
}