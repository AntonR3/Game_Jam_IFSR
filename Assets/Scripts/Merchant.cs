using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    public GameObject merchantText;
    [SerializeField] TextMeshProUGUI merchantTextData;
    public GameObject cam;

    private void Update()
    {
        merchantText.transform.LookAt(transform.position + cam.transform.forward);
    }

    public void ShowUpgradeShop()
    {
        merchantText.SetActive(true);
    }
    
    public void ChangeMerchantText(string text)
    {
        merchantTextData.text = text;
    }
}
