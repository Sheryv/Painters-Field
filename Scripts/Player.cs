using System;
using System.Collections;
using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class Player : NetworkBehaviour
    {
        public const int SourceServer = 1;
        public const int SourceClient = 2;
        public const int SourceAi = 3;
      //  public const int TypeFakeHost = 3;
      //  public const int TypeFakeRemote = 4;
        private const float LeftEdge = 2f;
        private const float RightEdge = 17.25f;
        private const float TopEdge = 11.8f;
        private const float BotEdge = 3.95f;
        private const float DetectAiRadius = 0.85f;
        private Rigidbody2D rigid;
        private float direction;
        private GameController gc;
        private AudioListener audioListener;
        private float counter;
        private bool aiMoveForced = false;

        public float Speed;
        public int PlayerId;
        public float LastPercent;
        public PlayerPattern Pattern;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            GameController.MatchFinishEvent += GameControllerOnMatchFinishEvent;
            audioListener = GetComponent<AudioListener>();
            if (isLocalPlayer)
            {
              //  audioListener.enabled = true;
            }
        }

        private void GameControllerOnMatchFinishEvent(int i)
        {
            if (isLocalPlayer)
            {
             //   audioListener.enabled = false;
            }
        }

        public void SetUp(PlayerPattern pattern, int id)
        {
            gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            direction = 0;
            PlayerId = id;
            Pattern = pattern;
            SpriteRenderer[] renderer = GetComponentsInChildren<SpriteRenderer>();
            renderer[0].color = pattern.PlayerColor;
            renderer[1].color = pattern.PlayerBackgroundColor;
            Speed = pattern.Speed;
            if (Pattern.Ai)
            {
               // StartCoroutine(AiMoving());
            }
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            float mid = Screen.width / 2f;
            float top = Screen.height * 0.7f;
            if ((isLocalPlayer || Master.Instance.GameMode == GameMode.Singleplayer) && !Pattern.Ai)
            {
                if (Master.GetState() == GameStates.Playing)
                {
                    if (Master.Instance.Preferences.ControlType == ControlTypes.Keyboard)
                    {
                        direction = Input.GetAxis("H" + PlayerId);
                        // v = Input.GetAxis("Vertical");
                    }
                    else if (Master.Instance.Preferences.ControlType == ControlTypes.Touch)
                    {
                        int count = Input.touchCount;
                        for (int i = 0; i < count; i++)
                        {
                            Touch t = Input.GetTouch(i);
                            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Began)
                            {
                                //left
                                if (t.position.x < mid && t.position.y < top)
                                    direction = -1;
                                //right
                                if (t.position.x > mid && t.position.y < top)
                                    direction = 1;
                            }
                            else
                            {
                                direction = 0;
                            }
                        }
                    }

                }

            }
            if (Master.GetState() == GameStates.Playing)
            {
                rigid.velocity = transform.up * Speed;
                transform.Rotate(new Vector3(0, 0, -direction * Pattern.RotationSpeed));
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }
        }

        private void Update()
        {
            if (Pattern.Ai)
            {
            AiMove();
            }
        }


        private void SetSpeed(float speed)
        {
            Speed = speed;
        }

        private IEnumerator AiMoving()
        {
            float d, s;
            while (true)
            {
                d = UnityEngine.Random.Range(0f, 1f);
                direction = gc.AiDirection.Evaluate(d);
                s = gc.AiTime.Evaluate(UnityEngine.Random.Range(0f, 1f));
                s -= direction / 2;
                yield return new WaitForSeconds(s);
            }
        }

        private void AiMove()
        {
            float d, s;
            if (counter <= 0)
            {
                d = UnityEngine.Random.Range(0f, 1f);
                direction = gc.AiDirection.Evaluate(d);
                counter = gc.AiTime.Evaluate(UnityEngine.Random.Range(0f, 1f));
                counter -= direction / 2;
                aiMoveForced = false;
            }
            else
            {
                counter-=Time.deltaTime;
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (!aiMoveForced && collision.collider is EdgeCollider2D)
            {
                float h;
                int l = UnityEngine.Random.Range(0, 1);
                if (l == 1) //going up
                {
                    h = -1f;
                }
                else
                {
                    h = 1f;
                }

                aiMoveForced = true;
                direction = h;
                counter = 1f;
            }
            if (collision.gameObject.GetComponent<Player>() != null)
            {
              //  Debug.Log("Player hit: "+collision.gameObject.name);
            }
           
        }

        private void GameStateChanged(GameStates obj)
        {

        }

        public void OnEnable()
        {
            Master.GameStateChangedEvent += GameStateChanged;
        }


        public void OnDisable()
        {
            Master.GameStateChangedEvent -= GameStateChanged;
            GameController.MatchFinishEvent -= GameControllerOnMatchFinishEvent;
        }

        private int GetRotateDirNearEdge(Vector3 pos)
        {
            float x = pos.x, y = pos.y;
            if (x+DetectAiRadius>RightEdge)
            {
                
            }
            return 1;
        }
    }
}