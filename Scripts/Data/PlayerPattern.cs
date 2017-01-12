using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [System.Serializable]
    public class PlayerPattern
    {
        [NonSerialized] public static Color32[] ColorsBasic = {new Color32(0, 85, 184, 255), new Color32(0, 176, 30, 255),
            new Color32(176, 0, 30, 255), new Color32(233, 205, 14, 255), new Color32(152, 2, 169, 255), new Color32(206, 89, 5, 255)};
        [NonSerialized] public static Color32[] ColorsBackground = {new Color32(0, 117, 255, 255), new Color32(22, 216, 88, 255),
            new Color32(216, 22, 88, 255), new Color32(255, 191, 8, 255), new Color32(216, 21, 238, 255), new Color32(255, 119, 21, 255)};
        private Color32 playerColor;
        private Color32 playerBackgroundColor;
        private float speed;
        private float rotationSpeed;
        private bool ai;
        private string playerName;
        private int sourceType;
        private int rewardsCount;

        public Color32 PlayerColor
        {
            get { return playerColor; }
            set { playerColor = value; }
        }

        public Color32 PlayerBackgroundColor
        {
            get { return playerBackgroundColor; }
            set { playerBackgroundColor = value; }
        }

        public float Speed
        {
            get { return speed; }
        }

        public float RotationSpeed
        {
            get { return rotationSpeed; }
        }

        public bool Ai
        {
            get { return ai; }
        }

        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public int SourceType
        {
            get { return sourceType; }
        }

        public int RewardsCount
        {
            get { return rewardsCount; }
            set { rewardsCount = value; }
        }


        public PlayerPattern(int colorId, string playerName, int sourceType)
        {
            this.playerName = playerName;
            this.sourceType = sourceType;
            this.rewardsCount = 0;
            ai = sourceType == Player.SourceAi;
            playerBackgroundColor = PlayerPattern.ColorsBackground[colorId];
            playerColor = PlayerPattern.ColorsBasic[colorId];
            rotationSpeed = 2.5f;
            speed = 2f;
        }

        public PlayerPattern( Color32 playerBackgroundColor, Color playerColor, float rotationSpeed, float speed, string playerName, int sourceType)
        {
            this.ai = sourceType == Player.SourceAi;
            this.playerBackgroundColor = playerBackgroundColor;
            this.playerColor = playerColor;
            this.rotationSpeed = rotationSpeed;
            this.speed = speed;
            this.playerName = playerName;
            this.sourceType = sourceType;
            this.rewardsCount = 0;
        }


        public GameObject GetPrefab()
        {
            return Master.Instance.Menu.PlayerPrefab;
        }

        public bool IsLocalPlayer()
        {
            return ((int)Master.Instance.ClientType == SourceType);
        }

        public static PlayerPattern GetLocalPlayerPattern()
        {
            for (int i = 0; i < Master.Instance.Menu.Patterns.Count; i++)
            {
                PlayerPattern s = Master.Instance.Menu.Patterns[i];
                if (s.IsLocalPlayer())
                {
                    return s;
                }
            }
            return null;
        }
    }
}