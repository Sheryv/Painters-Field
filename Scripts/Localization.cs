using System;
using System.Collections.Generic;
using Assets.Scripts.Gui;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof (Text))]
    public class Localization : MonoBehaviour
    {
        public const int LanguageCount = 1;
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

        public static string LocalCode()
        {
            return "en";
        }

        public static int LocalIndex()
        {
            return 0;
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
            {"ui.loading", "Loading..."},
            {"ui.play", "Play"},
            {"ui.ok", "OK"},
            {"ui.tip", "Tip"},
            {"ui.resume", "Resume"},
            {"ui.exit", "Exit"},
            {"ui.singleplayer", "Singleplayer"},
            {"ui.single", "Single Battle"},
            {"ui.ranking_battle", "Ranking Battle"},
            {"ui.save", "Save"},
            {"ui.connect", "Connect"},
            {"ui.continue", "Continue"},
            {"ui.color", "Color"},
            {"ui.version", "Version"},
            {"ui.ipaddress", "IP Address"},
            {"ui.available_soon", "Available Soon"},
            {"ui.sound", "Sound Volume"},
            {"ui.rounds_count", "Rounds Number"},
            {"ui.show_tips", "Show Tips Again"},
            {"ui.duration", "Match Duration (sec.)"},
            {"ui.report_bug", "Report an issue"},
            {"ui.request_feature", "Request new feature"},
            {"ui.won", "won"},
            {"ui.round", "round"},
            {"ui.website", "Website"},
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