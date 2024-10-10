using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements;

[Serializable]
public class FieldState
{
    public List<List<Card>> CardGroups { get; private set; }   // ������ ����
    public List<Card?> TopCardsInGroups { get; private set; }   // ������� ����� ������ ������
    public List<List<(CardValue, CardSuit)>> CardCombinations { get; private set; }   // ����������

    public int CardsNumber { get;  set; }
    public int CurrentFieldCardsNumber { get;  set; }
    public int CurrentBankCardsNumber { get;  set; }

    public List<Card> Bank { get; private set; }               // ���� ����

    public bool IFcardsPositionedCorrectly;

    public Card currentCard;


    public FieldState() {
        CardGroups = new List<List<Card>>();
        Bank = new List<Card>();
        TopCardsInGroups = new List<Card>();
        CardCombinations = new List<List<(CardValue, CardSuit)>>();
    }


}
