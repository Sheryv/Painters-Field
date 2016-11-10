using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public Color PlayerColor;
        public float Speed;
        public float RotationSpeed;
        // Use this for initialization
        void Start()
        {

        }

        public void SetUp()
        {
            SpriteRenderer[] renderer = GetComponentsInChildren<SpriteRenderer>();
            renderer[1].color = PlayerColor;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
