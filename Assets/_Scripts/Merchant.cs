using TMPro;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public GameObject merchantText;
    public GameObject cam;

    private void Update()
    {
        merchantText.transform.LookAt(transform.position + cam.transform.forward);
    }
}
