using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Master : MonoBehaviour
    {
        public const bool Debugging = true;
        public const int MaxPlayers = 4;
        public bool TestOnlyGameScene;

        public Dictionary<string, string> Loc2 = new Dictionary<string, string>();

        public static event Action<GameStates> GameStateChangedEvent;
        public static event Action TickEvent;


        public static Master Instance;
        public GoogleAnalyticsV4 googleAnalytics;
        private static bool firstInitialized;
        public Prefs Preferences;
        //public GameObject GoogleAnalitycsPrefab;
        public MatchData MatchData;
        public GameStates GameState = GameStates.Stopped;
        public GameMode GameMode = GameMode.Singleplayer;
        public ClientTypes ClientType = ClientTypes.Host;


        private List<PlayerPattern> playersPatterns;


        public List<PlayerPattern> PlayersPatterns
        {
            get { return playersPatterns; }
        }

        public static bool FirstInitialized
        {
            get { return firstInitialized; }
        }


        private void Awake()
        {
            if (!Application.isEditor)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            Instance = this;
            Application.logMessageReceived += ApplicationOnLogMessageReceived;
            DontDestroyOnLoad(gameObject);
            Preferences = LoadPrefs();
            Preferences.PrefsChangedEvent += PreferencesOnPrefsChangedEvent;
            GameStateChangedEvent += OnGameStateChanged;
            GameController.MatchRestartEvent += OnMatchRestart;
            GameController.MatchFinishEvent += OnMatchFinish;
            if (!firstInitialized)
            {
                firstInitialized = true;
                FirstInitialization();
            }
        }

        public void OnDisable()
        {
            Preferences.PrefsChangedEvent -= PreferencesOnPrefsChangedEvent;
            GameController.MatchRestartEvent -= OnMatchRestart;
            GameController.MatchFinishEvent -= OnMatchFinish;
        }

        private void PreferencesOnPrefsChangedEvent()
        {
            Preferences.Save();
        }

        private void OnMatchFinish(int winner)
        {
            Wd.Log("Finish | " + winner, this);
            //  GetComponent<AudioListener>().enabled = true;
        }

        private void OnMatchRestart()
        {
            GetAnalytics().DispatchHits();
            Wd.Log("Restart", this);
            // GetComponent<AudioListener>().enabled = false;
        }


        private void OnGameStateChanged(GameStates gameStates)
        {
            Wd.Log("State changed to: " + gameStates, this);

            if (gameStates == GameStates.Stopped)
            {
                if (PlayersPatterns != null)
                {
                    playersPatterns.Clear();
                    playersPatterns = null;
                }
            }
        }

        // Use this for initialization
        private void Start()
        {
            Input.simulateMouseWithTouches = false;
            if (googleAnalytics == null)
            {
                // GoogleAnalitycsPrefab.GetComponent<GoogleAnalyticsV4>().androidTrackingCode = id;
                //googleAnalytics = Instantiate(GoogleAnalitycsPrefab).GetComponent<GoogleAnalyticsV4>();
            }
            string id = "UA-90173964-1";
            googleAnalytics.androidTrackingCode = id;
            googleAnalytics.StartSession();
            StartCoroutine(Tick());
            MatchData = new MatchData();
            MatchData.Mode = MatchMode.Single;
            Thread th = new Thread(() =>
            {
                using (var client = new WebClient())
                {
                    var contents = client.DownloadString("http://www.google.com");
                    string t;
                    if (contents.Length > 150)
                    {
                        t = contents.Substring(0, 150);
                    }
                    else
                        t = contents;
                    Wd.Log("WebPage: " + t, this);
                }
            });
            th.Start();
        }

   

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Menu.UpdateInLobbyPlayerList();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                string s = "";
                foreach (KeyValuePair<string, string> kv in Loc2)
                {
                    s += "{ " + kv.Key + "," + kv.Value + "},\n";
                }
                Debug.Log(s);
            }
        }


        private void FirstInitialization()
        {
            if (Application.isMobilePlatform)
            {
                Data.Data.EventAction = "Android Action";
            }
        }

        public void StartMatch()
        {
            SceneManager.LoadScene(1);
        }

        public void LoadPlayersPatterns(List<PlayerPattern> patterns)
        {
            playersPatterns = patterns;
        }


        private IEnumerator Tick()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (TickEvent != null)
                {
                    TickEvent.Invoke();
                }
            }
        }


        private Prefs LoadPrefs()
        {
            Prefs p = Prefs.LoadPrefs();
            //todo temp controls
            if (Application.isEditor)
                p.ControlType = ControlTypes.Keyboard;
            else
                p.ControlType = ControlTypes.Touch;

            if (p.Nick == "")
            {
                p.Nick = "Noname";
            }
            return p;
        }

        public void SwitchControls()
        {
            if (Preferences.ControlType == ControlTypes.Keyboard)
            {
                Preferences.ControlType = ControlTypes.Touch;
            }
            else if (Preferences.ControlType == ControlTypes.Touch)
            {
                Preferences.ControlType = ControlTypes.Keyboard;
            }
        }

        public static GameStates GetState()
        {
            return Instance.GameState;
        }

        public static void SetState(GameStates state)
        {
            Instance.GameState = state;
            if (GameStateChangedEvent != null) GameStateChangedEvent.Invoke(state);
        }

        public static bool InGame()
        {
            return GetState() != GameStates.Stopped;
        }

        public static void Pause()
        {
            if (GetState() == GameStates.Paused)
            {
                SetState(GameStates.Playing);
            }
            else if (GetState() == GameStates.Playing)
            {
                SetState(GameStates.Paused);
            }
        }

        public static bool IsHost()
        {
            return Instance.ClientType == ClientTypes.Host;
        }

        public static bool IsSinglePlayer()
        {
            return Instance.GameMode == GameMode.Singleplayer;
        }

        public static GoogleAnalyticsV4 GetAnalytics()
        {
            return Instance.googleAnalytics;
        }

        public static void Exit()
        {
            GetAnalytics().StopSession();
            GetAnalytics().DispatchHits();
            GetAnalytics().Dispose();
            Application.Quit();
        }

        private void ApplicationOnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Wd.ConsoleLog(condition, stackTrace, type);
        }

        private void OnDestroy()
        {
            GetAnalytics().DispatchHits();
            GetAnalytics().Dispose();
        }


        [NonSerialized] public GuiRefs GuiGame;
        public MenuController Menu;
    }


    public enum GameMode
    {
        Singleplayer,
        Network
    }

    public enum ClientTypes
    {
        Host = Player.SourceServer,
        Remote = Player.SourceClient
    }

    public enum GameStates
    {
        Stopped,
        Playing,
        Paused,
        Finished,
        Restarting
    }

    public enum ControlTypes
    {
        Keyboard,
        Touch
    }
}