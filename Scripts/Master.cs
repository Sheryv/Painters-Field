using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Assets.Scripts.Data;
using Assets.Scripts.Gui;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Master : MonoBehaviour
    {
        private static readonly string UpdateInfoUrl = "http://sheryv2.cc/app_dev.php/api/getmsg/";
        public static bool Debugging = true;
        public const int MaxPlayers = 4;
        public bool TestOnlyGameScene;

        public Dictionary<string, string> Loc2 = new Dictionary<string, string>();

        public static event Action<GameStates> GameStateChangedEvent;
        public static event Action TickEvent;


        public static Master Instance;
        private static GoogleAnalyticsV4 googleAnalytics;
        private static bool firstInitialized;
        public static int UsingTime;
        public Prefs Preferences;
        public GameObject GoogleAnalitycsPrefab;
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
            UsingTime = PlayerPrefs.GetInt(Prefs.UseTimeKey, 0);
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

//            Debug.Log(DateTime.UtcNow.ToLocalTime());
//            Debug.Log(DateTime.UtcNow.ToLocalTime().ToLongTimeString());
//            Debug.Log(DateTime.UtcNow.ToLocalTime().ToLongDateString());
//            Debug.Log(Data.DataRe.DateTimeToUnixSeconds(DateTime.UtcNow));
//                        string s = JsonUtility.ToJson(ServerDataModel.Generate());
//                        Debug.Log(s);
//                        Debug.Log(JsonUtility.FromJson<ServerDataModel>(js).Items[0].Title);
//            Debug.Log(JsonUtility.ToJson(new Lol() {Text = WWW.EscapeURL("<color=#0095FF>[]</color> k")}));
            Input.simulateMouseWithTouches = false;

            StartCoroutine(Tick());
            MatchData = new MatchData();
            MatchData.Mode = MatchMode.Single;
//            Thread th = new Thread(() =>
//            {
//                using (var client = new WebClient())
//                {
//                    var contents = client.DownloadString("http://www.google.com");
//                    string t;
//                    if (contents.Length > 150)
//                    {
//                        t = contents.Substring(0, 150);
//                    }
//                    else
//                        t = contents;
//                    Wd.Log("WebPage: " + t, this);
//                }
//            });
//            th.Start();
        }


        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Menu.UpdateInLobbyPlayerList();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                string s =
                    "If you use any other configuration format, you have to define your own loader class extending it from FileLoader. When the configuration values are dynamic, you can use the PHP configuration file to execute your ow" +
                    "n logic. In addition, you can define your own services to load configurations from databases or web services.Global Configuration FilesSome system administrators may prefer to store sensitive parameters in files outsid" +
                    "e the project directory.Imagine that the database credentials for your website are stored in the / etc / sites / mysite.com / parameters.yml file.Loading this file is as simple as indicating the full file path when importi" +
                    "ng it from any other configuration file:Most of the time, local developers won't have the same files that exist on the production servers. For that reason, the Config component provides the ignore_errors option to silently " +
                    "discard errors when the loaded file doesn't exist:As you've seen, there are lots of ways to organize your configuration files. You can choose one of these or even create your own custom way of organizing the files. Don't feel" +
                    " limited by the Standard Edition that comes with Symfony.For even more customization, see How to Override Symfony's default Directory Structure";

                MessageBox.Create("title sd", s, null);
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
            // string id = "UA-90173964-1";
            //GoogleAnalitycsPrefab.GetComponent<GoogleAnalyticsV4>().androidTrackingCode = id;
            googleAnalytics = Instantiate(GoogleAnalitycsPrefab).GetComponent<GoogleAnalyticsV4>();
            // googleAnalytics.androidTrackingCode = id;
            googleAnalytics.StartSession();
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
            StartCoroutine(LoadDataFromServer());
        }

        public void StartMatch()
        {
            SceneManager.LoadScene(1);
            Wd.Log("Scene loaded " + SceneManager.sceneCount, this);
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
                if (TickEvent != null)
                {
                    TickEvent.Invoke();
                }
            }
        }

        private IEnumerator LoadDataFromServer()
        {
            WWW net = new WWW(UpdateInfoUrl+Prefs.GetClientId());
            yield return net;
            if (net.error == null)
            {
                try
                {
                    ServerDataModel data = JsonUtility.FromJson<ServerDataModel>(net.text);
                    Wd.Log("Loaded data from server with id " + DataRe.ReadableStringFromUnix(data.MessageId), this);
                    OnDataServerLoaded(data);
                }
                catch (Exception ex)
                {
                    Wd.LogErr(ex.Message, this);
                }
            }
            else
            {
                Wd.LogWarning("Cannot connect and get json from " + UpdateInfoUrl, this);
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
                    MessageBox.Create(s.Title, s.Content, null);
                }
                else
                    Wd.Log("Start date is bigger than date.now | "+data, this);
            }
            Wd.Log("This is updated version, no message needed | "+data, this);
            
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
            return googleAnalytics;
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
            if (type == LogType.Error || type == LogType.Exception)
            {
                Wd.EventLogException(condition, stackTrace, type.ToString());
            }
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

    [Serializable]
    public class Lol
    {
        public string Text;
    }
}