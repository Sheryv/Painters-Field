using UnityEngine;

namespace Assets.Scripts
{
    public class TestBehav : MonoBehaviour
    {
        [SerializeField]
        private RenderTexture rtex;
        [SerializeField]
        private Texture tex;
        [SerializeField]
        private Material mat;
        [SerializeField]
        private Material displayingMat;

        void Start()
        {
           // rtex = new RenderTexture(gc.Gui.RenderTexture.width, gc.Gui.RenderTexture.height, gc.Gui.RenderTexture.depth);
            displayingMat.mainTexture = rtex;
        }

        void Update()
        {
            //aby malowac ustawic jako source rendertex
            rtex.DiscardContents();
            Graphics.Blit(tex, rtex, mat);
        }
    }
}
