using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : Singleton<HandView>
{
    [SerializeField] private SplineContainer SplineContainer;
    [SerializeField] public float cardSpacing = 0.1f;

    private readonly List<CardView> cards = new();

    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null) return null;
        cards.Remove(cardView);
        UpdateCardPos(0.15f);
        return cardView;
    }

    public CardView GetCardView(Card card)
    {
        return cards.Where(CardView => CardView.Card == card).FirstOrDefault();
    }

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        UpdateCardPos(0.15f);
        yield return null;
    }

    public void UpdateCardPos(float Duration)
    {
        StartCoroutine(UpdateCardPosition(Duration));
    }

    private IEnumerator UpdateCardPosition(float duration)
    {
        if (cards.Count == 0) yield break;

        float totalWidth = (cards.Count - 1) * cardSpacing;
        if (totalWidth > 1f)
        {
            cardSpacing = 1f / (cards.Count - 1);
        }
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing * 0.5f;

        //float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;
        Spline spline = SplineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            p = Mathf.Clamp(p,0.0001f,0.9999f);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            forward.Normalize();
            Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(0, 90, 0);
            Vector3 targetPos = splinePosition + transform.position + 0.01f * i * Vector3.back;
            Quaternion targetRot = rotation;

            cards[i].transform.DOMove(targetPos, duration);
            cards[i].transform.DORotateQuaternion(targetRot, duration);
        }

        yield return new WaitForSeconds(duration); 
    } 
}
