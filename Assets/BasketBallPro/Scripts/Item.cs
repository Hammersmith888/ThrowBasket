
namespace GameBench
{
    using UnityEngine;
    using UnityEngine.UI;
    public struct ItemData
    {
        public int mainID, subID;
        public ItemType itemType;
        public int gameMode;
    }
    public class Item : MonoBehaviour
    {
        public Image mainImg, subImg, gameModeIcon;
        public Text countText;
        public Button btn;
        public GameObject priceLock, tickMark;
        public bool TickMark { set { tickMark.SetActive(value); } }
        internal ItemData data = new ItemData();
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
                priceLock.SetActive(!_unlocked);
                PlayerPrefs.SetInt(MyStringID, value ? 99 : 0);
                PlayerPrefs.Save();
            }
        }


        public string MyStringID
        {
            get
            {
                switch (data.itemType)
                {
                    default:
                    case ItemType.Globe:
                        return Configs.GetCBallKey(data.mainID, data.subID);
                    case ItemType.Emoji:
                        return Configs.GetCBallKey(data.mainID, data.subID, true);
                    case ItemType.Hoop:
                        return Configs.GetHoopKey(data.mainID);
                }
            }
        }
        bool CheckUnlock
        {
            get
            {
                return PlayerPrefs.GetInt(MyStringID, 0) > 0;
            }
        }
        internal void SetHoop(int idMain, int idSub, int cond, int gameMode)
        {
            data.gameMode = gameMode;
            gameModeIcon.sprite = Configs.Instance.gameModeIcon[gameMode];
            data.itemType = ItemType.Hoop;
            data.mainID = idMain;
            data.subID = idSub;
            mainImg.sprite = Configs.Instance.hoopSprites[data.mainID].bgSprite;
            subImg.sprite = Configs.Instance.hoopSprites[data.mainID].hoopSp;
            countText.text = string.Format("Score: {0}", cond);
            isUnlocked = CheckUnlock;
        }
        public void SetNoSoGlobe(int b, int q, int gameMode, ItemType bt = ItemType.SimpleCarrier)
        {
            data.gameMode = gameMode;
            gameModeIcon.sprite = Configs.Instance.gameModeIcon[gameMode];
            data.itemType = bt;
            data.mainID = b;
            mainImg.sprite = Configs.Instance.simpleCarrierBalls[data.mainID];
            subImg.gameObject.SetActive(false);
            countText.text = string.Format("{0}", q);
            isUnlocked = CheckUnlock;
        }
        public void SetGlobeBall(int b, int i, int q, int gameMode, ItemType bt = ItemType.Globe)
        {
            data.gameMode = gameMode;
            gameModeIcon.sprite = Configs.Instance.gameModeIcon[gameMode];
            data.itemType = bt;
            data.mainID = b; data.subID = i;
            mainImg.sprite = Configs.Instance.globeBallBg[data.mainID];
            subImg.sprite = Configs.Instance.globeBallCh[data.subID];
            countText.text = string.Format("{0}", q);
            isUnlocked = CheckUnlock;
        }
        public void SetEmojiBall(int main_id, int sub_id, int q, int gameMode, ItemType bt = ItemType.Emoji)
        {
            data.gameMode = gameMode;
            gameModeIcon.sprite = Configs.Instance.gameModeIcon[gameMode];
            data.itemType = bt;
            data.mainID = main_id;
            data.subID = 0;// sub_id;
            if (data.mainID == 0 && data.itemType == ItemType.Emoji)
            {
                mainImg.sprite = Configs.Instance.emojiBall1.bg;
                subImg.sprite = Configs.Instance.emojiBall1.emojis[data.subID];
            }
            else if (data.mainID == 1 && data.itemType == ItemType.Emoji)
            {
                mainImg.sprite = Configs.Instance.emojiBall2.bg;
                subImg.sprite = Configs.Instance.emojiBall1.emojis[data.subID];
            }
            countText.text = string.Format("{0}", q);
            isUnlocked = CheckUnlock;
        }
        public void OnClick()
        {
            if (isUnlocked)
            {
                GameManager.Instance.PlayClick();
                UIManager.Instance.DisableAllSelected(data.itemType);
                TickMark = true;
                GameManager.Instance.ChangeBallHoop(data.mainID, data.itemType, data.subID);
                //UIManager.Instance.ShowScreen(0);
            }
        }
    }
}