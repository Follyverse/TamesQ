using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tames
{
    [Serializable]
    public class IntroItem
    {
        public IntroItemOption option;
        public IntroContent content;
        public IntroLink links;
        [Tooltip("Please note, checkboxes are treated as questions not answers. For their actual question, select Nothing as question type.")]
        [HideInInspector]
        public IntroPage[] forward;
        [Tooltip("Fill this only if the type is DropDown, and put each option in one line.")]
        [HideInInspector]
        public int page = 0;
        IntroPage parent;
        private TextMeshProUGUI text, asterisk = null;
        private TMP_InputField inputField;
        private TMP_Dropdown dropDown;
        [HideInInspector]
        public Image image;
        float right = 0.95f;
        float left = 0.05f;
        float originalFontSize;
        int lineCount = 1;
        int questionLine = -1;
        bool selected = false;
        Rect checkBox;
        public int Create(IntroPage parent, int lastLine, out bool newPage)
        {
            this.parent = parent;
            GameObject ig, g = new GameObject("question");
            text = g.AddComponent<TextMeshProUGUI>();
            g.GetComponent<RectTransform>().parent = parent.rectTransform;
            originalFontSize = text.fontSize = 0.9f * Screen.height / (float)parent.lineCount;
            text.color = parent.questions;
            text.alignment = GetAlignment();
            text.enableWordWrapping = true;
            text.text = "";
            text.fontStyle = GetStyle();
            if (option.Requirable)
            {
                GameObject ga = new GameObject("asterisk");
                asterisk = ga.AddComponent<TextMeshProUGUI>();
                ga.GetComponent<RectTransform>().parent = parent.rectTransform;
                asterisk.color = parent.required;
                asterisk.fontSize = text.fontSize;
                asterisk.alignment = TextAlignmentOptions.MidlineLeft;
                asterisk.fontStyle = FontStyles.Bold;
                asterisk.text = "*";
            }
            int count = 0;
            newPage = false;
            questionLine = lastLine + 1;
            if (option.type == AnswerType.Checkbox) left += 0.05f;
            RectTransform rt;
            switch (option.type)
            {
                case AnswerType.DropDown:
                    ig = GameObject.Instantiate(parent.dropDownHolder);
                    rt = ig.GetComponent<RectTransform>();
                    rt.transform.parent = parent.rectTransform;
                    dropDown = ig.GetComponent<TMP_Dropdown>();
                    dropDown.enabled = true;
                    dropDown.interactable = true;
                    dropDown.targetGraphic.raycastTarget = true;
                    dropDown.targetGraphic.color = parent.answers;
                    dropDown.colors = new ColorBlock()
                    {
                        normalColor = parent.answers,
                        pressedColor = parent.questions,
                        selectedColor = parent.questions,
                        highlightedColor = parent.answers,
                        disabledColor = parent.answers,
                        colorMultiplier = 1
                    };
                    string[] ddItems = content.question.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    List<string> options = new List<string>();
                    options.Add("Please select ... ");
                    options.AddRange(ddItems);
                    dropDown.AddOptions(options);
                    dropDown.value = 1;
                    lastLine++;
                    break;
                case AnswerType.Typable:
                    ig = GameObject.Instantiate(parent.inputHolder);
                    rt = ig.GetComponent<RectTransform>();
                    rt.transform.parent = parent.rectTransform;
                    inputField = ig.GetComponent<TMP_InputField>();
                    inputField.enabled = true;
                    inputField.targetGraphic.raycastTarget = true;
                    count = lineCount = LineCount();
                    lastLine += count + 1;
                    break;
                case AnswerType.Checkbox:
                    ig = new GameObject("check");
                    image = ig.AddComponent<Image>();
                    image.color = parent.answers;
                    image.raycastTarget = true;
                    rt = ig.GetComponent<RectTransform>();
                    rt.parent = parent.rectTransform;
                    text.color = parent.answers;
                    count = lineCount = LineCount();
                    lastLine += count;
                    break;
                case AnswerType.Text:
                    count = lineCount = LineCount();
                    lastLine += count;
                    break;
                case AnswerType.Image:
                    count = lineCount = option.lineCount;
                    ig = new GameObject("check");
                    image = ig.AddComponent<Image>();
                    image.color = Color.white;
                    if (content.texture != null)
                        try { image.sprite = Sprite.Create((Texture2D)content.texture, new Rect(0, 0, content.texture.width, content.texture.height), Vector2.zero); }
                        catch { }
                    image.raycastTarget = true;
                    rt = ig.GetComponent<RectTransform>();
                    rt.parent = parent.rectTransform;
                    lastLine += count;
                    break;
            }
            if (lastLine > parent.lineCount - 2)
            {
                Debug.Log("br: " + lastLine + " " + parent.lineCount);
                questionLine = 0;
                lastLine = count;
                newPage = true;
            }
            if (option.lineAfter) lastLine++;
            Position();
            return lastLine;
        }
        public void SetQuestion(string s)
        { content.question = s; }

        FontStyles GetStyle()
        {
            if (option.style == EFontStyle.Normal) return FontStyles.Normal;
            else if (option.style == EFontStyle.Bold) return FontStyles.Bold;
            else if (option.style == EFontStyle.Italic) return FontStyles.Italic;
            else return FontStyles.Bold & FontStyles.Italic;
        }
        TextAlignmentOptions GetAlignment()
        {
            return option.alignment switch
            {
                EAlignment.Left => TextAlignmentOptions.MidlineLeft,
                EAlignment.Right => TextAlignmentOptions.MidlineRight,
                EAlignment.Mid => TextAlignmentOptions.Center,
                _ => TextAlignmentOptions.Justified
            };
        }
        public bool Check(Vector2 p)
        {
            if (checkBox.Contains(p))
            {
                selected = !selected;
                image.color = selected ? parent.selected : parent.answers;
                return true;
            }
            return false;
        }
        int LineCount()
        {
            TextMeshProUGUI t = parent.temp;
            t.text = "";
            t.color = parent.questions;
            t.fontSize = Screen.height / (float)parent.lineCount;
            t.fontStyle = text.fontStyle;
            string[] words = content.question.Split(' ');
            int count = 1;
            for (int i = 0; i < words.Length; i++)
            {
                t.text += (t.text.Length == 0 ? "" : " ") + words[i];
                Vector2 size = t.GetPreferredValues();
                if (size.x > Screen.width * (right - left))
                {
                    text.text += ' ' + words[i];
                    t.text = "";
                    count++;
                }
                else
                    text.text += (text.text.Length == 0 ? "" : " ") + words[i];
            }
            return count;
        }
        public void Position()
        {
            if (option.type == AnswerType.PageBreak || option.type == AnswerType.Line) return;
            float oh = parent.originalHeight;
            float ow = oh * parent.originalAspectRatio;
            float factor = Mathf.Min(Screen.width / ow, Screen.height / oh);
            float adjustedWidth = ow * factor;
            float itemHeight = oh * factor / parent.lineCount;
            float yOffset = itemHeight / 2f;
            float xOffset = itemHeight;
            float cOffset = option.type == AnswerType.Checkbox ? xOffset : 0;
            float width = adjustedWidth - 2 * xOffset;

            RectTransform rt = parent.rectTransform;
       //     Debug.Log("question " + width + " " + adjustedWidth);
            if (option.type != AnswerType.Image)
            {
                text.fontSize = factor * originalFontSize;
                rt = text.rectTransform;
                rt.sizeDelta = new Vector2(width - cOffset, itemHeight * lineCount);
                rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                rt.anchoredPosition = new Vector2(xOffset + cOffset, Screen.height - yOffset - (questionLine + lineCount) * itemHeight);
                //    Debug.Log(rt.anchoredPosition.y + " " + shorthand);
                if (option.Requirable)
                {
                    asterisk.fontSize = factor * originalFontSize;
                    rt = asterisk.rectTransform;
                    rt.sizeDelta = new Vector2(itemHeight / 2, itemHeight);
                    rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                    rt.anchoredPosition = new Vector2(xOffset / 4, Screen.height - yOffset - (questionLine + 1) * itemHeight);
                }
            }
            bool assigned = false;
            int yMore = 0;
            switch (option.type)
            {
                case AnswerType.Typable: assigned = true; yMore = 1; rt = inputField.gameObject.GetComponent<RectTransform>(); break;
                case AnswerType.DropDown: assigned = true; rt = dropDown.gameObject.GetComponent<RectTransform>(); break;
                case AnswerType.Checkbox:
                    rt = image.gameObject.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(itemHeight * 0.7f, itemHeight * 0.7f);
                    rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                    rt.anchoredPosition = new Vector2(xOffset, Screen.height - yOffset - (questionLine + 0.7f) * itemHeight);
                    checkBox = new Rect(rt.anchoredPosition, rt.sizeDelta);
                    break;
                case AnswerType.Image:
                    float ar = content.texture != null ? content.texture.width / (float)content.texture.height : 1;
                    rt = image.gameObject.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(itemHeight * lineCount * ar, itemHeight * lineCount);
                    rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                    cOffset = option.alignment switch
                    {
                        EAlignment.Left => 0,
                        EAlignment.Right => width - xOffset - rt.sizeDelta.x,
                        EAlignment.Mid => (width - xOffset - rt.sizeDelta.x) / 2,
                        _ => 0
                    };
                    rt.anchoredPosition = new Vector2(xOffset + cOffset, Screen.height - yOffset - (questionLine + lineCount) * itemHeight);
                    break;
            }
            if (assigned)
            {
                rt.sizeDelta = new Vector2(width, itemHeight);
                rt.pivot = rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
                rt.anchoredPosition = new Vector2(xOffset, Screen.height - yOffset - (questionLine + lineCount + yMore) * itemHeight);
            }
        }
        public void SetActive(bool b)
        {
            if (option.type == AnswerType.PageBreak || option.type == AnswerType.Line) return;
            text.gameObject.SetActive(b);
            if (option.Requirable) asterisk.gameObject.SetActive(b);
            switch (option.type)
            {
                case AnswerType.Typable: inputField.gameObject.SetActive(b); break;
                case AnswerType.DropDown: dropDown.gameObject.SetActive(b); break;
                case AnswerType.Checkbox:
                case AnswerType.Image: image.gameObject.SetActive(b); break;
            }
        }
        public bool Attended()
        {
            switch (option.type)
            {
                case AnswerType.Typable: return inputField.text != "";
                case AnswerType.DropDown: return dropDown.value > 0;
                case AnswerType.Checkbox: return selected;
                default: return true;
            }
        }
        public new string ToString()
        {
            switch (option.type)
            {
                case AnswerType.Typable: return inputField.text;
                case AnswerType.DropDown: return dropDown.value + "";
                case AnswerType.Checkbox: return selected + "";
                default: return "";
            }
        }
    }
    [Serializable]
    public class IntroItemOption
    {
        public bool required = false;
        public string name = "name";
        public AnswerType type = AnswerType.Text;
        public int lineCount = 1;
        public EFontStyle style = EFontStyle.Normal;
        public EAlignment alignment = EAlignment.Left;
        public bool lineAfter = false;
        public bool Requirable
        {
            get
            {
                if (!required) return false;
                return type == AnswerType.Typable || type == AnswerType.DropDown || type == AnswerType.Checkbox;
            }
        }
        public bool Sendable { get { return type == AnswerType.Checkbox || type == AnswerType.DropDown || type == AnswerType.Typable; } }
    }
    [Serializable]
    public class IntroContent
    {
        public Texture texture;
        [SerializeField]
        [TextAreaAttribute(3, 3)]
        public string question;
        [HideInInspector]
        public AnswerType type = AnswerType.Text;
    }

    [Serializable]
    public class IntroLink
    {
        [HideInInspector]
        public int count;
        public IntroPage[] pages;
    }
}