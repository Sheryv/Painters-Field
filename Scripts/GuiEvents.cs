using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GuiEvents : MonoBehaviour
    {
        private GameController gc = null;
        public List<GameObject> DestroyedObjectsOnRestart = new List<GameObject>(); 
        // Use this for initialization
        void Awake()
        {
            gc = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            DestroyedObjectsOnRestart.Add(Master.Instance.gameObject);
        }

        // Update is called once per frame
        void Start()
        {
            if (Master.Debugging && Application.isEditor)
            {
                gc.Gui.SwitchControlDebugBtn.SetActive(true);
            }
            if (Master.Debugging)
            {
            gc.Gui.ObjectsToDisableBeforeCalc.Add(LoggingPanel.Instance.gameObject);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Master.Pause();
            }
            gc.Gui.MatchTimeLabel.text = string.Format("{0:0.0}", gc.MatchTime);
        }

        public void TogglePause()
        {
            Master.Pause();
        }

        public void OnEnable()
        {
            GameController.MatchJustPreparedEvent += OnMatchJustPreparedEvent;
            GameController.MatchFinishEvent += OnMatchFinishEvent;
        }

        public void OnDisable()
        {
            GameController.MatchJustPreparedEvent -= OnMatchJustPreparedEvent;
            GameController.MatchFinishEvent -= OnMatchFinishEvent;
        }

        private void OnMatchFinishEvent(int winnerIndex)
        {
            ShowTable(winnerIndex);
        }

        private void OnMatchJustPreparedEvent(List<Player> players)
        {
            UpdateInGamePanel();
        }

        public void UpdateInGamePanel()
        {
            Transform list = gc.Gui.InGamePlayersPanel.transform.GetChild(0);
            List<Player> players = gc.GetPlayers();
                Player p;
                Transform item;
                    Text text;
            Image image;
            for (int i = 0; i < list.childCount; i++)
            {
                item = list.GetChild(i);
                if (players.Count > i)
                {
                    item.gameObject.SetActive(true);
                    p = players[i];
                    image = item.GetComponentInChildren<Image>();
                    text = item.GetComponent<Text>();
                    image.color = p.Pattern.PlayerColor;
                    text.text = p.Pattern.PlayerName;
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void ELobbyOpened()
        {
            
        }

        public void ELobbyHost()
        {
            
        }

        public void ELobbyConnectClient()
        {
            
        }

        public void EExit()
        {
            Master.Exit();
        }

        public void ESwitchControls()
        {
            Master.Instance.SwitchControls();
        }

        public void EPlayStart()
        {
            gc.Restart(0);
           // gc.StartCoroutine(StartFirst(1,0));
        }

        public void EPlayAudio()
        {
            Master.Instance.Menu.EPlayAudio();
        }

        private IEnumerator StartFirst(int normal, int ai)
        {
            yield return new WaitForSeconds(0.1f);
            
          //  gc.Restart();
           // yield return new WaitForSeconds(0.05f);
            //gc.Restart();
        }


        public void ShowTable(int winner)
        {
            PlayersTable table = gc.Gui.PlayersTable.GetComponent<PlayersTable>();
            table.Show(gc.GetPlayers(), winner);
        }


        public void ELoadMenu()
        {
            for (int i = 0; i < DestroyedObjectsOnRestart.Count; i++)
            {
                Destroy(DestroyedObjectsOnRestart[i]);
            }
            SceneManager.LoadScene(0);
        }

        public void ELoadLobby()
        {
            gc.Restart(0);
            gc.Gui.PlayersTable.SetActive(false);
        }
    }
}
