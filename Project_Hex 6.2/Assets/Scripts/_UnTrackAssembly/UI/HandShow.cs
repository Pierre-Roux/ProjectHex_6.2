using DG.Tweening;
using UnityEngine;

public class HandShow : MonoBehaviour
{
    [SerializeField] private Transform hand;

    [SerializeField] private Vector3 hiddenPosition;
    [SerializeField] private Vector3 shownPosition;

    [SerializeField] private float triggerHeight = 150f;
    [SerializeField] private float duration = 0.25f;

    private bool isOpen;
    private Tween currentTween;

    void Update()
    {
        bool shouldOpen = Input.mousePosition.y <= triggerHeight;

        if (shouldOpen == isOpen)
            return;

        isOpen = shouldOpen;

        currentTween?.Kill();

        currentTween = hand.DOLocalMove(
            isOpen ? shownPosition : hiddenPosition,
            duration)
            .SetEase(Ease.OutCubic);
    }
}
