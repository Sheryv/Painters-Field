using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class GuiRefs
    {
        public DrawPixels DrawingObject;
        public Camera LandscapeCamera;
        public Camera RenderCamera;
        public GameObject DisplayCreatedTextureQuad;
        public GameObject ResetBufferQuad;
       // public RenderTexture RenderTexture;
        public Material QuadMaterial;
        public RectTransform LeftRect;
        public RectTransform RightRect;
        public Text StateText;
        public Text LogText;
        public Text FinishText;
        public Text ControlsLabel;
        public Text MatchTimeLabel;
        public GameObject PanelMain;
        public GameObject PanelOptions;
        public GameObject InGamePlayersPanel;
        public GameObject PlayersTable;
        public List<GameObject> ObjectsToDisableBeforeCalc;
        public List<AudioClip> BackgroundClips;
        public AudioSource MatchStartAudio;
        public AudioSource MatchEndAudio;
        public AudioSource PowerUpPickAudio;
        public AudioSource PowerUpEndAudio;
    }
}
