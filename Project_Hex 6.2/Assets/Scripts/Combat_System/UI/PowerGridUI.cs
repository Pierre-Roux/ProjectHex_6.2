using TMPro;
using UnityEngine;

public class PowerGridUI : MonoBehaviour
{
    [SerializeField] private TMP_Text PowerGridText;
    [SerializeField] private GameObject PowerGridFlamme;

    public void UpdatePowerGridText(int AmountCurrent, int AmounMax)
    {
        if (AmountCurrent > AmounMax)
        {
            PowerGridFlamme.SetActive(true);
        }
        else
        {
            PowerGridFlamme.SetActive(false);
        }
        
        PowerGridText.text = AmountCurrent.ToString() + "/" + AmounMax.ToString();
    }
}
