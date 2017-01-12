using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Gui;
using UnityEngine;

namespace Assets.Scripts
{
    class Wd
    {
       /// <summary>
       /// Basic logging class
       /// </summary>
       /// <param name="msg">message</param>
       /// <param name="sender">object sending</param>
        public static void Log(string msg, object sender)
       {
           string s = "[" + sender.GetType().Name + "] " + msg;
           // Write(s);
            Debug.Log(s);
        }
       /// <summary>
       /// Basic logging class
       /// </summary>
       /// <param name="msg">message</param>
       /// <param name="sender">object sending</param>
        public static void LogWarning(string msg, object sender)
       {
           string s ="["+sender.GetType().Name+"] "+msg;
           // Write(s);
            Debug.LogWarning(s);
        }

       /// <summary>
       /// Basic logging class
       /// </summary>
       /// <param name="msg">message</param>
       /// <param name="sender">object sending</param>
        public static void LogErr(string msg, object sender)
       {
           string s ="["+sender.GetType().Name+"] "+msg;
         //   Write(s);
            Debug.LogError(s);
        }

        /// <summary>
        /// Basic logging class
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="sender">object sending</param>
        /// <param name="code">error code</param>
        public static void LogErr(string msg, object sender, int code)
        {
            string s ="["+sender.GetType().Name+": "+code+"] "+msg;
        //    Write(s);
            Debug.LogError(s);
        }

        public static void ConsoleLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
            {
                if (condition.Contains("Font size and style"))
                {
                    return;
                }
            }
//            if (type == LogType.Error || type == LogType.Assert)
//                Wd.LogErr(condition, false);
//            else if (type == LogType.Exception)
//                Wd.Ec(condition, false);
//            else if (type == LogType.Warning)
//                Wd.Wc(condition, false);
//            else
//                Wd.Dc(condition, false);
           // Write("<color=#0095FF>[" + type.ToString()+"]</color> "+condition);
            Write("|>" + type.ToString() + "<| " + condition);
        }

        private static void Write(string msg)
        {
            LoggingPanel.Instance.AddText(msg);
        }
    }
}
