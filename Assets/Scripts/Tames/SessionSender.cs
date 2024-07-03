using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Tames
{
    public class SessionSender
    {
        static string username = "molabfacvut@gmail.com";
        static string password = "irxddjjxkhlkkkkg";
        public static string recipient;
        public static string subject;
        static SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
        };
        public static void Send()
        {
            string[] lines = TameManager.EmailChoice();
            string s = "";
            for (int i = 0; i < lines.Length; i++)
                s += lines[i] + "\n\r";
            smtpClient.Send(username, recipient, subject, s);
        }
    }
}
