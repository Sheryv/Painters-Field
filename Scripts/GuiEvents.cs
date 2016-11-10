using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class GuiEvents : MonoBehaviour
    {
        private GameController gc;
        // Use this for initialization
        void Start()
        {
            gc= GameController.Instance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EExit()
        {
            gc.Exit();
        }

        public void EPlayStart()
        {
            gc.StartCoroutine(StartFirst());
        }

        private IEnumerator StartFirst()
        {
            yield return new WaitForSeconds(0.1f);
            gc.Restart();
          //  gc.Restart();
           // yield return new WaitForSeconds(0.05f);
            //gc.Restart();
        }
    }
}
