using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using FMODUnity;
public class RewardSystem : Singleton<RewardSystem>
{

    [SerializeField] public Transform PilePoint;
    [SerializeField] public GameObject CardRewardPanel;
    [SerializeField] private GameObject CursorGameobject;
    [SerializeField] private LayerMask TargetingLayerMask;
    [SerializeField] private Button ButtonCardReward;
    [SerializeField] private Button ButtonMoneyReward;

    public bool CardSelectionMode;

    public void GainCard(Card card, CardView cardView = null)
    {
        if (cardView != null)
        {
            RuntimeManager.PlayOneShot(AudioManager.Instance.TakeCardRewardSound);
            cardView.RewardTaken = true;
            StartCoroutine(GainCardAnim(cardView));
        }
        else
        {
            CardRewardPanel.SetActive(false);
        }
        DataBase.Instance.DeckList.Add(card.data);
    }

    public IEnumerator GainCardAnim(CardView cardView)
    {
        cardView.transform.DOMove(PilePoint.position, 0.20f);
        Tween tween = cardView.transform.DOScale(Vector3.zero, 0.15f);
        //Tween tween = cardView.transform.DOMove(discardPilePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
        CardRewardPanel.SetActive(false);
    }

    public void Update()
    {
        if (CardSelectionMode)
        {
            if (Input.GetMouseButtonDown(0)) // 0 = clic gauche 1 = clic droit
            {
                Vector3 origin = CursorGameobject.transform.position + new Vector3(0, 0, -1);
                Vector3 direction = Vector3.forward;
                float distance = 10f;

                Debug.DrawRay(origin, direction * distance, Color.red, 1f);

                RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, TargetingLayerMask);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null && hit.transform.TryGetComponent(out CardView cardView))
                    {
                        if (cardView.IsReward && !cardView.RewardTaken)
                        {
                            RewardSystem.Instance.GainCard(cardView.Card, cardView);
                            CardSelectionMode = false;
                            ButtonCardReward.interactable = false;
                            CardRewardPanel.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void CloseCardRewardPanel()
    {
        CardRewardPanel.SetActive(false);
    }

    public void AddMoney(int Amount)
    {
        DataBase.Instance.Money += Amount;
        ButtonMoneyReward.interactable = false;
    }
}
