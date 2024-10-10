using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
//запуск анимаций
//переключение спрайтов
//обновление модели карты
public class CardController 
{
    public CardSpriteManager cardSpriteManager;

    public void InitCardView(GameObject card, Card cardModel)
    {
        // Оставить ссылку на View, если нет - добавить компонент
        if (card.TryGetComponent<CardView>(out CardView cardView))
        {
            cardView = card.AddComponent<CardView>();
        }
        cardModel.CardView = cardView;
    }
    public void UpdateView(Card cardModel)
    {
        cardSpriteManager.UpdateView(cardModel, cardModel.CardView);
    }

    public void AddOnClickListener(Card card, Action onClick)
    {
        card.CardView.cardButton.onClick.AddListener(() => onClick());
    }

    public void AnimateToCombinationPlace(Card card, Transform combinationPanel)
    {
        card.CardView.GoToCombinationPlace(combinationPanel, () => SendCardToCombinationPlace(card.CardView, combinationPanel));
    }
    public void AnimateBankToCombinationPlace(Card card, Transform combinationPanel)
    {

        card.CardView.GoToCombinationPlaceBank(combinationPanel, () => SendBankCardToCombinationPlace(card, combinationPanel));
    }
    public void SendCardToCombinationPlace(CardView cardView, Transform combinationPanel)
    {
        cardView.gameObject.transform.SetParent(combinationPanel);
        cardView.cardButton.enabled = false;
    }
    public void SendBankCardToCombinationPlace(Card card, Transform combinationPanel)
    {
        card.CardView.gameObject.transform.SetParent(combinationPanel);
        card.CardView.cardButton.enabled = false;

        UnlockCard(card);
    }
    public void UnlockParentCardWithAnim(Card card)
    {
        var parent = card.Parent;
        if(parent != null)
        {
            parent.CardView.OpenCard(()=>UnlockCard(parent));
        }
        
    }
    public void UnlockCardWithAnim(Card card)
    {
        card.CardView.OpenCard(() => UnlockCard(card));
    }

    public void UnlockParentBankCard(Card card)
    {
        var parent = card.Parent;
        if(parent != null)
        {
            parent.CardView.cardButton.enabled = true;
        }
        
    }

    public void UnlockCard(Card card)
    {
        card.FacedUp = true;
        cardSpriteManager.UpdateView(card, card.CardView);
    }
    public void UnlockBanckCard(Card card)
    {
        card.CardView.cardButton.enabled = true;
    }
    public void Remove(Card card)
    {
        card.CardView.DestroyCard();
    }

    public void SetToStartPos(Card card, Transform parent)
    {
        card.CardView.ReturnToStartPos();
        card.CardView.gameObject.transform.parent = parent;
    }




}