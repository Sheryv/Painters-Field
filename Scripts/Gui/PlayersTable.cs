using System.Collections.Generic;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class PlayersTable : MonoBehaviour
    {
        [SerializeField] private Text winnerText;
        [SerializeField] private GameObject playersList;
        [SerializeField] private GameObject btnContinue;
        [SerializeField] private GameObject playerEntryPrefab;


        public void Show(List<Player> players, int winnerPlayerId)
        {
            string s = players[winnerPlayerId].Pattern.PlayerName + " " + Localization.GetLoc("ui.won");
            if (Master.Instance.MatchData.Mode == MatchMode.Tournament)
            {
                if (Master.Instance.MatchData.WasLastRound())
                {
                    s += " " + Localization.GetLoc("ui.last_round");
                }
                else
                {
                    s += " " + Localization.GetLoc("ui.round")+" "+ Master.Instance.MatchData.CurrRound;
                }
            }
            winnerText.text = s;
            MenuController.ClearChilds(playersList);
            for (int i = 0; i < players.Count; i++)
            {
                PlayerPattern pattern = players[i].Pattern;
                GameObject entry = Instantiate(playerEntryPrefab);
                PlayerEntryLobby pl = entry.GetComponent<PlayerEntryLobby>();
                entry.name = pattern.PlayerName;
                pl.PlayerName.text = pattern.PlayerName;
                pl.ColorImage.color = pattern.PlayerColor;
                pl.Pattern = pattern;
                pl.PercentText.gameObject.SetActive(true);
                pl.PercentText.text = string.Format("{0:f2}%",players[i].LastPercent);
                Wd.Log("> "+pattern.PlayerName+" | " + players[i].LastPercent+" | "+players[i].Pattern.PlayerBackgroundColor+" | ai: "+players[i].Pattern.Ai, this);
                entry.transform.SetParent(playersList.transform);
                entry.transform.localScale = new Vector3(1f, 1f, 1f);
                Master.Instance.Menu.GeneratePrizeImages(pattern.RewardsCount, pl.PrizesHolder);
            }
            btnContinue.SetActive(Master.IsHost() && !Master.Instance.MatchData.WasLastRound());
            gameObject.SetActive(true);
        }


        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}