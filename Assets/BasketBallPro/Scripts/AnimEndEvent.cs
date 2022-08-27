namespace GameBench
{
    using UnityEngine;

    public class AnimEndEvent : MonoBehaviour
    {

        public void EndAnim()
        {
            //HourlyReward.Instance.RewardUser();
            //HourlyReward.Instance.coinAnim.SetActive(true);
            gameObject.SetActive(false);
        }
        public void RewardUser()
        {
            HourlyReward.Instance.RewardUser();
            HourlyReward.Instance.coinAnim.SetActive(false);
        }
        public void StartGame()
        {
            GameManager.Instance.StartGame();
        }


        public void StartSpin()
        {
            gameObject.SetActive(false);
            LuckyWheel.Instance.SpinNow();
        }
    }
}