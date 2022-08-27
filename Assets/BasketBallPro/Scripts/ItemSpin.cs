namespace GameBench
{
    using UnityEngine;
    using UnityEngine.UI;
    public class ItemSpin : MonoBehaviour
    {
        public Image mainImg;
        public Button btn;

        public GameObject lockImg, tickMark;

        public bool TickMark { set { tickMark.SetActive(value); } }
        internal int mainID;
        public ItemType itemType;
        bool _unlocked;
        public bool isUnlocked
        {
            get
            {
                return _unlocked;
            }
            set
            {
                _unlocked = value;
                btn.interactable = _unlocked;
                lockImg.SetActive(!_unlocked);
                PlayerPrefs.SetInt(MyStringID, value ? 99 : 0);
                PlayerPrefs.Save();
            }
        }

        public string MyStringID
        {
            get
            {
                return Configs.GetBallKey(mainID);
            }
        }
        bool CheckUnlock
        {
            get
            {
                return PlayerPrefs.GetInt(MyStringID, 0) > 0;
            }
        }
        internal void SetBallSimple(int idMain, int idSub)
        {
            itemType = ItemType.Simple;
            mainID = idMain;
            mainImg.sprite = Configs.Instance.simpleBalls[mainID];
            isUnlocked = CheckUnlock;
        }
        public void OnClick()
        {
            if (isUnlocked)
            {
                GameManager.Instance.PlayClick();
                UIManager.Instance.DisableAllSelected(itemType);
                TickMark = true;
                GameManager.Instance.ChangeBallHoop(mainID, itemType);
                //UIManager.Instance.ShowScreen(0);
            }
        }
    }
}