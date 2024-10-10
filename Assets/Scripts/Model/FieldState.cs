using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements;

[Serializable]
public class FieldState
{
    public List<List<Card>> CardGroups { get; private set; }   // Группы карт
    public List<Card?> TopCardsInGroups { get; private set; }   // Верхние карты каждой группы
    public List<List<(CardValue, CardSuit)>> CardCombinations { get; private set; }   // Комбинации

    public int CardsNumber { get;  set; }
    public int CurrentFieldCardsNumber { get;  set; }
    public int CurrentBankCardsNumber { get;  set; }

    public List<Card> Bank { get; private set; }               // Банк карт

    public bool IFcardsPositionedCorrectly;

    public Card currentCard;


    public FieldState() {
        CardGroups = new List<List<Card>>();
        Bank = new List<Card>();
        TopCardsInGroups = new List<Card>();
        CardCombinations = new List<List<(CardValue, CardSuit)>>();
    }


}
