using System;
using System.Collections.Generic;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class ChangeColorPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject grid;
        [SerializeField]
        private GameObject colorPrefab;

        public void ChangeColor(GameObject btn)
        {
            PlayerPattern.GetLocalPlayerPattern().PlayerColor = btn.GetComponent<Image>().color;
            PlayerPattern.GetLocalPlayerPattern().PlayerBackgroundColor = PlayerPattern.ColorsBackground[Int32.Parse(btn.name)];
            Master.Instance.Menu.UpdateInLobbyPlayerList();
            Master.Instance.Menu.ToggleColorPanel(this.gameObject);
        }

        public void OnEnable()
        {
            List<int> ids = Master.Instance.Menu.GetLeftColorIds();
            for (int i = 0; i < ids.Count; i++)
            {
                GameObject go = Instantiate(colorPrefab);
                go.transform.SetParent(grid.transform);
                go.transform.localScale = new Vector3(1f, 1f, 1f);
                Button b = go.GetComponent<Button>();
                b.onClick.AddListener(delegate { ChangeColor(go); });
                b.onClick.AddListener( Master.Instance.Menu.EPlayAudio);
                Image im = go.GetComponent<Image>();
                im.color = PlayerPattern.ColorsBasic[ids[i]];
                go.name = ids[i].ToString();
            }

        }

        private void Call()
        {
            throw new NotImplementedException();
        }

        public void OnDisable()
        {
            foreach (Transform transform1 in grid.transform)
            {
                Destroy(transform1.gameObject);
            }
        }
    }
}
