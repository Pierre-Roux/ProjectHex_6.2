using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [SerializeField] public TMP_Text Choice_Text;
    [SerializeField] public TMP_Text Choice_Text2;

    public bool isElite;

    public void UpdateText(String Text1, String Text2)
    {
        Choice_Text.text = Text1;
        Choice_Text2.text = Text2;
    }
}
