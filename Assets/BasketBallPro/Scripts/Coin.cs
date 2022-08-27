namespace GameBench
{
    using UnityEngine;
    public class Coin : MonoBehaviour
    {
        //public Animator _animators;
        //public IEnumerator MoveToPosition(Transform obj, Vector3 targetPos, float delay = 0.5f)
        //{
        //    yield return new WaitForSeconds(0.05f);
        //    float elapsedTime = 0.0f;
        //    while (elapsedTime < delay)
        //    {
        //        obj.localPosition = Vector3.Lerp(obj.localPosition, targetPos, elapsedTime / delay);
        //        elapsedTime += Time.deltaTime;
        //        yield return null;
        //    }
        //    obj.localPosition = targetPos;
        //}
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!GameManager.Instance.shootBall.movedOnce)
                return;
            if (collision.gameObject.name.Contains("Ball"))
            {
                GameManager.Instance.Coins++;
                gameObject.SetActive(false);
                GameManager.Instance.PlaySfx(SFX.Claim1Coin);
            }
        }
    }
}