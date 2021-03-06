﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Data;
using Assets.Scripts.Gui;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Master : MonoBehaviour
    {
        public static readonly bool Debugging = false;
        public const int MaxPlayers = 4;
        public bool TestOnlyGameScene;

        public Dictionary<string, string> Loc2 = new Dictionary<string, string>();

        public static event Action<GameStates> GameStateChangedEvent;
        private static event Action TickEvent;


        public static Master Instance;
        public static int UsingTime;
        private static GoogleAnalyticsV4 googleAnalytics;
        private static bool firstInitialized;


        public Prefs Preferences;
        public GameObject GoogleAnalitycsPrefab;
        public MatchData MatchData;
        public GameStates GameState = GameStates.Stopped;
        public GameMode GameMode = GameMode.Singleplayer;
        public ClientTypes ClientType = ClientTypes.Host;


        private Thread ThreadTexture;
        [SerializeField] private RenderTexture renderTexture = null;
        private List<PlayerPattern> playersPatterns;


        public List<PlayerPattern> PlayersPatterns
        {
            get { return playersPatterns; }
        }

        public static bool FirstInitialized
        {
            get { return firstInitialized; }
        }

        public RenderTexture RenderTexture
        {
            get { return renderTexture; }
        }


        private void Awake()
        {
            if (!Debugging)
            {
                Debug.logger.filterLogType = LogType.Error;
            }
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
            UsingTime = PlayerPrefs.GetInt(Prefs.UseTimeKey, 0);
            StartCoroutine(CreateRenderTexture());
        }


        public void OnDisable()
        {
            Preferences.PrefsChangedEvent -= PreferencesOnPrefsChangedEvent;
            GameController.MatchRestartEvent -= OnMatchRestart;
            GameController.MatchFinishEvent -= OnMatchFinish;
        }

        private void PreferencesOnPrefsChangedEvent()
        {
        }

        private void OnMatchFinish(int winner)
        {
//            Wd.Log("Finish | " + winner, this);
        }

        private void OnMatchRestart()
        {
            GetAnalytics().DispatchHits();
//            Wd.Log("Restart", this);
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
//            Debug.Log(Data.DataRe.DateTimeToUnixSeconds(DateTime.UtcNow));
            Input.simulateMouseWithTouches = false;
            StartCoroutine(Tick());
            MatchData = new MatchData();
            MatchData.Mode = MatchMode.Single;
        }


        // Update is called once per frame
        private void Update()
        {
            if (Debugging)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    string s =
                        "If you use any other configuration format, you have to define your own loader class extending it from FileLoader. When the configuration values are dynamic, you can use the PHP configuration file to execute your ow" +
                        "n logic. In addition, you can define your own services to load configurations from databases or web services.Global Configuration FilesSome system administrators may prefer to store sensitive parameters in files outsid";
                    MessageBox.AddToQueue("title " + UnityEngine.Random.Range(0, 10000), s, null);
                }
            }
        }


        private void FirstInitialization()
        {
            TextAsset asset = Resources.Load<TextAsset>("params");
            if (asset != null)
            {
                Params p = JsonUtility.FromJson<Params>(asset.text);
                if (p != null)
                {
                    googleAnalytics = Instantiate(GoogleAnalitycsPrefab).GetComponent<GoogleAnalyticsV4>();
                    googleAnalytics.androidTrackingCode = p.GoogleAnalitycsUAID;
                    googleAnalytics.bundleVersion = Application.version;
                    googleAnalytics.SetUp();
                    googleAnalytics.StartSession();
                    StartCoroutine(LoadDataFromServer(p.UpdateMessageApiAddress));
                }
                else
                    Wd.Log("Null Params from json", this);
            }
            else
                Wd.Log("Null asset", this);
            int l = 0;
            if (PlayerPrefs.HasKey(Prefs.StartCountKey))
            {
                l = PlayerPrefs.GetInt(Prefs.StartCountKey);
            }
            l++;
            PlayerPrefs.SetInt(Prefs.StartCountKey, l);
            int time = 0;
            if (PlayerPrefs.HasKey(Prefs.UseTimeKey))
            {
                time = PlayerPrefs.GetInt(Prefs.UseTimeKey);
            }
            Wd.EventLogState("ApplicationStarted", "Start Num: " + l, l);
            Wd.EventLogState("ApplicationStarted", "Use Time: " + time, time);
        }

        public IEnumerator StartMatch()
        {
            Menu.LoadingText.SetActive(true);
            for (int i = 0; i < 100; i++)
            {
                if (RenderTexture != null)
                {
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (RenderTexture != null)
            {
                SceneManager.LoadSceneAsync(1);
                Wd.Log("Scene loading " + SceneManager.sceneCount, this);
            }
            else
            {
                Menu.LoadingText.SetActive(false);
                Wd.Log("Unable to get render texture", this);
            }
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
                UsingTime++;
                //todo temporary disabled
                if (TickEvent != null)
                {
                    TickEvent.Invoke();
                }
            }
        }

        private IEnumerator LoadDataFromServer(string url)
        {
            WWW net = new WWW(url + Prefs.GetClientId());
            yield return net;
            if (net.error == null)
            {
                try
                {
                    string json = ServerDataHolder.WrapToClass(net.text);
                    ServerDataHolder data = JsonUtility.FromJson<ServerDataHolder>(json);
                    if (data.Messages.Count > 0)
                    {
                        Wd.Log("Loaded data from server with id " + DataRe.ReadableStringFromUnix(data.Messages[0].MessageId), this);
                        OnDataServerLoaded(data.Messages[0]);
                    }
                    else
                        Wd.LogWarning("Array of messages is empty " + url, this);
                }
                catch (Exception ex)
                {
                    Wd.LogErr(ex.Message, this);
                }
            }
            else
            {
                Wd.LogWarning("Cannot connect and get json from " + url, this);
            }
        }

        private void OnDataServerLoaded(ServerDataModel data)
        {
            int[] app = DataRe.SplitVersionName(Application.version);
            int[] update = DataRe.SplitVersionName(data.Version);

            if (app[0] < update[0] || (app[0] == update[0] && app[1] < update[1]))
            {
                if (DataRe.UnixTimeToDateTime(data.StartDate) < DateTime.Now)
                {
                    ServerDataModelLanguage s = data.GetWithLocal(Localization.LocalCode());
                    MessageBox.AddToQueue(s.Title, s.Content, null);
                }
                else
                    Wd.Log("Start date is bigger than date.now | " + data, this);
            }
            Wd.Log("This is updated version, no message needed | " + data, this);
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
                p.Nick = "Your Nick";
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

        public IEnumerator CreateRenderTexture()
        {
            if (renderTexture == null)
            {
//                Wd.Log("creating tex", this);
                yield return null;
                renderTexture = new RenderTexture(GameController.TextureSize.X, GameController.TextureSize.Y, 24);
                yield return null;
//                Wd.Log("after creating tex", this);
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
            return googleAnalytics;
        }

        public static void Exit()
        {
            if (Master.GetAnalytics() != null)
            {
                GetAnalytics().StopSession();
                GetAnalytics().DispatchHits();
            }
            Application.Quit();
        }

        private void ApplicationOnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                Wd.EventLogException(condition, stackTrace, type.ToString());
            }
            Wd.ConsoleLog(condition, stackTrace, type);
        }

        private void OnDestroy()
        {
            if (Master.GetAnalytics() != null)
            {
                GetAnalytics().Dispose();
            }
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