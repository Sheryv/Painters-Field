using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof (Text))]
    public class Localization : MonoBehaviour
    {
        private bool converted = false;

        private void Awake()
        {
        }

        private void Start()
        {
            if (!converted)
            {
                Text t = GetComponent<Text>();
                string loc = GetLoc(t.text);
                if (loc != null)
                {
                    t.text = loc;
                }
                else
                {
                    Wd.LogErr("Loc key not found: " + t.text + " on " + gameObject.name, this);
                }
                converted = true;
            }
        }

        public static string GetLoc(string key)
        {
            if (LocalizationDictionary.ContainsKey(key))
            {
                return LocalizationDictionary[key];
            }
            else
            {
                Wd.LogWarning("Loc key not found: " + key, Master.Instance);
                return null;
            }
        }

        public static readonly Dictionary<string, string> LocalizationDictionary = new Dictionary<string, string>()
        {
            {"ui.options", "Options"},
            {"ui.configure", "Configure"},
            {"ui.network", "Network"},
            {"ui.start", "Start"},
            {"ui.restart", "Restart"},
            {"ui.editnick", "Edit Nick"},
            {"ui.host", "Host"},
            {"ui.back", "Back"},
            {"ui.exit", "Exit"},
            {"ui.singleplayer", "Singleplayer"},
            {"ui.save", "Save"},
            {"ui.connect", "Connect"},
            {"ui.continue", "Continue"},
            {"ui.color", "Color"},
            {"ui.version", "Version"},
            {"ui.ipaddress", "IP Address"},
            {"ui.available_soon", "Available Soon"},
            {"ui.sound", "Sound Volume"},
            {"ui.rounds_count", "Rounds Number"},
            {"ui.duration", "Match Duration (sec.)"},
            {"ui.won", "won"},
            {"ui.round", "round"},
            {"ui.tournament", "tournament"},
            {"ui.last_round", "last round"},
            {"ui.switch", "Switch ctrl mode"},
            {"Exit", "Exit"}
//            {"player nick", ""},
//            {"Player sadasdsada sadsdas d", ""},
//            {"noname", ""},
//            {"Restart", ""},
//            {"1", ""},
//            {"", ""},
//            {"X", ""},
//            {"Button", ""},
//            {"sad", ""},
        };
    }
}