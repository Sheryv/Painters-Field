using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class GuiRefs
    {
        public Player PlayerPrefab;
        public DrawPixels DrawingObject;
        public Camera LandscapeCamera;
        public Camera RenderCamera;
        public GameObject Quad;
        public RenderTexture RenderTexture;
        public Material QuadMaterial;
        public Text StateText;
        public Text LogText;
        public RectTransform LeftRect;
        public RectTransform RightRect;
        public GameObject PanelOptions;
    }
}
