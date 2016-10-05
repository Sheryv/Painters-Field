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
        public const bool Debugging = true;

        public static GameController Instance;

        public bool IsPortraitMode;
        public int DrawDistance;
        public List<Player> Players;
        public GameStates GameState = GameStates.Stopped;
        public GuiRefs Gui;

        private NetworkStartPosition[] startPositions;
        private int frames;
        private void Awake()
        {
            Instance = this;
            startPositions = GetComponentsInChildren<NetworkStartPosition>();

        }

        // Use this for initialization
        private void Start()
        {
            ChangeCameraView(IsPortraitMode);
            StartCoroutine(Tick());
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
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (GameState == GameStates.Paused)
                {
                    GameState = GameStates.Playing;
                    Time.timeScale = 1;
                }
                else if (GameState == GameStates.Playing)
                {
                    Time.timeScale = 0;
                    GameState = GameStates.Paused;
                }
            }
            frames++;
        }

        private void FixedUpdate()
        {
            float h = Input.GetAxis("Horizontal");

            
            float v = Input.GetAxis("Vertical");
            Player player;
            if (Players.Count>0)
            {
                player = Players[0];
                // Players[0].GetComponent<Rigidbody2D>().velocity = new Vector2(h * Time.fixedDeltaTime* Players[0].Speed, v*Time.fixedDeltaTime*Players[0].Speed);
                // Players[0].transform.position = Vector3.Lerp(Players[0].transform.position, Players[0].transform.position+ new Vector3(v,h, 0f), Time.fixedDeltaTime);
                player.transform.Rotate(new Vector3(0,0, -h* player.RotationSpeed));
                //Players[0].GetComponent<Rigidbody2D>().AddForce(Players[0].transform.up *  10);
                player.GetComponent<Rigidbody2D>().velocity = player.transform.up * player.Speed;
            }
                
        }

        private void StartGame()
        {
            Gui.DrawingObject.Prepare();
            GameObject go = (GameObject) Instantiate(Gui.PlayerPrefab.gameObject, startPositions[0].transform.position, Quaternion.identity);
            Player p = go.GetComponent<Player>();
            Players.Add(p);
            GameState = GameStates.Playing;
            Gui.StateText.text = GameState.ToString();
        }

        public void BeginGame()
        {
            GameObject go = (GameObject)Instantiate(Gui.PlayerPrefab.gameObject, startPositions[0].transform.position, Quaternion.identity);
            Player p = go.GetComponent<Player>();
            Players.Add(p);
            GameState = GameStates.Playing;
            Gui.StateText.text = GameState.ToString();

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
                    t.GetChild(i).localPosition= new Vector3(7.2f, -9.6f, 10f);
                    t.GetChild(i).Rotate(new Vector3(0,0,90f));
                }
            }
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

        public float tt = 1f;
    }

    public enum GameStates
    {
        Stopped,
        Playing,
        Paused
    }
}