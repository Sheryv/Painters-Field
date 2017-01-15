using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class PlayerEntryLobby : MonoBehaviour
    {
        [SerializeField]
        private Button btn;
        [SerializeField]
        private Text playerName;

        public Button Btn
        {
            get { return btn; }
        }

        public Text PlayerName
        {
            get { return playerName; }
        }

        public Image ColorImage
        {
            get { return colorImage; }
        }

        public GameObject PrizesHolder
        {
            get { return prizesHolder; }
        }

        public Text PercentText
        {
            get { return percentText; }
        }

        [SerializeField]
        private Image colorImage;
        [SerializeField]
        private GameObject prizesHolder;
        [SerializeField]
        private Text percentText;
       // private int id;
        private PlayerPattern pattern;

//        public int MessageId
//        {
//            get { return id; }
//        }

        public PlayerPattern Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }

        // Use this for initialization
        void Start()
        {
//            btn = GetComponentInChildren<Button>();
//            playerName = GetComponentInChildren<Text>();
//            colorImage = GetComponentInChildren<Image>();
            if (btn != null)
            {
            btn.onClick.AddListener(RemovePattern);
            btn.onClick.AddListener(Master.Instance.Menu.EPlayAudio);
            }
        }

        private void RemovePattern()
        {
            Master.Instance.Menu.ERemovePattern(this);
        }

        public void SetUp( PlayerPattern pattern)
        {
            this.playerName.text = pattern.PlayerName;
            this.colorImage.color = pattern.PlayerColor;
            this.pattern = pattern;
            if (Master.IsHost() && pattern.SourceType != Player.SourceServer)
            {
                btn.gameObject.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
