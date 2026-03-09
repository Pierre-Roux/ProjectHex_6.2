using System.Collections.Generic;
using FMODUnity;
using SerializeReferenceEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Data/NavCard")]
public class NavCardData : ScriptableObject
{
    [field: Header("Mandatory")]
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public Sprite BackgroundImage { get; private set; }

}
