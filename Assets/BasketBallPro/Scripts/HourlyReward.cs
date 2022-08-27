namespace GameBench
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    public class HourlyReward : MonoBehaviour
    {
        public bool reward4ScreenOn = false;
        public float rewardTimeInMinutes = 1;

        
        public int[] randomPrizes;
        [Space(10)]
        //public Image progressBar;
        //public Animator chestAnimMain, chestAnimPage;
        public Button claimRewardBtn;
        public GameObject/* chestUIDialog,*/ coinAnim;

        const int RewardAmount = 100;
        public Text timerText, collectText;

        private float time;
        public float timeAvailable { get { return rewardTimeInMinutes * 60.0f; } }
        bool _timerOn = false;

        const string EXCITED = "excited", IDLE = "idle", CLICKED = "clicked";
        public bool TimerOn
        {
            get { return _timerOn; }
            set
            {
                _timerOn = value;
                claimRewardBtn.gameObject.SetActive(!_timerOn);
                if (!_timerOn)
                    collectText.text = string.Format("Collect {0}", GameManager.GetValueFormated(currPrize));
                //if (chestAnimMain.gameObject.activeInHierarchy)
                //    chestAnimMain.Play(_timerOn ? IDLE : EXCITED);
                //if (chestAnimPage.gameObject.activeInHierarchy)
                //    chestAnimPage.Play(_timerOn ? IDLE : EXCITED);
            }
        }
        public const string TIMER_KEY = "TIMER_REWARD", LASTSAVEDTIME = "LASTSAVEDTIME";

        DateTime dT;
        private void Start()
        {
            if (!reward4ScreenOn)
            {
                time = PlayerPrefs.GetFloat(TIMER_KEY);
                DateTime.TryParse(PlayerPrefs.GetString(LASTSAVEDTIME, DateTime.Now.ToString()), out dT);
                float seconds = (float)(DateTime.Now - dT).TotalSeconds;
                time += seconds;
            }
            else
            {
                time = PlayerPrefs.GetFloat(TIMER_KEY);
            }
            TimerOn = true;
        }
        void Update()
        {
            if (!TimerOn)
                return;
            time += Time.deltaTime;
            float remainingTime = timeAvailable - time;
            //progressBar.fillAmount = 1 - remainingTime / timeAvailable;

            if (remainingTime > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(remainingTime);
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            }
            else
            {
                currPrize = randomPrizes[UnityEngine.Random.Range(0, randomPrizes.Length)];
                TimerOn = false;
                timerText.text = "00:00:00";
            }
        }

        public void ClaimPlayerReward()
        {
            //chestAnimPage.Play(CLICKED);
            GameManager.Instance.PlaySfx(SFX.ClaimReward);
            coinAnim.SetActive(true);
        }
        int currPrize;
        public void RewardUser()
        {
            GameManager.Instance.Coins += currPrize;
            time = 0;
            timerText.text = "00:00:00";
            TimerOn = true;
        }
        private void OnApplicationQuit()
        {
            OnApplicationPause(true);
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                PlayerPrefs.SetString(LASTSAVEDTIME, DateTime.Now.ToString());
                PlayerPrefs.SetFloat(TIMER_KEY, time);
                PlayerPrefs.Save();
            }
            else
            {
                if (reward4ScreenOn)
                {
                    time = PlayerPrefs.GetFloat(TIMER_KEY);
                }
                else
                {
                    time = PlayerPrefs.GetFloat(TIMER_KEY);
                    DateTime.TryParse(PlayerPrefs.GetString(LASTSAVEDTIME, DateTime.Now.ToString()), out dT);
                    float seconds = (float)(DateTime.Now - dT).TotalSeconds;
                    time += seconds;
                }
            }
        }

        public void ShowHideDialog(bool state)
        {
            //chestUIDialog.SetActive(state);
            //if (chestAnimMain.gameObject.activeInHierarchy)
            //    chestAnimMain.Play(_timerOn ? IDLE : EXCITED);
            //if (chestAnimPage.gameObject.activeInHierarchy)
            //    chestAnimPage.Play(_timerOn ? IDLE : EXCITED);
        }
        [ContextMenu("Delete Prefs")]
        void DoSomething()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted");

        }
        private static HourlyReward _instance;
        public static HourlyReward Instance
        { get { if (_instance == null) _instance = GameObject.FindObjectOfType<HourlyReward>(); return _instance; } }
    }
}