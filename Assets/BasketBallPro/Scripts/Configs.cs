namespace GameBench
{
    using UnityEngine;
    public class Configs : MonoBehaviour
    {
        public IAPInfoItem[] iapItems;
        public static int freeSpinTime = 10;
        public Hoop[] hoopSprites;
        public Sprite[] simpleBalls;
        public Sprite[] globeBallBg, globeBallCh;
        public EmojiBall emojiBall1, emojiBall2;
        public Sprite[] simpleCarrierBalls;
        public Sprite[] gameModeIcon;
        [Range(1, 5)]
        public int speedMultiplier;
        [Range(2, 10)]
        public int duration;
        public Sprite[] chunkSp;
        public AnimationCurve animationCurve;
        private static Configs _instance;
        public int spinTurnCost;
        public static Configs Instance
        { get { if (_instance == null) _instance = FindObjectOfType<Configs>(); return _instance; } }

        public const string SIMPLE_BALLS = "SIMPLEBALL_", RARE = "RARE_", HOOP = "_HOOP_",
            EMOJI_BALLS = "EMOJI_BALLS", GLOBE_BALLS = "GLOBE_BALLS";

        public static string GetCBallKey(int bgID, int iconID = 0, bool emoji = false)
        {
            return string.Format("{0}{1}{2}", bgID, emoji ? EMOJI_BALLS : GLOBE_BALLS, iconID);
        }
        public static string GetBallKey(int i)
        {
            return string.Format("{0}{1}", SIMPLE_BALLS, i);
        }
        public static string GetHoopKey(int i)
        {
            return string.Format("{0}{1}", HOOP, i);
        }
        [HideInInspector]
        internal int[] ballUnlockCondition = new int[5] { 5, 10, 20, 30, 50 };

        [HideInInspector]
        internal int[] hoopUnlockCondition = new int[4] { 20, 50, 100, 300 };
    }
    [System.Serializable]
    public class EmojiBall
    {
        public Sprite bg;
        public Sprite[] emojis;
    }
    [System.Serializable]
    public class Hoop
    {
        public Sprite bgSprite, hoopSp, stripSp;
    }
}