using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class MessageBox : MonoBehaviour
    {
        public static readonly Data[,] TipLobby = new Data[2, Localization.LanguageCount]
        {
            {
                new Data("Tip", "In this mode your goal is to paint the higher percentage of the screen with your " +
                                "color than other players before the time is up. You can paint areas that other players " +
                                "painted to make them change to your color.", null)
            },
            {
                new Data("Tip", "Now use Plus button to add players to list. You can remove them by clicking X button next to " +
                                "nick on list. To configure round count and round duration use Configure button", null)
            }
        };

        private bool isShown;

        [SerializeField] private Queue<Data> queue = new Queue<Data>();
        [SerializeField] private Text contentText;
        [SerializeField] private Text titleText;
        [SerializeField] private Button okBtn;
        [SerializeField] private Button secondBtn;
        [SerializeField] private Scrollbar rect;

        private void Show()
        {
            if (!isShown && queue.Count > 0)
            {
                Data data = queue.Dequeue();
                SetUp(data);
                gameObject.SetActive(true);
                isShown = true;
            }
        }

        private void Add(string title, string content, UnityAction btnClicked)
        {
            Data data = new Data(title, content, btnClicked);
            queue.Enqueue(data);
            Show();
        }

        private void Add(Data data)
        {
            queue.Enqueue(data);
            Show();
        }

        private void SetUp(Data data)
        {
            titleText.text = data.title;
            contentText.text = data.content;
            if (data.btnClicked != null)
            {
                okBtn.onClick.AddListener(data.btnClicked);
            }
            okBtn.onClick.AddListener(Close);
            rect.value = 1;
        }

        public void Close()
        {
            gameObject.SetActive(false);
            okBtn.onClick.RemoveAllListeners();
            isShown = false;
            Show();
        }

        public static void ShowTip(int number)
        {
            MessageBox msg = Master.Instance.Menu.MessageBoxPrefab.GetComponent<MessageBox>();
            msg.Add(TipLobby[number,Localization.LocalIndex()]);
        }

        public static void AddToQueue(string title, string content, UnityAction btnClicked)
        {
//            GameObject ms = Instantiate(Master.Instance.Menu.MessageBoxPrefab);
//            ms.transform.SetParent(Master.Instance.Menu.MainPanelHolder.transform);
//            ms.transform.localScale = new Vector3(1f,1f,1f);
//            ms.transform.position = new Vector3(0,0);
            MessageBox msg = Master.Instance.Menu.MessageBoxPrefab.GetComponent<MessageBox>();
            msg.Add(title, content, btnClicked);
        }


        [Serializable]
        public class Data
        {
            public string title;
            public string content;
            public UnityAction btnClicked;

            public Data(string title, string content, UnityAction btnClicked)
            {
                this.btnClicked = btnClicked;
                this.content = content;
                this.title = title;
            }
        }
    }
}