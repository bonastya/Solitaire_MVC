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
        card.CardView.GoToCombinationPlace(combinationPanel, () => SendBankCardToCombinationPlace(card, combinationPanel));
    }
    public void SendCardToCombinationPlace(CardView cardView, Transform combinationPanel)
    {
        cardView.gameObject.transform.SetParent(combinationPanel);
        cardView.cardButton.onClick.RemoveAllListeners();
    }
    public void SendBankCardToCombinationPlace(Card card, Transform combinationPanel)
    {
        card.CardView.gameObject.transform.SetParent(combinationPanel);
        card.CardView.cardButton.onClick.RemoveAllListeners();

        card.FacedUp = true;
        cardSpriteManager.UpdateView(card, card.CardView);

    }
    public void UnlockParentCard(Card card)
    {
        var parent = card.Parent;
        if(parent != null)
        {
            parent.FacedUp = true;
            cardSpriteManager.UpdateView(parent, parent.CardView);
        }
        
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


}