using UnityEngine;
public class Tgl : MonoBehaviour
{
    public GameObject crossObj;

    public bool Enabled
    {
        set
        {
            crossObj.SetActive(!value);
        }
    }
}