namespace GameBench
{
    using UnityEngine;
    using UnityEngine.UI;

    public class Timer : MonoBehaviour
    {
        bool showTimeLeft = true, end = true, pause = false, run = false;
        float endTime, curTime, startTime, timeAvailable = 20;
        public Text timerText;
        public void EndTimer()
        {
            if (end) return;
            run = false;
            end = true;
            endTime = Time.time;
            SetActive(false);
        }

        public void RunTimer(float timeSec = 20)
        {
            doneOnce = false;
            SetActive(true);
            timeAvailable = timeSec;
            run = true;
            end = false;
            startTime = Time.time;
        }
        void Update()
        {
            if (pause)
            {
                startTime += Time.deltaTime;
                return;
            }
            if (run)
                curTime = Time.time - startTime;
            else
                curTime = endTime - startTime;
            float showTime = curTime;
            if (showTimeLeft)
            {
                showTime = timeAvailable - curTime;
                if (showTime <= 0)
                {
                    showTime = 0;
                    if (!doneOnce)
                    {
                        GameManager.Instance.GameOver();
                        doneOnce = true;
                    }

                }
            }
            float minutes = showTime / 60;
            float seconds = showTime % 60;
            float fraction = (showTime * 10) % 10;//Mathf.Clamp((showTime * 100) % 100, 0, 99.9f);
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", (int)minutes, seconds, fraction);
        }
        bool doneOnce = false;

        internal void SetActive(bool v)
        {
            gameObject.SetActive(v);
        }
    }
}