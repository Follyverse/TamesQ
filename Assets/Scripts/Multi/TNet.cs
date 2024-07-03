using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Multi
{
    public class TNet
    {
        public const int Transform = 1;
        public const int ID = 2;
        public const int Intro = 3;
        public const int Survey = 4;
        public const int Alter = 5;
        public const int Progress = 6;

        public const int Project = 15;
        public const int Unreg = 16;
        public const int Request = 21;

        public class Message
        {
            public int messageId = 0;
            public int index = -1;
            public float[] floats = null;
            public string[] strings = null;
            public int[] ints = null;

            public string Json { get { Debug.Log("json"); return JsonUtility.ToJson(this, false); } }

           
        }
        public static TNet instance;
        public int userIndex = -1;
        public string webAddress = "";

        public string ip;
        public string port;
        void GetRequest(out UnityWebRequest request, byte[] bs)
        {
            webAddress = "http://" + ip + (port.Length == 0 ? "" : ":" + port) + "/";
            Debug.Log("try: 3 " + webAddress);
            request = new UnityWebRequest(webAddress, "GET", new DownloadHandlerBuffer(), new UploadHandlerRaw(bs));
            Debug.Log("try: 4");
        }

        public void Send(int id, string text)
        {
            Message msg = new Message();
            msg.index = CoreTame.userId;
            msg.messageId = id;
            msg.floats = new float[] { Tames.TameElement.ActiveTime };
            msg.strings = new string[] { text };
            string json = msg.Json;
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            GetRequest(out UnityWebRequest request, bytes);
            request.uploadHandler.contentType = "application/json";
            request.SendWebRequest();
        }
        public void Send(List<Vector3> pos, List<Quaternion> rot, List<float> time)
        {
             if (CoreTame.userId < 0) return;
            float[] fs = new float[pos.Count * 8];
            for (int i = 0; i < pos.Count; i++)
            {
                int i8 = i * 8;
                fs[i8] = time[i];
                fs[i8 + 1] = pos[i].x;
                fs[i8 + 2] = pos[i].y;
                fs[i8 + 3] = pos[i].z;
                fs[i8 + 4] = rot[i].x;
                fs[i8 + 5] = rot[i].y;
                fs[i8 + 6] = rot[i].z;
                fs[i8 + 7] = rot[i].w;
            }
            Message msg = new Message() { messageId = Transform, floats = fs, index = CoreTame.userId };
            byte[] bytes = Encoding.UTF8.GetBytes(msg.Json);
             GetRequest(out UnityWebRequest request, bytes);
            request.uploadHandler.contentType = "application/json";
            request.SendWebRequest();
        }
        public static void Send(string ip, string port, Message msg)
        {
             byte[] bytes = Encoding.UTF8.GetBytes(msg.Json);
            string webAddress = "http://" + ip + (port.Length == 0 ? "" : ":" + port) + "/";
            var request = new UnityWebRequest(webAddress, "GET", new DownloadHandlerBuffer(), new UploadHandlerRaw(bytes));
            request.uploadHandler.contentType = "application/json";
            request.SendWebRequest();
        }
       
    }
}
