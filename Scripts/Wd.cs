using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Gui;
using UnityEngine;

namespace Assets.Scripts
{
    internal class Wd
    {
        private const string CatError = "ErrorsCat";
        private const string CatInfo = "InfoCat";
        private const string CatState = "StateCat";
        public const string ActionSinglePlayerGame = "SinglePlayer_Game";
        public const string ActionNetworkGame = "MultiPlayer_Game";
        public const string ActionPanelOpened = "PanelOpened_UI";

        /// <summary>
        /// Basic logging class
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="sender">object sending</param>
        public static void Log(string msg, object sender)
        {
            if (Master.Debugging)
            {
                string s = "[" + sender.GetType().Name + "] " + msg;
                // Write(s);
                Debug.Log(s);
            }
        }

        /// <summary>
        /// Basic logging class
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="sender">object sending</param>
        public static void LogWarning(string msg, object sender)
        {
            if (Master.Debugging)
            {
                string s = "[" + sender.GetType().Name + "] " + msg;
                // Write(s);
                Debug.LogWarning(s);
            }
        }

        /// <summary>
        /// Basic logging class
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="sender">object sending</param>
        public static void LogErr(string msg, object sender)
        {
            string s = "[" + sender.GetType().Name + "] " + msg;
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
            string s = "[" + sender.GetType().Name + ": " + code + "] " + msg;
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


        public static void EventLogInfo(string action, string name, long value = -1)
        {
            SendEvent(CatInfo, action, name, value);
        }

        public static void EventLogGamePlayInfo(string name, long value = -1)
        {
            if (Master.Instance.GameMode == GameMode.Singleplayer)
            {
                EventLogInfo(ActionSinglePlayerGame, name, value);
            }
            else
            {
                EventLogInfo(ActionNetworkGame, name, value);
            }
        }

        public static void EventLogState(string action, string name, long value = -1)
        {
            SendEvent(CatState, action, name, value);
        }

        public static void EventLogError(string action, string name, long value = -1)
        {
            SendEvent(CatError, action, name, value);
        }

        public static void EventLogException(string msg, string stacktrace, string desc)
        {
            string st;
            if (stacktrace.Length > 100)
            {
                st = stacktrace.Substring(0, 100);
            }
            else
            {
                st = stacktrace;
            }
            string s = desc + " |> " + msg + " #|# \n" + stacktrace;
#if !UNITY_EDITOR
            if(Master.GetAnalytics() != null)
                Master.GetAnalytics().LogException(s, true);
#else
            LogErr("Analitycs Log Error: " + s, Master.Instance);
#endif
        }

        private static void SendEvent(string cat, string action, string name, long value)
        {
#if !UNITY_EDITOR
            if(Master.GetAnalytics() != null)
                 Master.GetAnalytics().LogEvent(cat, action, name, value);
#else
            Log("Analitycs Log: " + cat + " | " + action + " | " + name + " | " + value, Master.Instance);
#endif
        }
    }
}