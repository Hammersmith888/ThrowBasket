namespace GameBench
{
    using UnityEngine;
    public class ShootBall : MonoBehaviour
    {
        public GameObject shadow;
        public Collider2D col;
        public Rigidbody2D rb;
        public Vector2 defaultPos = new Vector3(0, -3.5f, -2);
        public Animator _animator;
        public SpriteRenderer ballSp, iconSp;

        public ItemType ballType;
        [HideInInspector]
        public int ballId, ballIconId;

        private Vector3 startPos, endPos;
        //float zDistance = 30.0f;
        bool isThrown, goneUp = false, done = true;

        internal bool movedOnce = false, gameStarted = false;
        //void Start()
        //{
        //    SetActive(false);
        //    gameStarted = false;
        //}

        public bool SetAsFront
        {
            set
            {
                ballSp.sortingOrder = value ? 5 : 2;
            }
            get
            {
                return ballSp.sortingOrder == 5;
            }
        }
        void FixedUpdate()
        {
            if (gameStarted)
            {
                if (isThrown)
                {
                    ScaleDown(); return;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Input.mousePosition * -1.0f;
                    //mousePos.z = zDistance;
                    startPos = Camera.main.ScreenToWorldPoint(mousePos);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 mousePos = Input.mousePosition * -1.0f;
                    //mousePos.z = zDistance;
                    endPos = Camera.main.ScreenToWorldPoint(mousePos);
                    endPos.z = Camera.main.nearClipPlane;
                    Vector3 throwDir = (startPos - endPos).normalized;
                    if (throwDir.y > 0.008f)
                    {
                        throwDir.y = Mathf.Clamp(throwDir.y, 0.3f, 0.31f);
                        //rb.WakeUp();
                        rb.isKinematic = false;
                        rb.AddForce(throwDir * 29f, ForceMode2D.Impulse);
                        isThrown = true;
                        GameManager.Instance.PlaySfx(SFX.Swosh);
                        shadow.SetActive(false);
                    }
                }
            }
        }
        internal void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        void ScaleDown()
        {
            if (rb.drag < 1.0f)
                rb.drag += 0.05f;//print(rb.drag);
            if (transform.localScale.x > 0.65f)
                transform.localScale *= 0.99f;
        }
        const string FADER = "Fader";
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == FADER && !goneUp)
            {
                goneUp = true;
            }
            else if (other.tag == FADER && goneUp)
            {
                _animator.Play("fadeOut");
            }
        }
        const string HOP = "Hop", GOAL = "Goal", MOVED_ONCE = "MovedOnce", STUCKER = "Stucker";

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.LogErrorFormat("I am here {0}", collision.collider.name);
            if (movedOnce && collision.collider.name.Contains(STUCKER))
            {
                GameManager.Instance.PlaySfx(SFX.PoleHit);
            }
        }
        void OnTriggerEnter2D(Collider2D other)
        {

            if (other.tag == MOVED_ONCE && !movedOnce)
            {
                movedOnce = true;
                rb.gravityScale = 2;
                return;
            }
            //if (other.tag == HOP && !movedOnce)
            //{
            //    movedOnce = true;
            //    return;
            //}
            if (other.tag == HOP && movedOnce)
            {
                SetAsFront = false;
            }
            if (other.tag == GOAL && movedOnce)
            {
                if (!SetAsFront)
                {
                    done = true;
                    UpdateScore();
                    GameManager.Instance.PlaySfx(SFX.Potted);
                    other.GetComponentInParent<HoopData>()._netAnim.SetTrigger("Score");
                    //other.GetComponentInParent<HoopData>().EnableCollider = true;
                }
            }
        }
        void UpdateScore()
        {
            GameManager.Instance.Mult = GetRandomMult;

            if (GameManager.Instance.CurrentGameMode == 3 && _bounceDone <= 0)
            {
                return;
            }
            int sc = (GetRandomScore * GameManager.Instance.Mult);
            UIManager.Instance.scoreAddText.text = string.Format("+{0}", sc);
            UIManager.Instance.scoreAddText.gameObject.SetActive(true);
            GameManager.Instance.Combo++;
            if (GameManager.Instance.Combo % 5 == 0)
            {
                GameManager.Instance.Crowns++;
            }
            GameManager.Instance.Score += sc;
            GameManager.Instance.Save();
        }
        public void ResetBallAnimEvent()
        {
            ResetBall();
        }

        public void HideBall()
        {
            gameObject.SetActive(false);
        }
        internal bool multiFirstHit = false;
        public void ResetBall(bool firstReset = false)
        {
            //GameManager.Instance.hoopData.EnableCollider = false;
            if (done || firstReset || GameManager.Instance.CurrentGameMode == 1)
            {
                SetActive(false);
                transform.localPosition = defaultPos; transform.localScale = Vector3.one; transform.eulerAngles = Vector3.zero;
                rb.drag = 0.5f;
                SetActive(true);
                if (GameManager.Instance.CurrentGameMode == 2 && done && !multiFirstHit && !firstReset)
                {
                    multiFirstHit = true;
                    GameManager.Instance.StartMovementMultiHoop();
                }
                if (GameManager.Instance.CurrentGameMode != 2 && GameManager.Instance.Combo > 2)
                {
                    GameManager.Instance.ChangeGoalPos();
                }
                movedOnce = goneUp = done = isThrown = false;
                rb.gravityScale = 1;
                
                col.enabled = gameStarted = true;
                //rb.Sleep();
                rb.isKinematic = true;
                shadow.SetActive(true);
                SetAsFront = true;
                UIManager.Instance.scoreAddText.gameObject.SetActive(false);
                if (firstReset)
                {
                    GameManager.Instance.Combo = GameManager.Instance.Score = 0;
                }
            }
            else
            {
                GameManager.Instance.GameOver();
            }
        }
        internal int _bounceDone = 0;
        public const string BOUNCE = "Bounce", ONE_UP = "oneUp";
        public bool isActive
        {
            get { return gameObject.activeInHierarchy; }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (GameManager.Instance.CurrentGameMode == 3 && collision.transform.tag == BOUNCE)
            {
                GameManager.Instance._bounceAnim.Play(ONE_UP);
                _bounceDone++;
            }
        }
        public int GetRandomMult { get { return UnityEngine.Random.Range(1, 5); } }
        public int GetRandomScore { get { return UnityEngine.Random.Range(1, 5); } }
    }
}