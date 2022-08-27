namespace GameBench
{
    using System.Collections;
    using UnityEngine;
    public enum SFX
    {
        Click = 0,
        PoleHit,
        Potted,
        SpinStart,
        ClaimReward,
        Claim1Coin,
        Claim2,
        Swosh
    }
    public class GameManager : MonoBehaviour
    {
        public const int TEST_BEST_SCORE = 50, TEST_CROWNS = 50;
        public Rigidbody2D goalRb;
        public GameObject gameOverPanel;
        public ScrollingScript hoopLst1, hoopLst2;
        public Timer timerObj;
        public HoopData hoopData;
        public ShootBall[] shootBalls;
        public Animator _bounceAnim;

        public GameObject gameModeBounce;
        public AudioClip[] sfxClips;
        [HideInInspector]
        public float sfxVol = 1.0f;
        public AudioSource sfxSource, musiSource;

        bool _sfx, _music;

        public bool SFXOn
        {
            get { return _sfx; }
            set
            {
                _sfx = value;
                UIManager.Instance.tglSound.Enabled = _sfx;
                sfxSource.volume = _sfx ? 1 : 0;
                PlayerPrefs.SetInt(SFX, SFXOn ? 1 : 0);
                PlayerPrefs.Save();

            }
        }
        public bool MusicOn
        {
            get { return _music; }
            set
            {
                _music = value;
                UIManager.Instance.tglMusic.Enabled = _music;
                musiSource.volume = _music ? 1 : 0;
                PlayerPrefs.SetInt(MUSIC, MusicOn ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        private void Start()
        {
            Load();
        }
        public void PlayGame()
        {
            gameOverPanel.GetComponent<Animator>().Play("out");
        }
        int CurrBall = 0;
        //public int CurrBall
        //{
        //    get
        //    {
        //        return _currBall;
        //    }
        //    set
        //    {
        //        _currBall = value;
        //    }
        //}
        public ShootBall shootBall
        {
            get
            {
                return shootBalls[CurrBall];
            }
        }

        public void StartMovementMultiHoop()
        {
            hoopLst1.ResetDefault(true, true);
            hoopLst2.ResetDefault(true, true);
        }

        public void GameOver()
        {
            UIManager.Instance.pauseBtn.SetActive(false);
            timerObj.EndTimer();
            UIManager.Instance.scoreText.gameObject.SetActive(false);
            shootBall.SetActive(false);
            Mult = 0; Combo = 0;
            shootBall.multiFirstHit = shootBall.gameStarted = false;
            //ChangeGoalPos(true);
            hoopData.SetActive(true);

            hoopLst1.ResetDefault();
            hoopLst2.ResetDefault();
            Debug.LogWarning("Score is " + Score);
            if (Score > BestScore)
                BestScore = Score;
            UIManager.Instance.ShowGameOver(true);
        }
        public void StartGame()
        {
            //shootBall.SetActive(true);
            AdsManager.Instance.ShowBanner(true);
            UIManager.Instance.pauseBtn.SetActive(true);
            shootBall.ResetBall(true);
            Score = 0;
            gameOverPanel.SetActive(false);
            gameModeBounce.SetActive(CurrentGameMode == 3);
            timerObj.SetActive(CurrentGameMode == 1);

            hoopData.SetActive(CurrentGameMode != 2);
            hoopLst1.SetActive(CurrentGameMode == 2);
            hoopLst2.SetActive(false);

            if (CurrentGameMode != 2)
            {
                ChangeGoalPos(true);
            }
            switch (CurrentGameMode)
            {
                case 1:
                    timerObj.RunTimer();
                    break;
                case 3:
                    shootBall._bounceDone = 0;
                    break;
            }
        }

        public void PlaySfx(SFX sfx)
        {
            sfxSource.PlayOneShot(sfxClips[(int)sfx]);
        }

        void Load()
        {

            for (int i = 0; i < 4; i++)
            {
                bestScores[i] = PlayerPrefs.GetInt(GetBestScoreKey(i), TEST_BEST_SCORE);
                crowns[i] = PlayerPrefs.GetInt(GetCrownKey(i), TEST_CROWNS);
                //Debug.LogWarningFormat("Score: {0} Crown: {1}", gameData[i].bestScore, gameData[i].crowns);
            }

            CurrentGameMode = PlayerPrefs.GetInt(MODE, 0);
            CurrHoopID = PlayerPrefs.GetInt(SELECTED_HOOP, 0);
            SFXOn = PlayerPrefs.GetInt(SFX, 1) == 1 ? true : false;
            MusicOn = PlayerPrefs.GetInt(MUSIC, 1) == 1 ? true : false;
            Coins = PlayerPrefs.GetInt(COINS, 100);
        }
        public const string SFX = "_SFX", MUSIC = "_Music", COINS = "COINS", BEST_SCORE = "BEST_SCORE_",
            CORWNS = "CROWNS_", MODE = "_Game_MODE", SELECTED_BALL = "_SELECTED_BALL", SELECTED_HOOP = "__SELECTED_HOOP";
        //internal int[] bestScoreLst = new int[4] { 0, 0, 0, 0 };

        public int[] bestScores = new int[4], crowns = new int[4];
        public string GetBestScoreKey(int i)
        {
            return string.Format("{0}{1}", BEST_SCORE, i);
        }
        public string GetCrownKey(int i)
        {
            return string.Format("{0}{1}", CORWNS, i);
        }
        public void SaveBestScore()
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerPrefs.SetInt(GetBestScoreKey(i), bestScores[i]);
            }
            PlayerPrefs.Save();
        }

        public void SaveCrown()
        {
            for (int i = 0; i < 4; i++)
            {
                PlayerPrefs.SetInt(GetCrownKey(i), crowns[i]);
            }
            PlayerPrefs.Save();
        }
        public void Save()
        {

            PlayerPrefs.SetInt(COINS, Coins);
            PlayerPrefs.SetInt(MODE, CurrentGameMode);
            PlayerPrefs.Save();
        }
        int _currDir;
        public int CurrDir { get { return _currDir; } set { _currDir = value; } }

        public void ChangeBallHoop(int bgId, ItemType bt = ItemType.Simple, int iconId = 0)
        {
            switch (bt)
            {
                default:
                case ItemType.Simple:
                    CurrBall = 0;
                    shootBall.ballSp.sprite = Configs.Instance.simpleBalls[bgId];
                    break;
                case ItemType.SimpleCarrier:
                    shootBall.ballSp.sprite = Configs.Instance.simpleCarrierBalls[bgId];
                    break;
                case ItemType.Globe:
                    CurrBall = 1;
                    shootBall.ballSp.sprite = Configs.Instance.globeBallBg[bgId];
                    shootBall.iconSp.sprite = Configs.Instance.globeBallCh[iconId];
                    break;
                case ItemType.Emoji:
                    CurrBall = 2;
                    Debug.LogWarningFormat("{0} >>>>>>>>>>>>>>>>> {1} >>>>>>>>>> {2}", iconId, bgId, shootBall.name);
                    if (bgId == 0)
                    {
                        shootBall.ballSp.sprite = Configs.Instance.emojiBall1.bg;
                        shootBall.iconSp.sprite = Configs.Instance.emojiBall1.emojis[0];
                    }
                    else
                    {
                        shootBall.ballSp.sprite = Configs.Instance.emojiBall2.bg;
                        shootBall.iconSp.sprite = Configs.Instance.emojiBall2.emojis[0];
                    }
                    break;
                case ItemType.Hoop:
                    CurrHoopID = bgId;
                    break;
            }
        }

        Vector2[] posz = new Vector2[6] {
          new Vector2(-0.5f, 4f), new Vector2(0, 4f), new Vector2(0.5f, 4f),
          new Vector2(-1f, 3.5f), new Vector2(0, 3.5f) ,new Vector2(0.5f,3.5f)};
        bool inProcess = false;
        public void ChangeGoalPos(bool reset = false)
        {
            if (inProcess) return;
            CurrDir = reset ? 1 : Random.Range(0, 6);
            //Debug.LogFormat("{0} ======= {1}", prevDir, CurrDir);
            inProcess = true;

            StartCoroutine(MoveToPosition(goalRb, posz[CurrDir], 1.5f));
        }
        public IEnumerator MoveToPosition(Rigidbody2D rb, Vector2 targetPos, float delay = 0.5f)
        {
            float elapsedTime = 0.0f;
            if (rb.position != targetPos)
                hoopData._netAnim.Play(string.Format("anim{0}", Random.Range(2, 5)));
            while (elapsedTime < delay)
            {
                rb.MovePosition(Vector3.Lerp(rb.position, targetPos, elapsedTime / delay));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            shootBall.gameStarted = true;
            rb.MovePosition(targetPos);
            inProcess = false;
        }
        int _coins;
        public int Coins
        {
            get { return _coins; }
            set
            {
                _coins = value;
                foreach (var item in UIManager.Instance.coinsText)
                {
                    item.text = GetValueFormated(_coins);
                }
                PlayerPrefs.SetInt(COINS, _coins);
                PlayerPrefs.Save();
            }
        }
        private static GameManager _instance;
        public static GameManager Instance
        { get { if (_instance == null) _instance = FindObjectOfType<GameManager>(); return _instance; } }
        int _score;
        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                UIManager.Instance.scoreText.gameObject.SetActive(_score > 0);
                UIManager.Instance.scoreTextLast.text = string.Format("Last: {0}", _score);
                UIManager.Instance.scoreText.text = GetValueFormated(_score);
            }
        }

        int _combo;
        public int Combo
        {
            get { return _combo; }
            set
            {
                _combo = value;
                if (!hoopData.coinObj.activeInHierarchy)
                    hoopData.coinObj.SetActive(_combo > 0 && _combo % 5 == 0);
            }
        }
        int _mult;
        public int Mult
        {
            get { return _mult; }
            set
            {
                _mult = value;
            }
        }
        int _currHoopId = 0;
        public int CurrHoopID
        {
            get { return _currHoopId; }
            set
            {
                _currHoopId = value;
                hoopData.hoopRend[0].sprite = Configs.Instance.hoopSprites[_currHoopId].hoopSp;
                hoopData.hoopRend[1].sprite = Configs.Instance.hoopSprites[_currHoopId].stripSp;
                hoopData.hoopRend[2].sprite = Configs.Instance.hoopSprites[_currHoopId].bgSprite;
                //if (CurrentGameMode == 2)
                //{
                for (int i = 0; i < hoopLst1.hoopsList.Length; i++)
                {
                    hoopLst1.hoopsList[i].SetHoop(CurrHoopID);
                    hoopLst2.hoopsList[i].SetHoop(CurrHoopID);
                }
                //}
                PlayerPrefs.GetInt(SELECTED_HOOP, 0);
                PlayerPrefs.Save();
            }
        }
        int _currGameMode;
        public int CurrentGameMode
        {
            get { return _currGameMode; }
            set
            {
                _currGameMode = value;
                BestScore = bestScores[CurrentGameMode];
                Crowns = crowns[CurrentGameMode];
                PlayerPrefs.SetInt(MODE, CurrentGameMode);
                PlayerPrefs.Save();
            }
        }
        public int Crowns
        {
            get { return crowns[CurrentGameMode]; }
            set
            {
                crowns[CurrentGameMode] = value;
                UIManager.Instance.barNextCrown.fillAmount = crowns[CurrentGameMode] / 50.0f;
                UIManager.Instance.crownText.text = crowns[CurrentGameMode].ToString();
                UIManager.Instance.RefreshCrownOnGameMode();
                UIManager.Instance.CheckUnlock();
            }
        }
        public int BestScore
        {
            get { return bestScores[CurrentGameMode]; }
            set
            {
                bestScores[CurrentGameMode] = value;
                UIManager.Instance.gameIconImg.sprite = Configs.Instance.gameModeIcon[CurrentGameMode];
                UIManager.Instance.bestText.text = string.Format("Best: {0}", bestScores[CurrentGameMode]);
                UIManager.Instance.RefreshScoreOnGameMode();
                UIManager.Instance.CheckUnlock(false);
            }
        }
        public static string GetValueFormated(int val)
        {
            return System.String.Format("{0:n0}", val);
        }
        internal void AddIAPCoins(int i)
        {
            Coins += Configs.Instance.iapItems[i].coinsCount;
        }

        public void PlayClick()
        {
            PlaySfx(GameBench.SFX.Click);
        }
    }


    public enum Dir
    {
        TopLeft = 0,
        TopCenter,
        TopRight,

        Left,
        Center,
        Right
    }
    public enum ItemType
    {
        Simple,
        Globe,
        Emoji,
        SimpleCarrier,
        Hoop
    }
}