namespace GameBench
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    public class LuckyWheel : MonoBehaviour
    {
        public GameObject spinDoneDialog;
        public Transform wheelToRotate;
        public SpriteRenderer dotSpRend;

        WheelSlice[] wheelParts;
        public Sprite[] dots = new Sprite[2];
        bool spinning;
        float anglePerReward;
        public Sprite coinSp;
        public Image rewardImg;

        void Awake()
        {
            spinning = false;
            wheelParts = wheelToRotate.GetComponentsInChildren<WheelSlice>();
            //SetupWheel();
        }

        public int GetRandomCoinReward()
        {
            return Random.Range(100, 1000);
        }
        private void OnEnable()
        {
            ResetWheel();
        }
        public void SetupWheel()
        {
            //Debug.LogWarningFormat("I am in WheelSetup {0}", wheelParts.Length);
            anglePerReward = 360 / wheelParts.Length;

            for (int i = 0; i < wheelParts.Length; i++)
            {
                wheelParts[i].transform.localEulerAngles = new Vector3(0, 0, (i * -anglePerReward));
                if (i == 0 || i == 3 || i == 6)
                {
                    wheelParts[i].SetValues(Configs.Instance.chunkSp[i], coinSp, GetRandomCoinReward(), RewardType.Coin);
                    continue;
                }
                int n = GetBallIndex();
                if (n == -1)
                {
                    wheelParts[i].SetValues(Configs.Instance.chunkSp[i], coinSp, GetRandomCoinReward(), RewardType.Coin);
                }
                else
                    wheelParts[i].SetValues(Configs.Instance.chunkSp[i], Configs.Instance.simpleBalls[n], n);
            }
        }

        public int GetBallIndex()
        {
            int val = 0;
            for (int i = 0; i < UIManager.Instance.spinBallItems.Length; i++)
            {
                if (!UIManager.Instance.spinBallItems[i].isUnlocked) val++;
            }
            if (val > 0)
            {
                do
                {
                    val = Random.Range(0, Configs.Instance.simpleBalls.Length);
                    //Debug.LogErrorFormat("Value of Selected Ball is {0}", val);
                } while (UIManager.Instance.spinBallItems[val].isUnlocked);
                return val;
            }
            else
                return -1;
        }
        RewardInfo _selectedReward;
        public RewardInfo SelectedReward
        {
            get
            {
                return _selectedReward;
            }
            set
            {
                _selectedReward = value;
            }
        }
        public GameObject coinAnim;
        public void OnClickSpinNow()
        {
            GameManager.Instance.PlaySfx(SFX.Claim2);
            if (GameManager.Instance.Coins >= 100)
            {
                coinAnim.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
                UIManager.Instance.ShowScreen(2);
            }
        }
        public void SpinNow()
        {
            GameManager.Instance.Coins -= 100;
            dotSpRend.sprite = dots[1];
            UIManager.Instance.SetBtnz = false;
            StartSpin();
        }
        public int targetToStopOn { get { return Random.Range(0, 12); } }
        public void StartSpin()
        {
            if (!spinning)
            {
                GameManager.Instance.PlaySfx(SFX.SpinStart);
                float maxAngle = 360 * Configs.Instance.speedMultiplier + targetToStopOn * anglePerReward;
                //AnimateWheel(false);
                StartCoroutine(RotateWheel(Configs.Instance.duration, maxAngle));
            }
        }
        IEnumerator RotateWheel(float time, float maxAngle)
        {
            spinning = true;
            float timer = 0.0f;
            float startAngle = wheelToRotate.transform.eulerAngles.z;
            maxAngle = maxAngle - startAngle;
            while (timer < time)
            {
                //to calculate rotation
                float angle = maxAngle * Configs.Instance.animationCurve.Evaluate(timer / time);
                wheelToRotate.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
                timer += Time.deltaTime;
                yield return 0;
            }
            wheelToRotate.transform.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);
            PostSpinSteps();
        }
        void PostSpinSteps()
        {
            if (SelectedReward.rewardType == RewardType.Coin)
            {
                rewardImg.sprite = coinSp;
                GameManager.Instance.Coins += SelectedReward.idQ;
            }
            else
            {
                rewardImg.sprite = SelectedReward.sp;
                UIManager.Instance.spinBallItems[SelectedReward.idQ].isUnlocked = true;
            }
            spinDoneDialog.SetActive(true);
        }
        public void ResetWheel()
        {
            spinning = false;
            SelectedReward = new RewardInfo();
            SetupWheel();
        }
        public void OnClickOkSpinDialog()
        {
            GameManager.Instance.PlayClick();
            dotSpRend.sprite = dots[0];
            UIManager.Instance.SetBtnz = true;
            ResetWheel();
        }
        private static LuckyWheel _instance;
        public static LuckyWheel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LuckyWheel>();
                }
                return _instance;
            }
        }

    }

    public enum RewardType
    {
        Ball,
        Coin
    }
}