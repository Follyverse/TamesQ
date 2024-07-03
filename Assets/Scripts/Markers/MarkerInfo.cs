using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
namespace Markers
{
  
    public class MarkerInfo : MonoBehaviour
    {
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
        public QControl[] controls;
        private Texture[] output;
        private bool changed = false;
        private InfoUI.InfoControl ic = null;
        /*
         * width and height,  
         */
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
}

