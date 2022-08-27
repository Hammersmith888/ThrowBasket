namespace GameBench
{
    using UnityEngine;
    public class WheelSlice : MonoBehaviour
    {
        public SpriteRenderer bgSpRend, iconSpRend;
        public TextMesh valueText;
        public RewardInfo rewardInfo = new RewardInfo();
        public void SetValues(Sprite bgSp, Sprite iconSp, int iQ = 0, RewardType rewardType = RewardType.Ball)
        {
            bgSpRend.sprite = bgSp;
            iconSpRend.sprite = rewardInfo.sp = iconSp;
            rewardInfo.rewardType = rewardType;
            rewardInfo.idQ = iQ;
            valueText.gameObject.SetActive(rewardType == RewardType.Coin);
            valueText.text = GameManager.GetValueFormated(rewardInfo.idQ);
        }
        public void SetValues(Sprite bgSp, int id, Sprite iconSp, int q = 0)
        {
            bgSpRend.sprite = bgSp;
            iconSpRend.sprite = iconSp;
            if (q > 0)
                valueText.text = GameManager.GetValueFormated(q);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            LuckyWheel.Instance.SelectedReward = rewardInfo;
        }
    }

    public struct RewardInfo
    {
        public int idQ;
        public RewardType rewardType;
        public Sprite sp;
    }
}