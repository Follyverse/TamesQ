using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;

namespace Tames
{
    public class IntroPage : MonoBehaviour
    {
        public static IntroPage instance;

        public bool opening = true;
        public Texture image;
        public Color background = Color.black;
        public Color questions = Color.grey;
        public Color answers = Color.white;
        public Color required = Color.red;
        public Color selected = Color.yellow;
        public Color foreNext = Color.black;
        public Color backNext = Color.white;
        public int lineCount = 20;
        public IntroItem[] items;
        public static bool active = true;
        [HideInInspector]
        public Canvas canvas;
        [HideInInspector]
        public GameObject owner;
        [HideInInspector]
        public RectTransform rectTransform;
        [HideInInspector]
        public RectTransform back;
        [HideInInspector]
        public TextMeshProUGUI temp;
        [HideInInspector]
        public GameObject inputHolder;
        [HideInInspector]
        public GameObject dropDownHolder;
        [HideInInspector]
        public float originalAspectRatio;
        [HideInInspector]
        public float originalHeight;
        Image next, backImage;
        TextMeshProUGUI nextText, backText;
        int currentPage = 0, pageCount = 0;
        Rect nextRect, backRect;
        List<IntroItem> sendable = new List<IntroItem>();
#if UNITY_EDITOR
        public void Read()
        {
            string filename = EditorUtility.OpenFilePanel("Select a file", Application.dataPath, ".txt");
            if (!System.IO.File.Exists(filename)) return;
            string[] lines = System.IO.File.ReadAllLines(filename);

            List<IntroItem> items = new List<IntroItem>();
            IntroItem ii;
            bool req;
            /*          for (int i = 0; i < lines.Length; i++)
                      {
                          int small = lines[i].IndexOf('>');
                          if (small < 0) continue;
                          string command = lines[i].Substring(0, small);
                          string value = lines[i].Substring(small + 1);
                          if (command == "")
                          {
                              items.Add(ii = new IntroItem() { type = AnswerType.Text });
                              ii.SetQuestion(value);

                              continue;
                          }
                          string[] s = command.Split(':', StringSplitOptions.RemoveEmptyEntries);
                          if (s.Length == 0 || s.Length > 2) continue;

                          req = s[0][0] == '*';
                          string c = req ? s[0].Substring(1) : s[0];
                          switch (c)
                          {
                              case "br": items.Add(new IntroItem() { type = AnswerType.PageBreak }); break;
                              case "ty":
                                  items.Add(ii = new IntroItem() { type = AnswerType.Typable, required = req });
                                  ii.SetQuestion(value);
                                  break;
                              case "dr":
                                  items.Add(ii = new IntroItem() { type = AnswerType.DropDown, required = req });
                                  ii.SetQuestion(value);
                                  break;
                              case "ch":
                                  items.Add(ii = new IntroItem() { type = AnswerType.Checkbox, required = req });
                                  ii.SetQuestion(value);
                                  ii.shorthand = s.Length == 2 ? s[1] : "";
                                  break;
                          }
                      }
                      this.items = items.ToArray();
               */
        }
#endif
        private void Start()
        {
            active = opening;
            instance= this;
            originalHeight = Screen.height;
            float h = Screen.height / (float)lineCount;
            originalAspectRatio = Screen.width / (float)Screen.height;
            owner = new GameObject("canvas");
            owner.transform.parent = transform;
            canvas = owner.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.targetDisplay = 0;
            owner.AddComponent<GraphicRaycaster>();
            if ((rectTransform = owner.GetComponent<RectTransform>()) == null) rectTransform = owner.AddComponent<RectTransform>();
            rectTransform.parent = owner.transform.parent.transform;
            owner.SetActive(true);

            GameObject g = new GameObject("back");
            Image im = g.AddComponent<Image>();
            back = g.GetComponent<RectTransform>();
            back.parent = rectTransform;
            back.sizeDelta = new Vector2(Screen.width, Screen.height);
            back.anchoredPosition = Vector2.zero;
            back.anchorMin = Vector2.zero;
            back.anchorMax = Vector2.zero;
            back.pivot = Vector2.zero;
            im.color = new Color(background.r, background.g, background.b);
            if (image != null) im.sprite = Sprite.Create((Texture2D)image, new Rect(0, 0, image.width, image.height), Vector2.zero);

            g = new GameObject("temp");
            temp = g.AddComponent<TextMeshProUGUI>();
            g.GetComponent<RectTransform>().parent = rectTransform;
            RectTransform rt = CoreTame.instance.messageCanvas.GetComponent<RectTransform>();
            for (int i = 0; i < rt.childCount; i++)
            {
                g = rt.GetChild(i).gameObject;
                if (g.GetComponent<TMP_Dropdown>() != null)
                    dropDownHolder = g;
                if (g.GetComponent<TMP_InputField>() != null)
                    inputHolder = g;
            }

            nextRect = CreateButton(new Rect(Screen.width - Screen.width / 8f - h / 2, h / 2f, Screen.width / 8f, h), foreNext, backNext, "Next", out next, out nextText);
            backRect = CreateButton(new Rect(h / 2, h / 2f, Screen.width / 8f, h), foreNext, backNext, "Back", out backImage, out backText);

            int lastLine = 0;
            bool newPage;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    newPage = false;
                    if (pageCount == 0) pageCount = 1;
                    if (items[i].option.type == AnswerType.PageBreak)
                    {
                        newPage = true;
                        lastLine = 0;
                    }
                    else if (items[i].option.type == AnswerType.Line)
                    {
                        if (lastLine + items[i].option.lineCount > lineCount - 2)
                        {
                            newPage = true;
                            lastLine = 0;
                        }
                        else
                            lastLine += items[i].option.lineCount;
                    }
                    else
                    {
                        lastLine = items[i].Create(this, lastLine, out newPage);
                        if (items[i].option.Sendable)
                            sendable.Add(items[i]);
                    }
                    //   Debug.Log(lastLine + " " + items[i].shorthand);
                    if (newPage)
                    {
                        currentPage++;
                        pageCount++;
                    }
                    items[i].page = currentPage;
                }
            }
            if (pageCount > 0)
                SetPage(0);
            temp.gameObject.SetActive(false);
            for (int i = 0; i < sendable.Count; i++)
                ProjectData.headers[0] += (i == 0 ? "" : ",") + sendable[i].option.name;
            canvas.gameObject.SetActive(opening);
        }
        Rect CreateButton(Rect rect, Color fore, Color back, string text, out Image next, out TextMeshProUGUI nextText)
        {
            GameObject g = new GameObject("next");
            next = g.AddComponent<Image>();
            next.color = back;
            next.raycastTarget = true;
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.parent = rectTransform;
            rt.sizeDelta = new Vector2(rect.width, rect.height);
            rt.pivot = rt.anchorMin = rt.anchorMax = Vector2.zero;
            rt.anchoredPosition = new Vector2(rect.x, rect.y);
            Rect r = new Rect(rt.anchoredPosition, rt.sizeDelta);

            g = new GameObject();
            nextText = g.AddComponent<TextMeshProUGUI>();
            nextText.color = fore;
            nextText.alignment = TextAlignmentOptions.Center;
            nextText.text = text;
            nextText.fontSize = 0.8f * rect.height;
            nextText.fontStyle = FontStyles.Bold;

            rt = g.GetComponent<RectTransform>();
            rt.parent = rectTransform;
            rt.sizeDelta = new Vector2(rect.width, rect.height);
            rt.pivot = rt.anchorMin = rt.anchorMax = Vector2.zero;
            rt.anchoredPosition = new Vector2(rect.x, rect.y);
            return r;
        }
        private void Update()
        {
            if (active)
            {
             //   Debug.Log("check 2");
                if (CoreTame.resolutionChanged > 1)
                {
                    CoreTame.resolutionChanged = 1;
                    Debug.Log("resolution changed ");
                    Position();
                }
                if (Mouse.current != null)
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        Vector2 p = Mouse.current.position.value;
                        Debug.Log("clicked:" + p.ToString() + " " + nextRect.ToString());
                        if (backRect.Contains(p))
                            SetPage(currentPage - 1);
                        else if (nextRect.Contains(p))
                        {
                            if (AllRequired())
                                if (currentPage == pageCount - 1)
                                {
                                    Debug.Log("active 1 = " + active);
                                    Submit();
                                    Debug.Log("active 2 = " + active);

                                }
                                else SetPage(currentPage + 1);
                        }
                        else
                            for (int i = 0; i < items.Length; i++)
                                if (items[i] != null)
                                    if (items[i].option.type == AnswerType.Checkbox)
                                        if (items[i].Check(p)) break;
                    }
            }
        }
        bool AllRequired()
        {
            foreach (IntroItem item in items)
                if (item.option.required && item.page == currentPage)
                    if (!item.Attended()) return false;
            return true;
        }
        void SetPage(int p)
        {
            if (p < 0) p = 0;
            for (int i = 0; i < items.Length; i++)
                if (items[i] != null)
                    items[i].SetActive(p == items[i].page);
            nextText.text = p == pageCount - 1 ? "Submit" : "Next";
            backText.text = p > 0 ? "Back" : "";
            backImage.gameObject.SetActive(p > 0);
            currentPage = p;
        }
        void Position()
        {

            back.sizeDelta = new Vector2(Screen.width, Screen.height);
            back.anchoredPosition = Vector2.zero;
            back.pivot = Vector2.zero;

            float factor = Screen.height / originalHeight;
            float itemHeight = Screen.height / (float)lineCount;
            float yOffset = itemHeight / 2f;

            RectTransform rt = next.rectTransform;
            rt.sizeDelta = new Vector2(itemHeight * 6, itemHeight);
            rt.pivot = Vector2.zero;
            rt.anchoredPosition = new Vector2(Screen.width - rt.sizeDelta.x - itemHeight / 2, itemHeight / 2);
            nextRect = new Rect(rt.anchoredPosition, rt.sizeDelta);
            rt = nextText.rectTransform;
            rt.sizeDelta = new Vector2(itemHeight * 6, itemHeight);
            rt.pivot = Vector2.zero;
            rt.anchoredPosition = new Vector2(Screen.width - rt.sizeDelta.x - itemHeight / 2, itemHeight / 2);

            nextText.fontSize = 0.9f * itemHeight;

            foreach (IntroItem i in items)
                if (i != null)
                    i.Position();
        }
        void Submit()
        {
            string r = "";
            for (int i = 0; i < sendable.Count; i++)
                if (sendable[i] != null)
                    r += (i == 0 ? "" : ",") + sendable[i].ToString().Replace(",", "|");
            Multi.TNet.Message msg = new Multi.TNet.Message();
            msg.messageId = Multi.TNet.Intro;
            msg.strings = new string[6];
            msg.strings[0] = ProjectData.instance.token;
            for (int i = 0; i < 4; i++)
                msg.strings[i + 1] = ProjectData.headers[i];
            msg.strings[5] = r;
            Debug.Log("iteration " + ProjectData.instance.port);
            StartCoroutine(Send(msg));
        }
        public IEnumerator Send(Multi.TNet.Message msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg.Json);
            Debug.Log("try: 2");
            string webAddress = "http://" + ProjectData.instance.IP + (ProjectData.instance.port.Length == 0 ? "" : ":" + ProjectData.instance.port) + "/";
            Debug.Log("try: 3 " + webAddress);
            var request = new UnityWebRequest(webAddress, "GET", new DownloadHandlerBuffer(), new UploadHandlerRaw(bytes));
            Debug.Log("try: 4");
            request.uploadHandler.contentType = "application/json";

            yield return request.SendWebRequest();
            Debug.Log(request.result + " " + request.downloadHandler.text);
            var responseText = request.downloadHandler.text;
            if (responseText.IndexOf("error") < 0)
            {
                int p = responseText.IndexOf("userId");
                if (p > 0)
                    p = responseText.IndexOf(":", p);
                if (p > 0)
                    responseText = responseText.Substring(p + 1);
                p = responseText.IndexOf("}");
                if (p > 0)
                    if (int.TryParse(responseText.Substring(0, p), out p))
                    {
                        CoreTame.userId = p;
                        active = false;
                        gameObject.SetActive(false);
                    }
            }

        }
    }
    public enum AnswerType { Typable, DropDown, Checkbox, Text, Image, PageBreak, Line }
    public enum EFontStyle { Normal, Bold, Italic, BoldItalic }
    public enum EAlignment { Left, Mid, Right, Justified }
}
