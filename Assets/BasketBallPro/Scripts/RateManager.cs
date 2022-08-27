using UnityEngine.UI;

namespace GameBench
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class RateManager : MonoBehaviour
    {
        public bool showOnCondition;
        public DelayTimer delayToShowPopup;
        [Space(5)]
        public string wPStoreAppID, androidPackageName, iOSStoreAppID;
        [Space(5)]
        public string email = "info.gamebench@gmail.com", subject = "Feedback about Basket Ball The game";
        [Space(3)]
        public Text btnText;
        public GameObject ratePanel;
        [HideInInspector]
        public bool popupVisible, appRated;
        public DateTime oldTimeExec;
        void Awake()
        {
            //SetRatePanel(false, (PlayerPrefs.GetInt("Rated") == 1));
            if (PlayerPrefs.HasKey("OldTime"))
            {
                DateTime.TryParse(PlayerPrefs.GetString("OldTime"), out oldTimeExec);
            }
            print(oldTimeExec);
        }

        void Update()
        {
            if (!showOnCondition) return;
            if (!popupVisible && !appRated)
                CheckForCondition();
        }

        void CheckForCondition()
        {
            TimeSpan tSpan = DateTime.Now - oldTimeExec;
            print(string.Format("Time Span {0} and Time Now {1} -- Minutes Passed {2}, Days Passed {3}",
                tSpan, DateTime.Now, tSpan.TotalMinutes, tSpan.TotalDays));
            switch (delayToShowPopup)
            {
                case DelayTimer._30Minute:
                    if (tSpan.TotalMinutes >= 30)
                    {
                        Show();
                    }
                    break;
                case DelayTimer._1Day:
                    if (tSpan.TotalDays >= 1)
                    {
                        Show();
                    }
                    break;
                case DelayTimer._3Days:
                    if (tSpan.TotalDays >= 3)
                    {
                        Show();
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnClickRate()
        {
            //SetRatePanel(false, true);
            string rateURL = "https://connect.unity.com/u/5b56f21603b00200199bb25a";
#if UNITY_ANDROID
            rateURL = "market://details?id=" + androidPackageName;
#elif UNITY_IOS
            rateURL = "itms-apps://itunes.apple.com/app/" + iOSStoreAppID;
#elif UNITY_WSA_10_0 || UNITY_WSA_8_1
            rateURL = "https://www.microsoft.com/store/apps/" + wPStoreAppID;
#endif
            Application.OpenURL(rateURL);
        }

        public void OnClickLater()
        {
            SetRatePanel(false);
            oldTimeExec = DateTime.Now;
            PlayerPrefs.SetString("OldTime", oldTimeExec.ToString());
        }

        string body = MyEscapeURL("");

        public void OnClickFeedback()
        {
            SetRatePanel(false, false);
            Application.OpenURL(string.Format("mailto:{0}?subject={1}&body={2}", email, MyEscapeURL(subject), MyEscapeURL(body)));
        }

        static string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

        public void OnClickNever()
        {
            SetRatePanel(false, true);
        }

        public void Show()
        {
            ResetRateDialog();
            SetRatePanel(true);
        }

        void SetRatePanel(bool state, bool rateDone = false)
        {
            //popupVisible = state;
            ////ratePanel.SetActive(state);
            //if (rateDone)
            //{
            //    PlayerPrefs.SetInt("Rated", 1);
            //    PlayerPrefs.Save();
            //}
        }
        int status = 0;
        public int SelectedOption
        {
            set
            {
                status = value;
                btnText.text = msgs[status];
            }
            get
            {
                return status;
            }
        }
        public static string[] msgs = { "Remind Later", "Send Us Feedback", "Rate Now" };
        public void OnClickRateStars(int st)
        {
            GameManager.Instance.PlayClick();
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].image.sprite = starSp[i <= st ? 1 : 0];
            }
            //StartCoroutine(WaitNGo(st));
            SelectedOption = st < 2 ? 1 : 2;
        }

        public void SendFeedbackRate()
        {
            switch (SelectedOption)
            {
                default:
                case 0:
                    OnClickLater();
                    break;
                case 1:
                    OnClickFeedback();
                    break;
                case 2:
                    OnClickRate();
                    break;
            }
        }
        //IEnumerator WaitNGo(int st)
        //{
        //    yield return new WaitForSeconds(0.5f);

        //    if (st < 2)
        //    {
        //        OnClickFeedback();
        //    }
        //    else
        //    {
        //        OnClickRate();
        //    }
        //}
        public Sprite[] starSp;
        public Button[] Stars;
        private void OnApplicationPause(bool pause)
        {
            if (!pause)
                ResetRateDialog();
        }
        void ResetRateDialog()
        {
            SelectedOption = 0;
            if (Stars.Length > 0)
                for (int i = 0; i < Stars.Length; i++)
                {
                    Stars[i].image.sprite = starSp[0];
                }
        }
        private static RateManager _instance;
        public static RateManager Instance
        { get { if (_instance == null) _instance = GameObject.FindObjectOfType<RateManager>(); return _instance; } }
    }

    public enum DelayTimer
    {
        _30Minute,
        _1Day,
        _3Days
    }
}