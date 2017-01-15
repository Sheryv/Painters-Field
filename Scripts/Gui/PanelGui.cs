using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gui
{
    public class PanelGui : MonoBehaviour
    {
        //   private Animator animator;
        public bool ShouldSavePrefsOnClose;

        public void DeacitvatePanel()
        {
            gameObject.SetActive(false);
        }

        public void ClosePanel()
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger(MenuController.DeactivateTrigger);
            if (ShouldSavePrefsOnClose)
            {
            Master.Instance.Preferences.Save();
            }
        }

        public void CloseActive()
        {
            Animator animator = Master.Instance.Menu.ActivePanel.GetComponent<Animator>();
            animator.SetTrigger(MenuController.DeactivateTrigger);
        }

        public void ShowPrevious()
        {
            Master.Instance.Menu.PreviousPanel.gameObject.SetActive(true);
        }

        public void OnEnable()
        {
            Master.Instance.Menu.ActivePanel = this;
        }

        public void Show()
        {
            Master.Instance.Menu.PreviousPanel = Master.Instance.Menu.ActivePanel;
            gameObject.SetActive(true);
        }

        public void LobbyOpened(bool isSinglePlayer)
        {
            gameObject.SetActive(true);
        }

        public void OptionsOpened(bool outOfGame)
        {
            gameObject.SetActive(true);
        }

        public void ChangeNickOpened(GameObject input)
        {
            InputField field = input.GetComponent<InputField>();
            field.text = Master.Instance.Preferences.Nick;
            Show();
        }
    }
}
