namespace GameBench
{
    using UnityEngine;
    using UnityEngine.UI;
    //Class that holds items for Our Custom dynamic Leaderboard ListView
    public class LeaderItem : MonoBehaviour
    {
        [HideInInspector]
        public string fId, picUrl; // holds id and pic URL returned from server.
        public Text txtName, txtScore, txtRank; //Used to Store and Display Name, Score and Rank from the server
        public Image imgPic; // Image View to show image of the specified ID

        public void AssignValues(string fId, string txtName, string txtScore, string rank)
        {
            this.fId = fId;
            this.txtName.text = txtName;
            this.txtScore.text = txtScore;
            txtRank.text = rank;
        }
    }
}