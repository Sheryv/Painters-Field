using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Gui;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// Parameter is winner index in list
        /// </summary>
        public static event Action<int> MatchFinishEvent;

        public static event Action<List<Player>> MatchJustPreparedEvent;
        public static event Action MatchBeginEvent;
        public static event Action MatchRestartEvent;


        private const float TickTime = 1f; //seconds
        private const float Left = 0f;
        private const float Right = 19.2f;
        private const float Top = 12.6f;
        private const float Bot = 1.8f;
        private const byte ColorCompareThreshold = 20;
        private const float TimeBeforeMove = 1f;
        [NonSerialized] public static readonly Point TextureSize = new Point(1920, 1080);
        //  public static GameController Instance;

        public GuiRefs Gui;
        public float MatchTime;
        public AnimationCurve AiTime;
        public AnimationCurve AiDirection;

        private AudioSource audioSource;
        [SerializeField] private List<Player> players;
        private NetworkStartPosition[] startPositions;
        private int frames;

        private void Awake()
        {
            startPositions = GetComponentsInChildren<NetworkStartPosition>();
            players = new List<Player>(4);
            Master.Instance.GuiGame = Gui;
            MatchTime = Master.Instance.MatchData.MatchDuration;
        }

        // Use this for initialization
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            Master.SetState(GameStates.Paused);
            //  ChangeCameraView(IsPortraitMode);
            Wd.Log(Screen.width + " | " + Screen.height, this);

            //todo test
            if (Master.Instance.TestOnlyGameScene)
            {
                List<PlayerPattern> patterns = new List<PlayerPattern>();
                patterns.Add(new PlayerPattern(0, "pl_1", Player.SourceServer));
                Master.Instance.LoadPlayersPatterns(patterns);
            }
            if (Master.Instance.GameMode == GameMode.Singleplayer)
            {
                Master.Instance.ClientType = ClientTypes.Host;
            }
            Restart(0);
        }

        public void OnEnable()
        {
            Master.GameStateChangedEvent += GameStateChanged;
//            Master.TickEvent += Tick;
        }

        //todo temporary disabled
        private void Tick()
        {
            frames = 0;
        }

        public void OnDisable()
        {
            Master.GameStateChangedEvent -= GameStateChanged;
//            Master.TickEvent -= Tick;
            GetComponent<AudioSource>().Stop();
        }


        // Update is called once per frame
        private void Update()
        {
            if (Master.GetState() == GameStates.Playing)
            {
                MatchTime -= Time.deltaTime;
            }


//            if (Input.GetKeyDown(KeyCode.P))
//            {
//                Master.Pause();
//            }

            if (MatchTime <= 0)
            {
                MatchTime = Master.Instance.MatchData.MatchDuration;
                FinishMatch();
            }

            frames++;
        }

        public void PreparePlayers(float delay)
        {
            GameObject go;
            Player p;
            List<PlayerPattern> patterns = Master.Instance.PlayersPatterns;
            for (int i = 0; i < patterns.Count; i++)
            {
                go = (GameObject) Instantiate(patterns[i].GetPrefab(), startPositions[i].transform.position, startPositions[i].transform.localRotation);
                p = go.GetComponent<Player>();
                p.SetUp(patterns[i], i);
                players.Add(p);
            }
            if (MatchJustPreparedEvent != null) MatchJustPreparedEvent(players);
            StartCoroutine(BeginGame(delay));
            Master.Instance.MatchData.CurrRound++;
        }

        public IEnumerator BeginGame(float delay)
        {
            RenderTexture texture = Master.Instance.RenderTexture;
            Gui.FinishText.gameObject.SetActive(false);
            Gui.PanelMain.SetActive(false);
           // texture.DiscardContents();
            Gui.RenderCamera.targetTexture = texture;
            Gui.QuadMaterial.mainTexture = texture;
            Gui.DisplayCreatedTextureQuad.SetActive(true);
            Gui.RenderCamera.gameObject.SetActive(true);
            Gui.ResetBufferQuad.SetActive(true);
            yield return new WaitForSeconds(0.02f);
            Gui.ResetBufferQuad.SetActive(false);
//            for (int i = 0; i < TimeBeforeMove; i++)
            {
                Gui.MatchStartAudio.Play();
                yield return new WaitForSeconds(1f);
            }
            audioSource.clip = Gui.BackgroundClips[UnityEngine.Random.Range(0, Gui.BackgroundClips.Count)];
            audioSource.Play();
            Wd.EventLogGamePlayInfo("Match Start - Players Count " + players.Count + " | " + Master.Instance.MatchData.RoundsCount, players.Count);
            Master.SetState(GameStates.Playing);
            if (MatchBeginEvent != null) MatchBeginEvent();
            MatchTime = Master.Instance.MatchData.MatchDuration;
        }

        public void Restart(float delay)
        {
            if (MatchRestartEvent != null) MatchRestartEvent();
            audioSource.Stop();

            for (int i = 0; i < players.Count; i++)
            {
                Destroy(players[i].gameObject);
            }
            players.Clear();
            PreparePlayers(delay);
        }

        public void FinishMatch()
        {
            Master.SetState(GameStates.Finished);
            Gui.MatchEndAudio.Play();
            Gui.FinishText.gameObject.SetActive(true);
            //calc percent
            StartCoroutine(CalculatePercents());
        }

        public float Clamp(float x, bool left)
        {
            if (left)
            {
                return x < 0 ? 0 : x;
            }
            else
            {
                return x > 0 ? 0 : x;
            }
        }

        public void GameStateChanged(GameStates state)
        {
            if (state == GameStates.Paused)
            {
                Gui.PanelMain.SetActive(true);
                audioSource.Pause();
            }
            else if (state == GameStates.Playing)
            {
                Gui.PanelMain.SetActive(false);
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                }
            }
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

        private void ChangeCameraView(bool portraitMode)
        {
            if (portraitMode)
            {
                Gui.LandscapeCamera.orthographicSize = 9.6f;
                Transform t = Gui.LandscapeCamera.gameObject.transform;
                //Gui.LandscapeCamera.gameObject.SetActive(false);
                //Gui.PortraitCamera.gameObject.SetActive(true);
                t.position = new Vector3(7.2f, 9.6f, -10f);
                for (int i = 0; i < t.childCount; i++)
                {
                    t.GetChild(i).localPosition = new Vector3(7.2f, -9.6f, 10f);
                    t.GetChild(i).Rotate(new Vector3(0, 0, 90f));
                }
            }
        }

        [SerializeField] private Texture2D tex;

        private IEnumerator CalculatePercents()
        {
            for (int i = 0; i < Gui.ObjectsToDisableBeforeCalc.Count; i++)
            {
                Gui.ObjectsToDisableBeforeCalc[i].SetActive(false);
            }
            for (int i = 0; i < GetPlayers().Count; i++)
            {
                GetPlayers()[i].gameObject.SetActive(false);
            }
            yield return null;

            tex = new Texture2D(TextureSize.X, TextureSize.Y, TextureFormat.RGB24, false);
            RenderTexture.active = Master.Instance.RenderTexture;
            Gui.RenderCamera.Render();
            //    yield return new WaitForEndOfFrame();
            tex.ReadPixels(TextureSize.GetRect(), 0, 0);
            // tex.ReadPixels(new Rect(1920.0f/2f-20f, 1080.0f/2f-20f, 40f, 40f), 0, 0);
            tex.Apply();
            Wd.Log("Texture saved", this);

            yield return null;
            for (int i = 0; i < Gui.ObjectsToDisableBeforeCalc.Count; i++)
            {
                Gui.ObjectsToDisableBeforeCalc[i].SetActive(true);
            }
            yield return null;

//            Color32[] pixs = tex.GetPixels32();
            Color32[] pixs = tex.GetPixels32();
            int[] points = new int[GetPlayers().Count];
            for (int i = 0; i < pixs.Length; i++)
            {
                Color32 c = pixs[i];
                int ind = GetPlayerIndexWithColor(c);
                if (ind >= 0)
                {
                    points[ind]++;
                }
            }
            yield return null;

            int sum = 0;
            for (int i = 0; i < points.Length; i++)
            {
                sum += points[i];
            }
            Wd.Log("pixels: " + pixs.Length + " | sum: " + sum, this);
            int winnerIndex = 0;
            float winner = points[0];
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] > winner)
                {
                    winner = points[i];
                    winnerIndex = i;
                }
                Player p = GetPlayers()[i];
                p.LastPercent = points[i]/((float) sum)*100f;
            }
            GetPlayers()[winnerIndex].Pattern.RewardsCount++;
            if (MatchFinishEvent != null) MatchFinishEvent.Invoke(winnerIndex);

            Wd.EventLogGamePlayInfo("Match Finish - Winner", winnerIndex);
        }

        private int GetPlayerIndexWithColor(Color32 pixel)
        {
            for (int i = 0; i < GetPlayers().Count; i++)
            {
                if (CompareApproxColors32(GetPlayers()[i].Pattern.PlayerBackgroundColor, pixel))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool CompareApproxColors32(Color32 c1, Color32 c2)
        {
            if (c1.r < c2.r + ColorCompareThreshold && c1.r > c2.r - ColorCompareThreshold)
            {
                if (c1.g < c2.g + ColorCompareThreshold && c1.g > c2.g - ColorCompareThreshold)
                {
                    if (c1.b < c2.b + ColorCompareThreshold && c1.b > c2.b - ColorCompareThreshold)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}