using UnityEngine;
using System.Collections;

public class TestCode : MonoBehaviour
{
    void Start()
    {
        GameBench.RateManager.Instance.Show();
    }

    public void ResetRate()
    {
        GameBench.RateManager.Instance.Show();
    }
}
