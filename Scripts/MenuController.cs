using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Gui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MenuController : MonoBehaviour
    {
        public static int DeactivateTrigger;


        public GameObject MainPanelHolder;
        public PanelGui ActivePanel;
        public PanelGui PreviousPanel;

        private List<PlayerPattern> patterns;
        private int cpuNum;

        public List<PlayerPattern> Patterns
        {
            get { return patterns; }
        }

        private void Start()
        {
            VersionLabel.text = Localization.GetLoc("ui.version") + ": " + Application.version;
            PlayerNickTextSingle.text = Master.Instance.Preferences.Nick;
            if (Master.Debugging)
            {
                DebugCanvas.SetActive(true);
            }
            ConfigDurationText.text = Master.Instance.MatchData.MatchDuration.ToString();
            ConfigRoundsText.text = Master.Instance.MatchData.RoundsCount.ToString();
        }


        private void Awake()
        {
            DeactivateTrigger = Animator.StringToHash("DeactivatePanel");
            patterns = new List<PlayerPattern>();
            if (Master.IsHost())
            {
                Master.Instance.Menu.EAddPatternSingle(false);
            }
        }


        public void OnDisable()
        {
            Master.Instance.Preferences.NickChangedEvent -= PreferencesOnNickChangedEvent;
            Master.Instance.Preferences.SoundVolumeChangedEvent -= PreferencesOnSoundVolumeChangedEvent;

        }

        private void PreferencesOnNickChangedEvent(string s)
        {
            PlayerPattern.GetLocalPlayerPattern().PlayerName = s;
            PlayerNickTextSingle.text = s;
            UpdateInLobbyPlayerList();
        }

        public void OnEnable()
        {
            Master.Instance.Preferences.NickChangedEvent += PreferencesOnNickChangedEvent;
            Master.Instance.Preferences.SoundVolumeChangedEvent += PreferencesOnSoundVolumeChangedEvent;
            SoundVolumeSlider.value = Master.Instance.Preferences.SoundVolume;
        }

        private void PreferencesOnSoundVolumeChangedEvent(float f)
        {
            AudioListener.volume = f;
        }

        public void EExit()
        {
            Master.Exit();
        }

        public void ESwitchControls()
        {
            Master.Instance.SwitchControls();
        }

        public void ToggleColorPanel(GameObject panel)
        {
            panel.SetActive(!panel.activeInHierarchy);
        }

        public int GetNextRandomColorId()
        {
            List<int> ids = GetLeftColorIds();
            int r = UnityEngine.Random.Range(0, ids.Count);
            return ids[r];
        }

        public List<int> GetLeftColorIds()
        {
            List<int> ids = new List<int>();
            for (int i = 0; PlayerPattern.ColorsBasic.Length > i; i++)
            {
                ids.Add(i);
            }

            for (int i = 0; i < patterns.Count; i++)
            {
                for (int j = 0; j < PlayerPattern.ColorsBasic.Length; j++)
                {
                    if (GameController.CompareApproxColors32(PlayerPattern.ColorsBasic[j], patterns[i].PlayerColor))
                    {
                        ids.Remove(j);
                    }
                }
            }
            return ids;
        }


        public void EAddPatternSingle(bool isAi)
        {
            AddPattern(isAi ? Player.SourceAi : Player.SourceServer, null);
        }

        private void AddPattern(int source, string nick)
        {
            if (patterns == null)
            {
                patterns = new List<PlayerPattern>();
            }
            int num = patterns.Count;
            if (num >= Master.MaxPlayers)
            {
                return;
            }
            cpuNum++;
            string n = "PL " + cpuNum;
            if (source == Player.SourceServer)
            {
                if (Master.IsHost())
                {
                    n = Master.Instance.Preferences.Nick;
                }
            }
            else if (source == Player.SourceClient)
            {
            }
            else
            {
                n = "CPU " + cpuNum;
            }
            int id = GetNextRandomColorId();
            PlayerPattern pattern = new PlayerPattern(PlayerPattern.ColorsBackground[id], PlayerPattern.ColorsBasic[id], 3f, 2f, n, source);
            patterns.Add(pattern);
            UpdateInLobbyPlayerList();
        }

        public void UpdateInLobbyPlayerList()
        {
            ClearChilds(PlayerEntryLobbyListSingle.gameObject);
            for (int i = 0; i < patterns.Count; i++)
            {
                PlayerPattern pattern = patterns[i];
                GameObject entry = Instantiate(PlayerEntryLobbyPrefab);
                PlayerEntryLobby pl = entry.GetComponent<PlayerEntryLobby>();
                entry.name = pattern.PlayerName;
                pl.SetUp( pattern);
                if (Master.IsSinglePlayer())
                {
                    entry.transform.SetParent(PlayerEntryLobbyListSingle.gameObject.transform);
                    entry.transform.localScale = new Vector3(1f, 1f, 1f);
                } //todo network
            }
        }

        public void ERemovePattern(PlayerEntryLobby pattern)
        {
            patterns.Remove(pattern.Pattern);
            Destroy(pattern.gameObject);
        }

        public void ELoadGame()
        {
            StartMatch(patterns);
        }


        public void ELoadGame2()
        {
            List<PlayerPattern> patterns = new List<PlayerPattern>();
            patterns.Add(new PlayerPattern(0, "pl_1", Player.SourceServer));
            patterns.Add(new PlayerPattern(1, "pl_2", Player.SourceClient));
            StartMatch(patterns);
        }

        public void EChangeNick(InputField input)
        {
            Master.Instance.Preferences.Nick = input.text;
        }

        public void EIncreaseRounds()
        {
            Master.Instance.MatchData.RoundsCount++;
            Master.Instance.MatchData.Mode = MatchMode.Tournament;
            if (Master.Instance.MatchData.RoundsCount >7 )
            {
                Master.Instance.MatchData.RoundsCount = 7;

            }
            ConfigRoundsText.text = Master.Instance.MatchData.RoundsCount.ToString();
        }
        public void EDecreaseRounds()
        {
            Master.Instance.MatchData.RoundsCount--;
            if (Master.Instance.MatchData.RoundsCount < 1)
            {
                Master.Instance.MatchData.RoundsCount = 1;
            }
            if (Master.Instance.MatchData.RoundsCount == 1)
                Master.Instance.MatchData.Mode = MatchMode.Single;
            ConfigRoundsText.text = Master.Instance.MatchData.RoundsCount.ToString();
        }
        public void EIncreaseDuration()
        {
            Master.Instance.MatchData.MatchDuration+=10;
            if (Master.Instance.MatchData.MatchDuration >420 )
            {
                Master.Instance.MatchData.MatchDuration = 420;

            }
            ConfigDurationText.text = Master.Instance.MatchData.MatchDuration.ToString();
        }
        public void EDecreaseDuration()
        {
            Master.Instance.MatchData.MatchDuration-=10;
            if (Master.Instance.MatchData.MatchDuration < 10)
            {
                Master.Instance.MatchData.MatchDuration = 10;
            }
            ConfigDurationText.text = Master.Instance.MatchData.MatchDuration.ToString();
        }

        public void EPlayAudio()
        {
            selectAudio.Play();
        }

        public void EOpenWebsite()
        {
            Application.OpenURL("https://www.sheryv.tk/www");
        }
        public void EReportBug()
        {
            Application.OpenURL("https://www.sheryv.tk/www/en/ticket/bug/new");
        }
        public void ERequestFeature()
        {
            Application.OpenURL("https://www.sheryv.tk/www/en/ticket/feature/new");
        }

        public void ESetNeedShowTip()
        {
            Prefs.SetNeedShowTip();
        }

        public void EChangeVolume()
        {
            Master.Instance.Preferences.SoundVolume = SoundVolumeSlider.normalizedValue;
        }

        private void StartMatch(List<PlayerPattern> patterns)
        {
            Master.Instance.LoadPlayersPatterns(patterns);
            StartCoroutine(Master.Instance.StartMatch());
        }

        public void GeneratePrizeImages(int count, GameObject parnetHolder)
        {
            for (int j = 0; j < count; j++)
            {
                GameObject p = Instantiate(PrizeImagePrefab);
                p.transform.SetParent(parnetHolder.transform);
                p.transform.localScale = new Vector3(1f, 1f, 1f);
                p.transform.localPosition = new Vector3(j*35f, 0, 0);
            }
        }

        public static void ClearChilds(GameObject go)
        {
            for (int i = go.transform.childCount-1; i >= 0; i--)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                child.SetActive(false);
                DestroyImmediate(child);
            }
        }

        public GameObject PlayerPrefab;

        public Text PlayerNickTextSingle;
        public Text VersionLabel;
        public InputField IpField;

        public Slider SoundVolumeSlider;
        public GameObject DebugCanvas;
        public GameObject PlayerEntryLobbyPrefab;
        public GameObject PrizeImagePrefab;
        public VerticalLayoutGroup PlayerEntryLobbyListSingle;
        public VerticalLayoutGroup PlayerEntryLobbyListNet;
        public GameObject PanelOptions;
        public GameObject PanelLobbySingle;
        public GameObject PanelLobbyNet;

        public Text ConfigRoundsText;
        public Text ConfigDurationText;

        public AudioSource selectAudio;
        public GameObject MessageBoxPrefab;
        public GameObject LoadingText;
    }
}