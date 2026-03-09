using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] public TMP_Text Cost;
    [SerializeField] public int OverrideRarity =-1;
    [SerializeField] public int PriceAdded =0;
    [SerializeField] public float PriceMultiply =1;
    [SerializeField] public bool InfinitSlot = false;
    [SerializeField] public GameObject CardParent;
    [HideInInspector] public CardView HoldedCardView;
}
