namespace GameBench
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class HoopData : MonoBehaviour
    {
        public SpriteRenderer[] hoopRend;
        //public GameObject[] colliderObj;
        public Animator _netAnim;
        public GameObject coinObj;
        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        //public bool EnableCollider
        //{
        //    set
        //    {
        //        foreach (var item in colliderObj)
        //        {
        //            item.SetActive(value);
        //        }
        //    }
        //}
        public void SetHoop(int i)
        {
            hoopRend[0].sprite = Configs.Instance.hoopSprites[i].hoopSp;
            hoopRend[1].sprite = Configs.Instance.hoopSprites[i].stripSp;
        }
        //public void SetActive01(bool first, bool second)
        //{
        //    hoopRend[0].gameObject.SetActive(first);
        //    hoopRend[1].gameObject.SetActive(second);
        //}
        //public void SetActiveBg(bool state)
        //{
        //    hoopRend[1].gameObject.SetActive(state);
        //}
        //public void SetHoop(Sprite sp1, Sprite sp2)
        //{
        //    hoopRend[0].sprite = sp1;
        //    hoopRend[1].sprite = sp2;
        //}
    }
}