namespace GameBench
{
    using UnityEngine;
    [ExecuteInEditMode]
    public class ChangeSortingOrder : MonoBehaviour
    {
        // Use this for initialization
        public int order = 8;

        void Start()
        {
            GetComponent<MeshRenderer>().sortingOrder = order;
        }
        public void DisableObject()
        {
            gameObject.SetActive(false);
        }
    }
}