using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [SerializeField] public TMP_Text Choice_Text;
    [SerializeField] public int index;

    public void Start()
    {
        Choice_Text.text = CharacterSelectorSystem.Instance.Characters[index].name;
    }
}
