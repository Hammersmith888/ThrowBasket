
namespace GameBench
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    public class UIManager : MonoBehaviour
    {
        public GameObject spinUI;
        public Button spinBtn, spinBackBtn;
        public Image gameIconImg;
        public Image[] customizeBtnImgs;
        public Sprite[] customizeBtnSp;
        public Text scoreTextLast, scoreText, bestText, crownText, scoreTextShare, crownLevelText,
            scoreAddText;
        public Text[] coinsText;
        public Image barNextCrown;
        public Text[] gameModeScoreText, gameModeCrownText;
        public Transform bgFlipable;
        public GameObject shareObj, pauseBtn;
        public GameObject[] screens;
        public Transform ballsParent, hoppsParent, cBallParent;

        internal Item[] cBallItems, hoopItems;

        internal ItemSpin[] spinBallItems;

        public Tgl tglSound, tglMusic;
        private void Awake()
        {
            PlayerPrefs.SetInt(Configs.GetHoopKey(0), 99);
            PlayerPrefs.SetInt(Configs.GetBallKey(0), 99);
            spinBallItems = ballsParent.GetComponentsInChildren<ItemSpin>();
            hoopItems = hoppsParent.GetComponentsInChildren<Item>();
            cBallItems = cBallParent.GetComponentsInChildren<Item>();
            int cond = 0, ind = 0;
            for (int i = 0; i < hoopItems.Length; i++)
            {
                //Debug.LogErrorFormat("A = {0}, B = {1}", a, b);
                hoopItems[i].SetHoop(i, i, Configs.Instance.hoopUnlockCondition[cond], ind);
                ind++; if (ind >= 4) ind = 0;
                if ((i + 1) % 4 == 0) { cond++; }
            }
            for (int i = 0; i < spinBallItems.Length; i++)
            {
                spinBallItems[i].SetBallSimple(i, i);
            }
            int bgID = 0; cond = ind = 0;
            for (int i = 0; i < cBallItems.Length - 4; i++)
            {
                //Debug.LogErrorFormat("Cond= {0} Index= {1}", cond, ind);
                cBallItems[i].SetGlobeBall(bgID, i, Configs.Instance.ballUnlockCondition[cond], ind);
                bgID++; if (bgID >= 8) bgID = 0;
                ind++; if (ind >= 4) ind = 0;
                if ((i + 1) % 4 == 0) { cond++; }
            }
            int iz = cBallItems.Length - 4;
            cBallItems[iz].SetEmojiBall(0, 0, Configs.Instance.ballUnlockCondition[4], 0); iz++;
            cBallItems[iz].SetNoSoGlobe(0, Configs.Instance.ballUnlockCondition[4], 1); iz++;
            cBallItems[iz].SetNoSoGlobe(1, Configs.Instance.ballUnlockCondition[4], 2); iz++;
            cBallItems[iz].SetEmojiBall(1, 0, Configs.Instance.ballUnlockCondition[4], 3);

            spinBallItems[0].TickMark = true;
            hoopItems[0].TickMark = true;
            ShowScreen(0);
        }
        bool settingAnim
        {
            get
            {
                return settingBtnAnim.GetBool("e");
            }
            set
            {
                settingBtnAnim.SetBool("e", value);
            }
        }
        public Animator _customizeAnim, settingBtnAnim;

        
        public void OnClickSound()
        {
            GameManager.Instance.PlayClick();
            GameManager.Instance.SFXOn = !GameManager.Instance.SFXOn;
            settingAnim = false;
        }
        
        public void OnClickMusic()
        {
            GameManager.Instance.PlayClick();
            GameManager.Instance.MusicOn = !GameManager.Instance.MusicOn;
            settingAnim = false;
        }
        public void OnClickSettingBtn(int id)
        {
            switch (id)
            {
                case 0:
                    settingAnim = !settingAnim;
                    break;
                case 1:
                    settingAnim = false;
                    ShowScreen(5);
                    break;
            }
        }
        
        public void BallHopClick(bool hopp)
        {
            GameManager.Instance.PlayClick();
            _customizeAnim.SetBool("hopp", hopp);
        }

        public void ShowScreen(int id)
        {
            if (id == 3 || id == 4)
            {
                if (!GameBench.FBManager.Instance.LoggedIn)
                {
                    GameBench.FBManager.Instance.InitNLogin();
                    return;
                }
            }
            for (int i = 0; i < screens.Length; i++)
            {
                screens[i].SetActive(id == i);
            }

            if (id == 0)
            {
                RefreshRewardAdUI();
            }
        }
        public GameObject videoRewardUI;
        public Button btnOnShop;
        public void ShowRewardAd()
        {
            AdsManager.Instance.ShowRewardedAd();
        }
        public void OnClickSpin()
        {
            ShowScreen(6);
            spinUI.SetActive(true);
        }


        private static UIManager _instance;
        public static UIManager Instance
        { get { if (_instance == null) _instance = FindObjectOfType<UIManager>(); return _instance; } }

        internal void DisableAllSelected(ItemType itemType)
        {
            if (itemType == ItemType.Hoop)
            {
                foreach (var item in hoopItems)
                {
                    item.TickMark = false;
                }
            }
            else
            {
                foreach (var item in cBallItems)
                {
                    item.TickMark = false;
                }
                foreach (var item in spinBallItems)
                {
                    item.TickMark = false;
                }
            }
        }

        internal void RefreshRewardAdUI()
        {
            videoRewardUI.SetActive(AdsManager.Instance.IsAdAvailable);
            btnOnShop.interactable = AdsManager.Instance.IsAdAvailable;
        }

        public bool SetBtnz { set { spinBtn.interactable = value; spinBackBtn.interactable = value; } }

        int intCount = 0;
        public static int totalGamesCount = 0;
        const int GAME_COUNT_TO_SHOW_RATE = 6;
        [OPS.Obfuscator.Attribute.DoNotRenameAttribute]
        internal void ShowGameOver(bool show)
        {
            AdsManager.Instance.ShowBanner(false);
            intCount++;
            totalGamesCount++;
            if (intCount >= 3) { intCount = 0; AdsManager.Instance.ShowInterstitial(); }

            //if (show)
            if (totalGamesCount <= GAME_COUNT_TO_SHOW_RATE)
            {
                ShowScreen(0);
            }
            else
            {
                totalGamesCount = 0;
                ShowScreen(5);
            }
            //else
            //    screens[0].SetActive(false);
        }


        
        public void OnClickShare()
        {
            scoreTextShare.text = GameManager.GetValueFormated(GameManager.Instance.BestScore);
            shareObj.SetActive(true);
        }
        internal void CheckUnlock(bool isBall = true)
        {
            if (isBall)
            {
                int bUn = 0, ballIndex = -1;
                for (int i = 0; i < cBallItems.Length; i++)
                {
                    ballIndex++;
                    if (ballIndex >= 4) ballIndex = 0;
                    //Debug.LogWarningFormat("Ball Index {0}, i {1}", ballIndex, i);
                    if (cBallItems[i].isUnlocked)
                    {
                        continue;
                    }
                    else
                    {
                        bUn = Configs.Instance.ballUnlockCondition[ballIndex];
                        cBallItems[i].isUnlocked = GameManager.Instance.crowns[ballIndex] >= bUn;
                    }
                }
            }
            else
            {
                int hoopIndex = -1;
                for (int i = 0; i < hoopItems.Length; i++)
                {
                    hoopIndex++;
                    if (hoopIndex >= 4) hoopIndex = 0;

                    //Debug.LogWarningFormat("Hoop Index {0}, i {1}", hoopIndex, i);
                    if (hoopItems[i].isUnlocked)
                        continue;
                    else
                        hoopItems[i].isUnlocked = GameManager.Instance.bestScores[hoopIndex] >= Configs.Instance.hoopUnlockCondition[hoopIndex];
                }
            }
        }
        public void RefreshScoreOnGameMode()
        {
            for (int i = 0; i < gameModeScoreText.Length; i++)
            {
                gameModeScoreText[i].text = string.Format("Best: {0}", GameManager.Instance.bestScores[i]);
            }
            GameManager.Instance.SaveBestScore();
        }

        public void RefreshCrownOnGameMode()
        {
            for (int i = 0; i < gameModeScoreText.Length; i++)
            {
                gameModeCrownText[i].text = GameManager.Instance.crowns[i].ToString();
            }
            GameManager.Instance.SaveCrown();
        }

     
        public void OnClickPauseGame()
        {
            GameManager.Instance.GameOver();
        }

        const string LIKED_FB = "LIKED_ON_FB";
        string your_page_id_number = "336392406508042";

        [OPS.Obfuscator.Attribute.DoNotRenameAttribute]
        public void OnClickFB()
        {
            GameManager.Instance.PlayClick();

            string facebookApp = "fb://page/" + your_page_id_number;
            string facebookAddress = "https://facebook.com/" + your_page_id_number;
            float startTime;
            startTime = Time.timeSinceLevelLoad;
            Application.OpenURL(facebookApp);

            if (Time.timeSinceLevelLoad - startTime <= 1f)
            {
                //fail. Open safari.
                Application.OpenURL(facebookAddress);
            }

            if (!PlayerPrefs.HasKey(LIKED_FB))
            {
                PlayerPrefs.SetInt(LIKED_FB, 123456);
                PlayerPrefs.Save();
                GameManager.Instance.Coins += 100;
            }
        }
        
        public void ChangeGameMode(int id)
        {
            GameManager.Instance.PlayClick();
            GameManager.Instance.CurrentGameMode = id;
            scoreTextLast.text = string.Format("Last: 0");
            ShowScreen(0);

        }
    }
}