namespace GameBench
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class AnimExit : MonoBehaviour
    {
        public void OnAnimEnd()
        {
            gameObject.SetActive(false);
        }

        public void EndSplash()
        {
            //gameObject.SetActive(false);
            //SceneManager.LoadScene(1);
        }
    }
}