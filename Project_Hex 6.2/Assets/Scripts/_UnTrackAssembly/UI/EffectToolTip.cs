using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectToolTip : MonoBehaviour
{
    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Description;
    [SerializeField] public Image Image;
    [SerializeField] public Transform Point1;
    [SerializeField] public Transform Point2;

    public void Set(string title, string description, Sprite image)
    {
        Title.text = title;
        Description.text = description;
        Image.sprite = image;
        Image.SetNativeSize();
    }

    public void Reset()
    {
        Title.text = "";
        Description.text = "";
        Image.sprite = null;
    }

    public IEnumerator Appear()
    {
        Tween tween = transform.DOMove(Point2.position, 0.2f);
        yield return tween.WaitForCompletion();
    }

    public IEnumerator Disappear()
    {
        Tween tween = transform.DOMove(Point1.position, 0.2f);
        yield return tween.WaitForCompletion();
    }
}
