﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class LoggingPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text text;
        [SerializeField] private Scrollbar scroll;

        public static LoggingPanel Instance;


        private const float Top = -431f;
        private const float Bot = 1410f;

        // Use this for initialization
        private void Awake()
        {
            Instance = this;
        }

        public void AddText(string message)
        {
            text.text += message + "\n";
            if (text.text.Length > 3000)
            {
                text.text = text.text.Substring(200, text.text.Length - 201);
            }
        }

        public void ToggleVisibility()
        {
            panel.SetActive(!panel.activeInHierarchy);
            scroll.gameObject.SetActive(!scroll.gameObject.activeInHierarchy);
        }

        public void OnScroll(Scrollbar sc)
        {
            float dif = Bot - Top;
            dif = sc.value*dif;
            dif = Bot - dif;
            text.transform.localPosition = new Vector3(text.transform.localPosition.x, dif, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Master.Debugging)
            {
                if (Input.GetKeyDown(KeyCode.Menu))
                {
                    ToggleVisibility();
                }
            }
        }
    }
}