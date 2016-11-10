using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        private const float TickTime = 1f; //seconds
        private const float Left = 0f;
        private const float Right = 19.2f;
        private const float Top = 12.6f;
        private const float Bot = 1.8f;
        private const float TimeBeforeMove = 1f;
        public const bool Debugging = true;

        public static GameController Instance;

        public bool IsPortraitMode;
        public int DrawDistance;
        public List<Player> Players;
        public GameStates GameState = GameStates.Stopped;
        public ControlTypes ControlType;
        public GuiRefs Gui;

        private NetworkStartPosition[] startPositions;
        private int frames;
        private RenderTexture texture;

        private void Awake()
        {
            if (Application.isEditor)
                ControlType = ControlTypes.Keyboard;
            else
                ControlType = ControlTypes.Touch;
            Instance = this;
            startPositions = GetComponentsInChildren<NetworkStartPosition>();
        }

        // Use this for initialization
        private void Start()
        {
            ChangeCameraView(IsPortraitMode);
            StartCoroutine(Tick());
            D(Screen.width + " | " + Screen.height);
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameState == GameStates.Playing)
            {
                //     Gui.DrawingObject.DrawColors();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause();
            }
            frames++;
        }

        private void FixedUpdate()
        {
            float mid = Screen.width/2f;
            float top = Screen.height*0.7f;
            float h = 0;
            float v = 0;
            Player player;

            if (ControlType == ControlTypes.Touch)
            {
                int count = Input.touchCount;
                for (int i = 0; i < count; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Began)
                    {
                        //left
                        if (t.position.x < mid && t.position.y < top)
                        {
                            h = -1;
                        }
                        //lright
                        if (t.position.x > mid && t.position.y < top)
                        {
                            h = 1;
                        }
                    }
                    else
                    {
                        h = 0;
                    }
                    if (t.phase == TouchPhase.Began)
                    {
                        // D("Began " + t.position);
                    }
                }
            }
            if (Players.Count > 0)
            {
                int co = Players.Count;
                for (int i = 0; i < co; i++)
                {
                    player = Players[i];
                    if (ControlType == ControlTypes.Keyboard)
                    {
                        h = Input.GetAxis("H"+i);
                       // v = Input.GetAxis("Vertical");
                    }
                    if (GameState == GameStates.Playing)
                    {
                        // Players[0].GetComponent<Rigidbody2D>().velocity = new Vector2(h * Time.fixedDeltaTime* Players[0].Speed, v*Time.fixedDeltaTime*Players[0].Speed);
                        // Players[0].transform.position = Vector3.Lerp(Players[0].transform.position, Players[0].transform.position+ new Vector3(v,h, 0f), Time.fixedDeltaTime);

                        player.transform.Rotate(new Vector3(0, 0, -h*player.RotationSpeed));
                        //Players[0].GetComponent<Rigidbody2D>().AddForce(Players[0].transform.up *  10);
                        float x = player.transform.position.x;
                        float y = player.transform.position.y;
                        Vector3 up = player.transform.up;
                        Vector3 vec = up;
                        const float off = 0.5f;

                        float vx = up.x;
                        float vy = up.y;
                        if (x < Left + off)
                        {
                            // player.transform.position.Set(Left, y,0);
                            vx = Clamp(up.x, true);
                        }
                        else if (x > Right - off)
                        {
                            //  player.transform.position.Set(Right, y, 0);
                            vx = Clamp(up.x, false);
                        }
                        if (y > Top - off)
                        {
                            vy = Clamp(up.y, false);
                            //  player.transform.position.Set(x, Top, 0);
                        }
                        else if (y < Bot + off)
                        {
                            vy = Clamp(up.y, true);
                            //  player.transform.position.Set(x, Bot, 0);
                        }
                        vec = new Vector3(vx, vy, 0);
                        player.GetComponent<Rigidbody2D>().velocity = vec*player.Speed;
                    }
                    else
                    {
                        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    }
                }
            }
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

        public void D(String s)
        {
            Gui.LogText.text = s;
            Debug.Log(s);
        }

        /* private void StartGame()
        {
            Gui.DrawingObject.Prepare();
            GameObject go = (GameObject) Instantiate(Gui.PlayerPrefab.gameObject, startPositions[0].transform.position, Quaternion.identity);
            Player p = go.GetComponent<Player>();
            Players.Add(p);
            GameState = GameStates.Playing;
            Gui.StateText.text = GameState.ToString();
        }*/

        public void PreparePlayers()
        {
            GameObject go = (GameObject) Instantiate(Gui.PlayerPrefab.gameObject, startPositions[0].transform.position, Quaternion.identity);
            Player p = go.GetComponent<Player>();
            p.PlayerColor = Color.blue;
            p.SetUp();
            Players.Add(p);
            go = (GameObject) Instantiate(Gui.PlayerPrefab.gameObject, startPositions[1].transform.position, Quaternion.identity);
            p = go.GetComponent<Player>();
            p.PlayerColor = Color.green;
            p.SetUp();
            Players.Add(p);
            
            StartCoroutine(BeginGame());
        }

        public IEnumerator BeginGame()
        {
            Gui.PanelOptions.SetActive(false);
            texture = new RenderTexture(Gui.RenderTexture.width, Gui.RenderTexture.height, Gui.RenderTexture.depth, Gui.RenderTexture.format);
            Gui.RenderCamera.targetTexture = texture;
            Gui.QuadMaterial.mainTexture = texture;
            Gui.Quad.SetActive(true);
            Gui.RenderCamera.gameObject.SetActive(true);
            yield return new WaitForSeconds(TimeBeforeMove);
            GameState = GameStates.Playing;
            Gui.StateText.text = GameState.ToString();
        }

        public void Restart()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Destroy(Players[i].gameObject);
            }
            Players.Clear();
            PreparePlayers();
          
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

        public void Pause()
        {
            if (GameState == GameStates.Paused)
            {
                GameState = GameStates.Playing;
                Gui.PanelOptions.SetActive(false);
                //Time.timeScale = 1;
            }
            else if (GameState == GameStates.Playing)
            {
                Gui.PanelOptions.SetActive(true);
                GameState = GameStates.Paused;
                // Time.timeScale = 0;
            }
            else if (GameState == GameStates.Stopped)
            {
                Gui.PanelOptions.SetActive(true);
            }
            Gui.StateText.text = GameState.ToString() + " " + frames;
        }

        private IEnumerator Tick()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                Gui.StateText.text = GameState.ToString() + " " + frames;
                frames = 0;
            }
        }

        public void Exit()
        {
            Application.Quit();
        }

        public float tt = 1f;
    }

    public enum GameStates
    {
        Stopped,
        Playing,
        Paused
    }

    public enum ControlTypes
    {
        Keyboard,
        Touch
    }
}