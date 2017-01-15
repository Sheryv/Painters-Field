using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class MessageBox : MonoBehaviour
    {

        [SerializeField] private Text contentText;
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Button okBtn;
        [SerializeField]
        private Button secondBtn;
        [SerializeField]
        private Scrollbar rect;
        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        public void SetUp(string title, string content, UnityAction btnClicked)
        {
            titleText.text = title;
            contentText.text = content;
            if (btnClicked != null)
            {
                okBtn.onClick.AddListener(btnClicked);
            }
            else
            {
                okBtn.onClick.AddListener(Close);
            }
            gameObject.SetActive(true);
            rect.value = 1;
        }

        public void Close()
        {
           gameObject.SetActive(false);
        }

        public static void Create(string title, string content, UnityAction btnClicked)
        {
//            GameObject ms = Instantiate(Master.Instance.Menu.MessageBoxPrefab);
//            ms.transform.SetParent(Master.Instance.Menu.MainPanelHolder.transform);
//            ms.transform.localScale = new Vector3(1f,1f,1f);
//            ms.transform.position = new Vector3(0,0);
            MessageBox msg = Master.Instance.Menu.MessageBoxPrefab.GetComponent<MessageBox>();
            msg.SetUp(title, content, btnClicked);
           
        }
    }
}
