using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class Prefs
    {
        private const string Key = "painters_field_key";
        public const string StartCountKey = "StartCountKey";
        public const string UseTimeKey = "UseTimeKey";
        public const string ClientIdKey = "ClientIdKey";
        public event Action<string> NickChangedEvent;
        public event Action<ControlTypes> ControlTypeChangedEvent;
        public event Action<float> SoundVolumeChangedEvent;
        public event Action PrefsChangedEvent;

        [SerializeField] private ControlTypes controlType;
        [SerializeField] private string nick;
        [SerializeField] private float soundVolume;

        private Prefs()
        {
        }



        public ControlTypes ControlType
        {
            get { return controlType; }
            set
            {
                if (ControlTypeChangedEvent != null) ControlTypeChangedEvent(value);
                controlType = value;
                PrefsChanged();
            }
        }

        public string Nick
        {
            get { return nick; }
            set
            {
                if (NickChangedEvent != null) NickChangedEvent(value);
                nick = value;
                PrefsChanged();
            }
        }

        public float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                if (SoundVolumeChangedEvent != null) SoundVolumeChangedEvent(value);
                soundVolume = value;
                PrefsChanged();
            }
        }

        private void PrefsChanged()
        {
            if (PrefsChangedEvent != null) PrefsChangedEvent();
        }

        public void Save()
        {
            string s = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(Key, s);
            PlayerPrefs.SetInt(UseTimeKey, Master.UsingTime);
            Wd.Log("Saving prefs | use time: "+Master.UsingTime, this);
        }

        public static Prefs LoadPrefs()
        {
            if (PlayerPrefs.HasKey(Key))
            {

                string js = PlayerPrefs.GetString(Key);
                Prefs prefs = JsonUtility.FromJson<Prefs>(js);
                return prefs;
            }
            else
            {
                Prefs p = new Prefs();
                p.controlType = ControlTypes.Touch;
                p.nick = "Noname";
                return p;
            }
        }

        public static string GetClientId()
        {
            if (PlayerPrefs.HasKey(ClientIdKey))
            {
                return PlayerPrefs.GetString(ClientIdKey);
            }
            StringBuilder sb = new StringBuilder();
            byte[] b = Guid.NewGuid().ToByteArray();
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X"));
            }
            string k = sb.ToString();
            PlayerPrefs.SetString(ClientIdKey, k);
            return k;
        }
    }
}