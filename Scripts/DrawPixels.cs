using System.Collections.Generic;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts
{
    public class DrawPixels : MonoBehaviour
    {
        [SerializeField] private List<Point> points;
        [SerializeField] private Color[] colors;
        private Sprite sprite;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private SpriteRenderer fieldRenderer;
        [SerializeField] private RenderTexture canvasTexture;
        [SerializeField] private Material material;
        [SerializeField]private int drawDistance;
        // [SerializeField] public Color Color;
        private float distanceSquare;
        private Texture2D tempTexture2D;
        private GameController gc;
        // Use this for initialization
        private void Start()
        {
            gc = GameObject.FindWithTag("GameCotroller").GetComponent<GameController>();
            points = new List<Point>();
        }

        public void Prepare()
        {
            CreateClean();
            distanceSquare = drawDistance*drawDistance;
            SetPointsTemplateArray();
        }

        private void CreateClean()
        {
            Color c = new Color(1f, 1f, 1f, 0.01f);
            Texture2D bg = background.sprite.texture;
            tempTexture2D = new Texture2D(bg.width, bg.height);//, TextureFormat.DXT5, false);
            for (int i = 0; i < tempTexture2D.height; i++)
            {
                for (int j = 0; j < tempTexture2D.width; j++)
                {
                    tempTexture2D.SetPixel(j, i, c);
                }
            }
            tempTexture2D.wrapMode = TextureWrapMode.Clamp;
            tempTexture2D.anisoLevel = 4;
            tempTexture2D.filterMode = FilterMode.Bilinear;
            sprite = fieldRenderer.sprite = Sprite.Create(tempTexture2D, Rect.MinMaxRect(0, 0, tempTexture2D.width, tempTexture2D.height), new Vector2());
        }


        // Update is called once per frame
        private void Update()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Master.Debugging)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    if(Master.GetState() == GameStates.Playing)
                        DrawColors();
                }
            }
        }

        public void DrawColors()
        {
            Player player;
            Texture2D tex = sprite.texture;
            for (int pl = 0; pl < gc.GetPlayers().Count; pl++)
            {
                int plY;
                int plX;
                player = gc.GetPlayers()[pl];
//                if (gc.IsPortraitMode)
//                {
//                    plY = (int) (player.transform.localPosition.x*100);
//                    plX = (int) (player.transform.localPosition.y*100);
//                    plY = tex.height - plY;
//                }
//                else
                {
                    plX = (int) (player.transform.localPosition.x*100);
                    plY = (int) (player.transform.localPosition.y*100);
                }
                //     Debug.Log("Drawing player " + pl + " | px: " + plX + ", " + plY);
                Point point;
                int count = points.Count;
                for (int i = 0; i < count; i++)
                {
                    point = points[i];
                    int x = point.X + plX;
                    int y = point.Y + plY;
                    //     if (tex.GetPixel(x, y) != player.PlayerColor)
                    tex.SetPixel(x, y, player.Pattern.PlayerColor);
                }
                /*               for (int i = 0; i < tex.height; i++)
                {
                    int height = tex.height - i;
                    for (int width = 0; width < tex.width; width++)
                    {
                        float d = DistanceSquare(x, y, width, height);
                        if (d < drawDistanceSquare)
                        {
                            // Debug.Log("pix "+j+" "+h);
                            if (tex.GetPixel(width, height) != player.PlayerColor)
                            {
                                tex.SetPixel(width, height, player.PlayerColor);
                            }
                        }
                    }
                }*/
            }
            tex.Apply();
            //  Debug.Log("Draw finished | points " + points.Count);
        }


        public void DrawColorsNew()
        {
            Player player;
            Texture2D tex = sprite.texture;
            for (int pl = 0; pl < gc.GetPlayers().Count; pl++)
            {
                int plY;
                int plX;
                player = gc.GetPlayers()[pl];
//                if (gc.IsPortraitMode)
//                {
//                    plY = (int) (player.transform.localPosition.x*100);
//                    plX = (int) (player.transform.localPosition.y*100);
//                    plY = tex.height - plY;
//                }
//                else
                {
                    plX = (int) (player.transform.localPosition.x*100);
                    plY = (int) (player.transform.localPosition.y*100);
                }
                //     Debug.Log("Drawing player " + pl + " | px: " + plX + ", " + plY);
                Point point;
                int count = points.Count;
                for (int i = 0; i < count; i++)
                {
                    point = points[i];
                    int x = point.X + plX;
                    int y = point.Y + plY;
                    //     if (tex.GetPixel(x, y) != player.PlayerColor)
                    tex.SetPixel(x, y, player.Pattern.PlayerColor);
                }
                /*               for (int i = 0; i < tex.height; i++)
                {
                    int height = tex.height - i;
                    for (int width = 0; width < tex.width; width++)
                    {
                        float d = DistanceSquare(x, y, width, height);
                        if (d < drawDistanceSquare)
                        {
                            // Debug.Log("pix "+j+" "+h);
                            if (tex.GetPixel(width, height) != player.PlayerColor)
                            {
                                tex.SetPixel(width, height, player.PlayerColor);
                            }
                        }
                    }
                }*/
            }
            tex.Apply();
            //  Debug.Log("Draw finished | points " + points.Count);
        }

        public void SaveTexture()
        {
            RenderTexture.active = canvasTexture;
            Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            material.mainTexture = tex; //Put the painted texture as the base
//            foreach (Transform child in brushContainer.transform)
//            {//Clear brushes
//                Destroy(child.gameObject);
//            }
//            //StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
//            Invoke("ShowCursor", 0.1f);
        }

        private void SetPointsTemplateArray()
        {
            int radius = drawDistance;
            for (int y = -radius; y < radius; y++)
            {
                for (int x = -radius; x < radius; x++)
                {
                    float d = DistanceSquare(0, 0, x, y);
                    if (d <= distanceSquare)
                    {
                        points.Add(new Point(x, y));
                    }
                }
            }
            Debug.Log("Points ready for dist " + drawDistance + " | count: " + points.Count);
        }

        private static float DistanceSquare(float x, float y, float x2, float y2)
        {
            return (x2 - x)*(x2 - x) + (y2 - y)*(y2 - y);
        }
    }
}