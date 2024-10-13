using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������������� ����������, �������� � ������������ ����� - ��������, �����������, ���������, ��������, ��������
/// </summary>
public class CardController 
{

    public CardSpriteManager cardSpriteManager;

    #region main actions with view

    /// <summary>
    /// �������� ������ �� View, ���� ��� - �������� ���������
    /// </summary>
    public void InitCardView(GameObject card, Card cardModel)
    {
        if (card.TryGetComponent<CardView>(out CardView cardView))
        {
            cardView = card.AddComponent<CardView>();
        }
        cardModel.CardView = cardView;
    }

    public void AddOnClickListener(Card card, Action onClick)
    {
        card.CardView.cardButton.onClick.AddListener(() => onClick());
    }

    /// <summary>
    /// ��������� ������� � ����������� �� ���������� ������ �����
    /// </summary>
    public void UpdateView(Card cardModel)
    {
        cardSpriteManager.UpdateView(cardModel, cardModel.CardView);

    }

    /// <summary>
    /// ����������� �� ��������� ������� �� ����, ����������� � ��������
    /// </summary>
    public void SetToStartPos(Card card, Transform parent)
    {
        card.CardView.ReturnToStartPos();
        card.CardView.gameObject.transform.parent = parent;
    }

    public void Remove(Card card)
    {
        card.CardView.DestroyCard();
    }



    #endregion main actions with view

    #region to combination place animations and callbacks

    /// <summary>
    /// �������� ����� ���� - ����������� � ������ ����������
    /// </summary>
    public void AnimateToCombinationPlace(Card card, Transform combinationPanel)
    {
        // Callback - ����������� � ��������
        card.CardView.GoToCombinationPlace(combinationPanel, () => SendCardToCombinationPlace(card.CardView, combinationPanel));
    }

    public void SendCardToCombinationPlace(CardView cardView, Transform combinationPanel)
    {
        // ����������� �������� � ��������, ���������� �������
        cardView.gameObject.transform.SetParent(combinationPanel);
        cardView.cardButton.enabled = false;
    }

    /// <summary>
    /// �������� ����� ����� - ����������� � ������ ����������
    /// </summary>
    public void AnimateBankToCombinationPlace(Card card, Transform combinationPanel)
    {
        card.CardView.GoToCombinationPlaceBank(combinationPanel, () => SendBankCardToCombinationPlace(card, combinationPanel));
    }

    public void SendBankCardToCombinationPlace(Card card, Transform combinationPanel)
    {
        card.CardView.gameObject.transform.SetParent(combinationPanel); 
        UnlockCard(card);
        card.CardView.cardButton.enabled = false;

    }

    #endregion to combination place animations and callbacks

    #region unlock animations and callbacks
    public void UnlockParentCardWithAnim(Card card, List<Card> topCards)
    {
        // ��������� ����� � ������ ����������� � ��������� � ���������� �������
        var parent = card.Parent;
        if (parent != null)
        {
            parent.CardView.OpenCard(() => UnlockCard(parent));
        }
        topCards[card.GroupNum] = parent;
    }
    public void UnlockCardWithAnim(Card card)
    {
        // ��������� ����� � ���������
        card.CardView.OpenCard(() => UnlockCard(card));
    }

    public void UnlockParentBankCard(Card card)
    {
        // ����� ����� ���������� �������� ��� ������� 
        if(card.Parent != null)
        {
            card.Parent.CardView.cardButton.enabled = true;
        }
        
    }
    public void UnlockCard(Card card)
    {
        // ����������� �������� ������� �����
        card.FacedUp = true;
        cardSpriteManager.UpdateView(card, card.CardView);
    }

    #endregion unlock animations and callbacks


}